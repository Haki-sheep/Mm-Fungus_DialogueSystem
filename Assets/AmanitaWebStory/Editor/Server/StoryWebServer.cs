using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace AmanitaWebStory.Editor
{
    [InitializeOnLoad]
    public static class StoryWebServer
    {
        private const int MaxBodyBytes = 4 * 1024 * 1024;
        private static readonly TimeSpan ShutdownTimeout = TimeSpan.FromSeconds(2);
        private const string EnabledKey = "AmanitaWebStory.Enabled";
        private const string PortKey = "AmanitaWebStory.Port";
        private const string TokenKey = "AmanitaWebStory.Token";
        private static readonly ConcurrentQueue<Action> requestQueue = new ConcurrentQueue<Action>();
        private static HttpListener listener;
        private static CancellationTokenSource lifetime;
        private static string token;
        private static string origin;
        private static string endpointOwner;
        private static byte[] html;
        private static Task serverTask;
        private static bool startPending;

        public static string Url => origin + "/#token=" + token;

        static StoryWebServer()
        {
            EditorApplication.update += ProcessRequests;
            AssemblyReloadEvents.beforeAssemblyReload += () => Stop(false);
            EditorApplication.quitting += () => Stop(true);
            startPending = SessionState.GetBool(EnabledKey, false);
        }

        #region 服务生命周期

        [MenuItem("Tools/MmFungus/打开网页剧情编辑器", false, 0)]
        public static void Open() => Start(true);

        public static void Start(bool OpenBrowser)
        {
            startPending = false;
            if (listener != null)
            {
                StoryCliEndpoint.Publish(origin, token, endpointOwner);
                if (OpenBrowser) Application.OpenURL(Url);
                return;
            }
            html = File.ReadAllBytes(StoryAssetExporter.Root + "/Web/index.html");
            int Port = SessionState.GetInt(PortKey, 0);
            if (Port == 0)
            {
                var Probe = new TcpListener(IPAddress.Loopback, 0);
                Probe.Start();
                Port = ((IPEndPoint)Probe.LocalEndpoint).Port;
                Probe.Stop();
            }
            token = SessionState.GetString(TokenKey, "");
            if (string.IsNullOrEmpty(token)) token = Guid.NewGuid().ToString("N");
            origin = "http://127.0.0.1:" + Port;
            var NewListener = new HttpListener();
            NewListener.Prefixes.Add(origin + "/");
            try { NewListener.Start(); }
            catch { NewListener.Close(); throw; }
            string NewOwner = Guid.NewGuid().ToString("N");
            try { StoryCliEndpoint.Publish(origin, token, NewOwner); }
            catch { NewListener.Close(); throw; }
            endpointOwner = NewOwner;
            listener = NewListener;
            lifetime = new CancellationTokenSource();
            SessionState.SetBool(EnabledKey, true);
            SessionState.SetInt(PortKey, Port);
            SessionState.SetString(TokenKey, token);
            serverTask = ListenAsync(NewListener, lifetime.Token);
            if (OpenBrowser) Application.OpenURL(Url);
            Debug.Log("[网页剧情] 本地编辑器已启动 " + origin);
        }

        [MenuItem("Tools/MmFungus/停止网页剧情服务", false, 1)]
        public static void Close() => Stop(true);

        private static void Stop(bool Disable)
        {
            startPending = false;
            if (Disable) SessionState.SetBool(EnabledKey, false);
            var ActiveLifetime = lifetime;
            var ActiveListener = listener;
            var ActiveTask = serverTask;
            string ActiveOwner = endpointOwner;
            listener = null;
            lifetime = null;
            serverTask = null;
            endpointOwner = null;
            try
            {
                ActiveLifetime?.Cancel();
                ActiveListener?.Close();
                if (ActiveTask != null && !ActiveTask.Wait(ShutdownTimeout))
                    throw new TimeoutException("网页剧情服务停止超时 无法确认旧监听端口已释放");
            }
            finally
            {
                ActiveLifetime?.Dispose();
                StoryCliEndpoint.Remove(ActiveOwner);
                while (requestQueue.TryDequeue(out var Request)) Request();
            }
        }

        private static void ProcessRequests()
        {
            if (serverTask != null && serverTask.IsFaulted)
            {
                var Failure = serverTask.Exception;
                serverTask = null;
                Stop(true);
                Debug.LogException(Failure);
            }
            if (EditorApplication.isCompiling || EditorApplication.isUpdating) return;
            if (startPending)
                Start(false);
            for (int Index = 0; Index < 8 && requestQueue.TryDequeue(out var Request); Index++) Request();
        }

        #endregion

        #region 请求协议

        private static async Task ListenAsync(HttpListener ActiveListener, CancellationToken Token)
        {
            try
            {
                while (!Token.IsCancellationRequested)
                {
                    var Context = await ActiveListener.GetContextAsync().ConfigureAwait(false);
                    await HandleAsync(Context, Token).ConfigureAwait(false);
                }
            }
            catch (Exception Exception) when (Token.IsCancellationRequested &&
                (Exception is HttpListenerException || Exception is ObjectDisposedException || Exception is OperationCanceledException)) { }
        }

        private static async Task HandleAsync(HttpListenerContext Context, CancellationToken Token)
        {
            bool ResponseStarted = false;
            bool ReadingBody = false;
            try
            {
                var Request = Context.Request;
                Context.Response.Headers["Cache-Control"] = "no-store";
                Context.Response.Headers["X-Content-Type-Options"] = "nosniff";
                Context.Response.Headers["Content-Security-Policy"] = "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; img-src 'self' data:; connect-src 'self'; frame-ancestors 'none'";
                if (Request.Url.AbsolutePath == "/" && Request.HttpMethod == "GET")
                {
                    Context.Response.ContentType = "text/html; charset=utf-8";
                    ResponseStarted = true;
                    await WriteAsync(Context, html).ConfigureAwait(false);
                    return;
                }
                if (Request.Headers["Authorization"] != "Bearer " + token ||
                    !string.IsNullOrEmpty(Request.Headers["Origin"]) && Request.Headers["Origin"] != origin)
                {
                    Context.Response.StatusCode = 403;
                    ResponseStarted = true;
                    await WriteJsonAsync(Context, new { error = "请从 Unity 菜单重新打开网页编辑器" }).ConfigureAwait(false);
                    return;
                }
                string Body = "";
                if (Request.HttpMethod == "POST")
                {
                    ReadingBody = true;
                    if (Request.ContentLength64 > MaxBodyBytes) throw new ArgumentException("剧情数据超过4MB限制");
                    using (var Buffer = new MemoryStream())
                    {
                        byte[] ChunkList = new byte[8192];
                        int Count;
                        while ((Count = await Request.InputStream.ReadAsync(ChunkList, 0, ChunkList.Length, Token).ConfigureAwait(false)) > 0)
                        {
                            if (Buffer.Length + Count > MaxBodyBytes) throw new ArgumentException("剧情数据超过4MB限制");
                            Buffer.Write(ChunkList, 0, Count);
                        }
                        Body = Encoding.UTF8.GetString(Buffer.ToArray());
                    }
                    ReadingBody = false;
                }
                string CapturedBody = Body;
                var Completion = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
                using (Token.Register(() => Completion.TrySetCanceled()))
                {
                    requestQueue.Enqueue(() =>
                    {
                        if (Token.IsCancellationRequested) return;
                        try { Completion.TrySetResult(Dispatch(Request.HttpMethod, Request.Url, CapturedBody)); }
                        catch (Exception Exception) { Completion.TrySetException(Exception); }
                    });
                    object Result = await Completion.Task.ConfigureAwait(false);
                    ResponseStarted = true;
                    await WriteJsonAsync(Context, Result).ConfigureAwait(false);
                }
            }
            catch (Exception Exception) when ((ResponseStarted || ReadingBody) &&
                (Exception is HttpListenerException || Exception is IOException || Exception is ObjectDisposedException))
            {
            }
            catch (OperationCanceledException) when (Token.IsCancellationRequested) { }
            catch (Exception Exception)
            {
                Context.Response.StatusCode = Exception is ArgumentException || Exception is JsonException || Exception is InvalidOperationException ? 400 : 500;
                try { await WriteJsonAsync(Context, new { error = Exception.Message }).ConfigureAwait(false); }
                catch (HttpListenerException) { }
                catch (IOException) { }
                catch (ObjectDisposedException) { }
            }
            finally { Context.Response.Close(); }
        }

        private static object Dispatch(string Method, Uri Uri, string Body)
        {
            string Path = Uri.AbsolutePath;
            if (Path.StartsWith("/api/cli/", StringComparison.Ordinal)) return StoryCliEndpoint.Dispatch(Method, Uri, Body);
            if (Method == "GET" && Path == "/api/catalog") return StoryCatalog.Read();
            if (Method == "GET" && Path == "/api/story")
            {
                if (!Uri.Query.StartsWith("?id=", StringComparison.Ordinal)) throw new ArgumentException("缺少剧情标识");
                return StoryImporter.Read(System.Uri.UnescapeDataString(Uri.Query.Substring(4)));
            }
            if (Method == "POST" && Path == "/api/validate")
            {
                var Document = StoryValidator.Parse(Body, out var Warnings);
                return new { nodes = Document.nodes.Count, warnings = Warnings };
            }
            if (Method == "POST" && Path == "/api/import")
            {
                var Data = JObject.Parse(Body);
                return StoryImporter.Import(Data["document"].ToString(), Data["revision"]?.Value<string>() ?? "");
            }
            if (Method == "POST" && Path == "/api/select")
            {
                string Id = JObject.Parse(Body)["id"].Value<string>();
                var Asset = StoryImporter.Load(Id);
                Selection.activeObject = Asset;
                EditorGUIUtility.PingObject(Asset);
                return new { selected = true };
            }
            if (Method == "POST" && Path == "/api/delete")
            {
                var Data = JObject.Parse(Body);
                return StoryDeletion.Delete(Data["id"].Value<string>(), Data["revision"].Value<string>());
            }
            throw new ArgumentException("不支持的编辑操作 " + Method + " " + Path);
        }

        private static Task WriteJsonAsync(HttpListenerContext Context, object Value)
        {
            Context.Response.ContentType = "application/json; charset=utf-8";
            return WriteAsync(Context, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Value)));
        }

        private static Task WriteAsync(HttpListenerContext Context, byte[] Data)
        {
            Context.Response.ContentLength64 = Data.Length;
            return Context.Response.OutputStream.WriteAsync(Data, 0, Data.Length);
        }

        #endregion
    }
}

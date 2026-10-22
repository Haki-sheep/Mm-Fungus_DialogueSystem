using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace AmanitaWebStory.Editor
{
    public static class StoryCliEndpoint
    {
        public const int ProtocolVersion = 1;
        private static string EndpointPath => Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Library/AmanitaWebStory/endpoint.json");

        public static void Publish(string Origin, string Token, string Owner)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(EndpointPath));
            string PendingPath = EndpointPath + ".tmp";
            File.WriteAllText(PendingPath, JsonConvert.SerializeObject(new
            {
                protocolVersion = ProtocolVersion,
                instanceId = Owner,
                projectPath = Directory.GetParent(Application.dataPath).FullName,
                origin = Origin,
                token = Token
            }, Formatting.Indented));
            if (File.Exists(EndpointPath)) File.Replace(PendingPath, EndpointPath, null);
            else File.Move(PendingPath, EndpointPath);
        }

        public static void Remove(string Owner)
        {
            if (string.IsNullOrEmpty(Owner) || !File.Exists(EndpointPath)) return;
            var Endpoint = JObject.Parse(File.ReadAllText(EndpointPath));
            if (Endpoint.Value<string>("instanceId") == Owner) File.Delete(EndpointPath);
        }

        public static object Dispatch(string Method, Uri RequestUri, string Body)
        {
            if (Method != "GET") throw new ArgumentException("CLI 查询仅支持 GET");
            switch (RequestUri.AbsolutePath)
            {
                case "/api/cli/status":
                    return new
                    {
                        protocolVersion = ProtocolVersion,
                        projectPath = Directory.GetParent(Application.dataPath).FullName,
                        unityVersion = Application.unityVersion,
                        playing = EditorApplication.isPlayingOrWillChangePlaymode,
                        compiling = EditorApplication.isCompiling,
                        compilationFailed = EditorUtility.scriptCompilationFailed,
                        updating = EditorApplication.isUpdating
                    };
                case "/api/cli/capabilities":
                    return new
                    {
                        protocolVersion = ProtocolVersion,
                        commands = new[] { "capabilities", "list", "read", "validate", "import", "delete", "audit", "status" },
                        schema = JObject.Parse(File.ReadAllText(StoryAssetExporter.Root + "/CLI/story.schema.json")),
                        catalog = StoryCatalog.Read(),
                        rules = new[]
                        {
                            "资源与角色标识从 catalog 获取",
                            "更新或删除已有剧情必须提交 read 返回的 revision",
                            "新建剧情 revision 留空 已有剧情不可强制覆盖",
                            "事件只支持 signal 模式 游戏行为由项目订阅 StoryPlayer.EventRaised 实现",
                            "import 与 delete 禁止在 Play 时执行",
                            "CLI 不执行任意代码 不切换或保存当前场景"
                        }
                    };
                case "/api/cli/list":
                    return JObject.FromObject(StoryCatalog.Read())["stories"];
                case "/api/cli/audit":
                    return Audit(ReadPath(RequestUri));
                default:
                    throw new ArgumentException("未知 CLI 查询 " + RequestUri.AbsolutePath);
            }
        }

        private static string ReadPath(Uri RequestUri)
        {
            if (!RequestUri.Query.StartsWith("?id=", StringComparison.Ordinal))
                throw new ArgumentException("audit 需要 id 参数");
            return StoryImporter.AssetPath(Uri.UnescapeDataString(RequestUri.Query.Substring(4)));
        }

        private static object Audit(string AssetPath)
        {
            var Story = AssetDatabase.LoadAssetAtPath<StoryAsset>(AssetPath);
            if (Story == null)
                throw new FileNotFoundException("找不到剧情资产 " + AssetPath);
            var MissingReferenceList = new List<object>();
            using (var Serialized = new SerializedObject(Story))
            {
                var Property = Serialized.GetIterator();
                while (Property.Next(true))
                    if (Property.propertyType == SerializedPropertyType.ObjectReference &&
                        Property.objectReferenceValue == null && Property.objectReferenceInstanceIDValue != 0)
                        MissingReferenceList.Add(new { property = Property.propertyPath });
            }
            return new
            {
                assetPath = AssetPath,
                valid = MissingReferenceList.Count == 0,
                dependencies = AssetDatabase.GetDependencies(AssetPath, true).OrderBy(Path => Path, StringComparer.Ordinal).ToArray(),
                missingReferences = MissingReferenceList,
                scope = "检查剧情资产的角色 立绘 音频引用与递归资源依赖 不检查场景 不删除资源"
            };
        }
    }
}

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
                        schema = JObject.Parse(File.ReadAllText(StoryTemplateBuilder.Root + "/CLI/story.schema.json")),
                        catalog = StoryCatalog.Read(),
                        rules = new[]
                        {
                            "资源与角色标识从 catalog 获取",
                            "更新或删除已有剧情必须提交 read 返回的 revision",
                            "新建剧情 revision 留空 已有剧情不可强制覆盖",
                            "signal 事件生成事件 SO 接收后的游戏行为仍由项目组件实现",
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
            if (string.IsNullOrEmpty(RequestUri.Query)) return StoryTemplateBuilder.TemplatePath;
            if (!RequestUri.Query.StartsWith("?path=", StringComparison.Ordinal))
                throw new ArgumentException("audit 只接受 path 参数");
            return Uri.UnescapeDataString(RequestUri.Query.Substring(6));
        }

        private static object Audit(string AssetPath)
        {
            string Normalized = AssetPath.Replace('\\', '/');
            bool IsTemplate = Normalized == StoryTemplateBuilder.TemplatePath;
            bool IsGenerated = Normalized.StartsWith(StoryTemplateBuilder.GeneratedRoot + "/", StringComparison.Ordinal) &&
                Normalized.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase) &&
                Normalized.Substring(StoryTemplateBuilder.GeneratedRoot.Length + 1).IndexOf('/') < 0;
            if (!IsTemplate && !IsGenerated)
                throw new ArgumentException("audit 只检查剧情模板或 Generated 目录直属预制体 " + AssetPath);
            var Asset = AssetDatabase.LoadAssetAtPath<GameObject>(Normalized);
            if (Asset == null || Asset.GetComponent<WebStoryHost>() == null)
                throw new FileNotFoundException("找不到剧情预制体 " + Normalized);
            var MissingScriptList = new List<object>();
            var MissingReferenceList = new List<object>();
            foreach (var Transform in Asset.GetComponentsInChildren<Transform>(true))
            {
                string ObjectPath = AnimationUtility.CalculateTransformPath(Transform, Asset.transform);
                int Count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(Transform.gameObject);
                if (Count > 0) MissingScriptList.Add(new { objectPath = ObjectPath, count = Count });
                foreach (var Component in Transform.GetComponents<Component>())
                {
                    if (Component == null) continue;
                    using (var Serialized = new SerializedObject(Component))
                    {
                        var Property = Serialized.GetIterator();
                        while (Property.Next(true))
                            if (Property.propertyType == SerializedPropertyType.ObjectReference &&
                                Property.objectReferenceValue == null && Property.objectReferenceInstanceIDValue != 0)
                                MissingReferenceList.Add(new { objectPath = ObjectPath, component = Component.GetType().FullName, property = Property.propertyPath });
                    }
                }
            }
            return new
            {
                assetPath = Normalized,
                valid = MissingScriptList.Count == 0 && MissingReferenceList.Count == 0,
                dependencies = AssetDatabase.GetDependencies(Normalized, true).OrderBy(Path => Path, StringComparer.Ordinal).ToArray(),
                missingScripts = MissingScriptList,
                missingReferences = MissingReferenceList,
                scope = "检查指定预制体中的组件引用与递归资源依赖 不检查场景 不删除资源"
            };
        }
    }
}

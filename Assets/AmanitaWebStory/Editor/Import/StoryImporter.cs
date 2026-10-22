using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEditor;

namespace AmanitaWebStory.Editor
{
    public static class StoryImporter
    {
        private static readonly Regex IdPattern = new Regex("^[a-zA-Z][a-zA-Z0-9_]{0,63}$");

        /// <summary>
        /// 网页回填 校验版本后把剧情写成 StoryAsset 已有资产原地更新以保留 GUID
        /// </summary>
        public static object Import(string Json, string ExpectedRevision)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                throw new InvalidOperationException("请退出 Play 后回填剧情");
            var Document = StoryValidator.Parse(Json, out var Warnings);
            string Path = AssetPath(Document.id);
            var Existing = AssetDatabase.LoadMainAssetAtPath(Path);
            if (Existing != null)
            {
                if (!(Existing is StoryAsset Previous) || Previous.Id != Document.id || string.IsNullOrEmpty(Previous.SourceJson))
                    throw new InvalidOperationException("目标路径已被非网页剧情资产占用 " + Path);
                if (Revision(Previous.SourceJson) != ExpectedRevision)
                    throw new InvalidOperationException("Unity 里已有更新版本 请先读取 Unity 版本后再编辑回填 或更换剧情标识另存");
            }
            else if (!string.IsNullOrEmpty(ExpectedRevision))
                throw new InvalidOperationException("原剧情资产已被移除 请重新读取 Unity 目录");
            string Canonical = JsonConvert.SerializeObject(Document, Formatting.Indented);
            StoryAssetExporter.Export(Canonical, Path);
            return new { assetPath = Path, revision = Revision(Canonical), nodes = Document.nodes.Count, warnings = Warnings };
        }

        public static object Read(string Id)
        {
            string Path = AssetPath(Id);
            var Story = Load(Id);
            return new { document = JsonConvert.DeserializeObject<StoryDocument>(Story.SourceJson), revision = Revision(Story.SourceJson), assetPath = Path };
        }

        public static StoryAsset Load(string Id)
        {
            var Story = AssetDatabase.LoadAssetAtPath<StoryAsset>(AssetPath(Id));
            if (Story == null || Story.Id != Id || string.IsNullOrEmpty(Story.SourceJson))
                throw new FileNotFoundException("没有找到网页剧情 " + Id);
            return Story;
        }

        public static string AssetPath(string Id)
        {
            if (Id == null || !IdPattern.IsMatch(Id))
                throw new ArgumentException("剧情标识格式错误");
            return StoryAssetExporter.ExportRoot + "/" + Id + ".asset";
        }

        public static string Revision(string Json)
        {
            using (var Hash = SHA256.Create())
                return BitConverter.ToString(Hash.ComputeHash(Encoding.UTF8.GetBytes(Json))).Replace("-", "").ToLowerInvariant();
        }
    }
}

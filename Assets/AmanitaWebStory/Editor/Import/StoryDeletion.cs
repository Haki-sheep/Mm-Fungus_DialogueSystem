using System;
using System.IO;
using UnityEditor;

namespace AmanitaWebStory.Editor
{
    public static class StoryDeletion
    {
        private const string TrashRoot = StoryAssetExporter.Root + "/Trash";

        /// <summary>
        /// 剧情资产移入 Trash 子目录而非直接删除 便于误删后手动恢复
        /// </summary>
        public static object Delete(string Id, string ExpectedRevision)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                throw new InvalidOperationException("请退出 Play 后删除剧情");
            var Story = StoryImporter.Load(Id);
            string Path = StoryImporter.AssetPath(Id);
            if (StoryImporter.Revision(Story.SourceJson) != ExpectedRevision)
                throw new InvalidOperationException("Unity 剧情版本已变化 请刷新后重新选择删除");
            if (!AssetDatabase.IsValidFolder(TrashRoot) && string.IsNullOrEmpty(AssetDatabase.CreateFolder(StoryAssetExporter.Root, "Trash")))
                throw new IOException("无法创建剧情回收目录");
            string FolderName = Id + "_" + Guid.NewGuid().ToString("N");
            if (string.IsNullOrEmpty(AssetDatabase.CreateFolder(TrashRoot, FolderName)))
                throw new IOException("无法创建剧情回收位置 " + FolderName);
            string Destination = TrashRoot + "/" + FolderName;
            string Error = AssetDatabase.MoveAsset(Path, Destination + "/" + System.IO.Path.GetFileName(Path));
            if (!string.IsNullOrEmpty(Error))
            {
                AssetDatabase.DeleteAsset(Destination);
                throw new IOException("删除剧情时无法移动资产 " + Path + " " + Error);
            }
            return new { deleted = true, id = Id, trashPath = Destination };
        }
    }
}

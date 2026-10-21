using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AmanitaWebStory.Editor
{
    public static class StoryDeletion
    {
        private const string TrashRoot = StoryTemplateBuilder.Root + "/Trash";

        public static object Delete(string Id, string ExpectedRevision)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                throw new InvalidOperationException("请退出 Play 后删除剧情");
            string PrefabPath = StoryImporter.AssetPath(Id);
            var Root = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            if (Root == null)
                throw new FileNotFoundException("剧情资产不存在 " + Id);
            var Host = Root.GetComponent<WebStoryHost>();
            if (Host == null || Host.StoryId != Id || string.IsNullOrEmpty(Host.SourceJson))
                throw new InvalidOperationException("目标不是本工具生成的剧情 " + PrefabPath);
            if (StoryImporter.Revision(Host.SourceJson) != ExpectedRevision)
                throw new InvalidOperationException("Unity 剧情版本已变化 请刷新后重新选择删除");
            var PathList = new List<string> { PrefabPath };
            string EventPath = StoryTemplateBuilder.GeneratedRoot + "/" + Id + ".events.asset";
            var EventAsset = AssetDatabase.LoadMainAssetAtPath(EventPath);
            if (EventAsset != null)
            {
                if (!(EventAsset is StorySignalCatalog Catalog) || Catalog.StoryId != Id)
                    throw new InvalidOperationException("事件 SO 不属于此剧情 " + EventPath);
                PathList.Add(EventPath);
            }
            if (!AssetDatabase.IsValidFolder(TrashRoot) && string.IsNullOrEmpty(AssetDatabase.CreateFolder(StoryTemplateBuilder.Root, "Trash")))
                throw new IOException("无法创建剧情回收目录");
            string FolderName = Id + "_" + Guid.NewGuid().ToString("N");
            if (string.IsNullOrEmpty(AssetDatabase.CreateFolder(TrashRoot, FolderName)))
                throw new IOException("无法创建剧情回收位置 " + FolderName);
            string Destination = TrashRoot + "/" + FolderName;
            var MovedList = new List<KeyValuePair<string, string>>();
            try
            {
                foreach (string Path in PathList)
                {
                    string Target = Destination + "/" + System.IO.Path.GetFileName(Path);
                    string Error = AssetDatabase.MoveAsset(Path, Target);
                    if (!string.IsNullOrEmpty(Error))
                        throw new IOException("删除剧情时无法移动资产 " + Path + " " + Error);
                    MovedList.Add(new KeyValuePair<string, string>(Path, Target));
                }
            }
            catch (Exception Error)
            {
                var FailureList = new List<Exception> { Error };
                for (int Index = MovedList.Count - 1; Index >= 0; Index--)
                {
                    string RollbackError = AssetDatabase.MoveAsset(MovedList[Index].Value, MovedList[Index].Key);
                    if (!string.IsNullOrEmpty(RollbackError))
                        FailureList.Add(new IOException("恢复剧情资产失败 " + MovedList[Index].Key + " " + RollbackError));
                }
                if (FailureList.Count > 1)
                    throw new AggregateException("删除剧情失败 且回滚未完成", FailureList);
                AssetDatabase.DeleteAsset(Destination);
                throw;
            }
            return new { deleted = true, id = Id, trashPath = Destination };
        }
    }
}

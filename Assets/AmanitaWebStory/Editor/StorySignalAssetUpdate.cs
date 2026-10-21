using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AmanitaWebStory.Editor
{
    public sealed class StorySignalAssetUpdate
    {
        private readonly string path;
        private readonly StoryDocument document;
        private readonly StorySignalCatalog existing;
        private readonly string backup;
        private bool applied;

        public StorySignalAssetUpdate(StoryDocument Document)
        {
            document = Document;
            path = StoryTemplateBuilder.GeneratedRoot + "/" + Document.id + ".events.asset";
            var Occupant = AssetDatabase.LoadMainAssetAtPath(path);
            existing = Occupant as StorySignalCatalog;
            if (Occupant != null && (existing == null || existing.StoryId != Document.id))
                throw new InvalidOperationException("事件 SO 路径已被其他资产占用 " + path);
            if (File.Exists(path) && Occupant == null)
                throw new InvalidOperationException("事件 SO 文件尚未正确导入 " + path);
            if (existing != null)
                backup = EditorJsonUtility.ToJson(existing);
        }

        public StorySignalCatalog Apply()
        {
            var DefinitionList = document.nodes.Where(Node => Node.type == "event" && Node.eventMode == "signal")
                .GroupBy(Node => Node.eventId).Select(Group => Group.First())
                .Select(Node => new StorySignalDefinition
                {
                    Id = Node.eventId, Title = Node.eventTitle,
                    Kind = (eStoryValueKind)Enum.Parse(typeof(eStoryValueKind), Node.eventValueKind)
                }).ToList();
            if (existing == null && DefinitionList.Count == 0)
                return null;
            if (existing != null)
                foreach (var Definition in DefinitionList)
                {
                    var Previous = existing.DefinitionList.Find(Item => Item.Id == Definition.Id);
                    if (Previous != null && Previous.Kind != Definition.Kind)
                        throw new InvalidOperationException("已回填事件的参数类型不可更改 请使用新的事件标识 " + Definition.Id);
                }
            var Catalog = existing != null ? existing : ScriptableObject.CreateInstance<StorySignalCatalog>();
            applied = true;
            Catalog.StoryId = document.id;
            Catalog.DefinitionList = DefinitionList;
            if (existing == null)
                AssetDatabase.CreateAsset(Catalog, path);
            else
            {
                EditorUtility.SetDirty(Catalog);
                AssetDatabase.SaveAssetIfDirty(Catalog);
            }
            return Catalog;
        }

        public void Rollback()
        {
            if (!applied)
                return;
            if (existing != null)
            {
                EditorJsonUtility.FromJsonOverwrite(backup, existing);
                EditorUtility.SetDirty(existing);
                AssetDatabase.SaveAssetIfDirty(existing);
            }
            else if (!AssetDatabase.DeleteAsset(path) && File.Exists(path))
                throw new IOException("未能撤回新建事件 SO " + path);
        }
    }
}

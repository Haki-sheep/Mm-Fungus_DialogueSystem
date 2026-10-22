using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AmanitaWebStory.Editor
{
    public static class StoryCatalog
    {
        public const int MaxOptions = 6;

        /// <summary>
        /// 收集 AmanitaWebStory 目录下的角色 角色标识重复或为空时直接报错
        /// </summary>
        public static Dictionary<string, StoryActor> Actors()
        {
            var ActorDict = new Dictionary<string, StoryActor>();
            foreach (string Guid in AssetDatabase.FindAssets("t:StoryActor", new[] { StoryAssetExporter.Root }))
            {
                string Path = AssetDatabase.GUIDToAssetPath(Guid);
                var Actor = AssetDatabase.LoadAssetAtPath<StoryActor>(Path);
                if (string.IsNullOrEmpty(Actor.Id))
                    throw new InvalidOperationException("角色未填写标识 " + Path);
                if (ActorDict.TryGetValue(Actor.Id, out var Previous))
                    throw new InvalidOperationException($"角色标识 {Actor.Id} 重复 {AssetDatabase.GetAssetPath(Previous)} 与 {Path}");
                if (Actor.Portraits.Any(Sprite => Sprite == null) || Actor.Portraits.GroupBy(Sprite => Sprite.name).Any(Group => Group.Count() > 1))
                    throw new InvalidOperationException($"角色 {Actor.Id} 的立绘为空或名称重复 {Path}");
                ActorDict.Add(Actor.Id, Actor);
            }
            return ActorDict;
        }

        public static IEnumerable<StoryAsset> Stories()
        {
            if (!Directory.Exists(StoryAssetExporter.ExportRoot))
                return Array.Empty<StoryAsset>();
            return AssetDatabase.FindAssets("t:StoryAsset", new[] { StoryAssetExporter.ExportRoot })
                .Select(Guid => AssetDatabase.LoadAssetAtPath<StoryAsset>(AssetDatabase.GUIDToAssetPath(Guid)))
                .Where(Story => !string.IsNullOrEmpty(Story.SourceJson));
        }

        public static object Read()
        {
            return new
            {
                version = 1,
                project = Application.productName,
                maxOptions = MaxOptions,
                actors = Actors().Values.Select(Actor => new
                {
                    id = Actor.Id,
                    title = Actor.DisplayName,
                    portraits = Actor.Portraits.Select(Portrait => new { id = Portrait.name, title = Portrait.name }).ToArray()
                }).ToArray(),
                audio = AssetDatabase.FindAssets("t:AudioClip", new[] { "Assets" }).Select(Guid => new
                {
                    id = Guid,
                    title = Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(Guid)),
                    path = AssetDatabase.GUIDToAssetPath(Guid)
                }).OrderBy(Clip => Clip.title).ToArray(),
                stories = Stories().Select(Story => new
                {
                    id = Story.Id,
                    title = Story.Title,
                    revision = StoryImporter.Revision(Story.SourceJson)
                }).ToArray()
            };
        }
    }
}

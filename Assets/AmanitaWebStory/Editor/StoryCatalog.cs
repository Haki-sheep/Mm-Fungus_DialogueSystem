using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fungus;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace AmanitaWebStory.Editor
{
    public static class StoryCatalog
    {
        public static WebStoryHost Template => AssetDatabase.LoadAssetAtPath<GameObject>(StoryTemplateBuilder.TemplatePath).GetComponent<WebStoryHost>();

        public static object Read()
        {
            var Host = Template;
            var ActorList = Host.GetComponentsInChildren<Character>(true);
            if (ActorList.Select(Actor => Actor.name).Distinct().Count() != ActorList.Length)
                throw new InvalidOperationException("模板里的角色对象名称必须唯一");
            if (Host.EventList.Select(Event => Event.Id).Distinct().Count() != Host.EventList.Count)
                throw new InvalidOperationException("模板里登记了重复事件标识");
            return new
            {
                version = 1,
                project = Application.productName,
                templatePath = StoryTemplateBuilder.TemplatePath,
                maxOptions = Host.MenuDialog.GetComponentsInChildren<UnityEngine.UI.Button>(true).Length,
                actors = ActorList.Select(Actor => new
                {
                    id = Actor.name, title = Actor.NameText,
                    portraits = Actor.Portraits.Select(Portrait => new { id = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(Portrait)), title = Portrait.name }).ToArray()
                }).ToArray(),
                events = Host.EventList.Select(Event => new
                {
                    id = Event.Id, title = Event.Title, help = Event.Help,
                    type = Event.Kind.ToString(), label = Event.ParameterLabel, parameterHelp = Event.ParameterHelp,
                    defaultValue = Event.DefaultValue, ready = HasUsableBinding(Event)
                }).ToArray(),
                audio = AssetDatabase.FindAssets("t:AudioClip", new[] { "Assets" }).Select(Guid => new
                {
                    id = Guid, title = Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(Guid)),
                    path = AssetDatabase.GUIDToAssetPath(Guid)
                }).OrderBy(Clip => Clip.title).ToArray(),
                stories = Directory.Exists(StoryTemplateBuilder.GeneratedRoot)
                    ? AssetDatabase.FindAssets("t:Prefab", new[] { StoryTemplateBuilder.GeneratedRoot })
                        .Select(Guid => AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(Guid)).GetComponent<WebStoryHost>())
                        .Where(Story => Story != null && !string.IsNullOrEmpty(Story.SourceJson))
                        .Select(Story => new { id = Story.StoryId, title = JObject.Parse(Story.SourceJson)["title"].Value<string>(), revision = StoryImporter.Revision(Story.SourceJson) }).ToArray()
                    : Array.Empty<object>()
            };
        }

        public static bool HasUsableBinding(StoryEventBinding Binding)
        {
            UnityEngine.Events.UnityEventBase Event = Binding.Kind == eStoryValueKind.None ? Binding.OnInvoke :
                Binding.Kind == eStoryValueKind.String ? Binding.OnString : Binding.Kind == eStoryValueKind.Integer ? Binding.OnInteger :
                Binding.Kind == eStoryValueKind.Float ? Binding.OnFloat : (UnityEngine.Events.UnityEventBase)Binding.OnBoolean;
            for (int Index = 0; Index < Event.GetPersistentEventCount(); Index++)
                if (Event.GetPersistentTarget(Index) != null && !string.IsNullOrEmpty(Event.GetPersistentMethodName(Index)) &&
                    Event.GetPersistentListenerState(Index) != UnityEngine.Events.UnityEventCallState.Off)
                    return true;
            return false;
        }
    }
}

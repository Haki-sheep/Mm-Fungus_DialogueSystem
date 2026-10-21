using System;
using System.Collections.Generic;
using Fungus.Inspector;
using UnityEngine;

namespace AmanitaWebStory
{
    [Serializable]
    public sealed class StorySignalDefinition
    {
        [LabelText("事件标识")]
        public string Id;
        [LabelText("事件名称")]
        public string Title;
        [LabelText("参数类型")]
        public eStoryValueKind Kind;
    }

    public sealed class StorySignalCatalog : ScriptableObject
    {
        [LabelText("所属剧情")]
        public string StoryId;
        [LabelText("剧情事件定义")]
        public List<StorySignalDefinition> DefinitionList = new List<StorySignalDefinition>();

        public event Action<WebStoryHost, string, string> Raised;

        public void Raise(WebStoryHost Sender, string Id, string Value)
        {
            if (!DefinitionList.Exists(Definition => Definition.Id == Id))
                throw new InvalidOperationException($"剧情 {StoryId} 没有定义事件 {Id}");
            Raised?.Invoke(Sender, Id, Value);
        }
    }
}

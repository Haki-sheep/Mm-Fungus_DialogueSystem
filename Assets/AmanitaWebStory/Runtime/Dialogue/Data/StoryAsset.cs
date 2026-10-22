using System;
using System.Collections.Generic;
using UnityEngine;

namespace AmanitaWebStory
{
    [Serializable]
    public sealed class StoryInitialVariable
    {
        [field: SerializeField]
        public string Key { get; private set; }
        [field: SerializeField]
        public StoryValue Value { get; private set; }

        public StoryInitialVariable(string Key, StoryValue Value) { this.Key = Key; this.Value = Value; }
    }

    public sealed class StoryAsset : ScriptableObject
    {
        [SerializeField]
        private string storyId;
        [SerializeField]
        private string title;
        [SerializeField]
        private string entry;
        [SerializeField, HideInInspector]
        private string sourceJson;
        [SerializeField, Tooltip("剧情内容资源 由界面决定显示尺寸与布局")]
        private Sprite background;
        [SerializeField]
        private List<StoryInitialVariable> variableList = new List<StoryInitialVariable>();
        [SerializeReference]
        private List<StoryStep> stepList = new List<StoryStep>();

        public string Id => storyId;
        public string Title => title;
        public string Entry => entry;
        public string SourceJson => sourceJson;
        public Sprite Background => background;
        public IReadOnlyList<StoryInitialVariable> Variables => variableList.AsReadOnly();
        public IReadOnlyList<StoryStep> Steps => stepList.AsReadOnly();

#if UNITY_EDITOR
        public void SetContent(string Id, string Title, string Entry, string SourceJson,
            List<StoryInitialVariable> VariableList, List<StoryStep> StepList)
        {
            storyId = Id; title = Title; entry = Entry; sourceJson = SourceJson;
            variableList = new List<StoryInitialVariable>(VariableList);
            stepList = new List<StoryStep>(StepList);
        }
#endif
    }
}

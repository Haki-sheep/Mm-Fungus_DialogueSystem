using System;
using System.Collections.Generic;
using System.Globalization;
using Fungus;
using Fungus.Inspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AmanitaWebStory
{
    public sealed class WebStoryHost : MonoBehaviour
    {
        [LabelText("剧情流程图")]
        public Flowchart Flowchart;
        [LabelText("开始节点")]
        public Block Entry;
        [LabelText("进入场景自动播放")]
        public bool AutoPlay = true;
        [LabelText("对白界面")]
        public SayDialog SayDialog;
        [LabelText("选项界面")]
        public MenuDialog MenuDialog;
        [LabelText("输入系统模板")]
        public GameObject EventSystemPrefab;
        [LabelText("提供给策划的事件")]
        public List<StoryEventBinding> EventList = new List<StoryEventBinding>();
        [LabelText("网页生成的剧情事件 SO")]
        public StorySignalCatalog SignalCatalog;
        [HideInInspector]
        public string SourceJson;
        [HideInInspector]
        public string StoryId;

        private readonly Dictionary<string, StoryEventBinding> eventDict = new Dictionary<string, StoryEventBinding>();
        private readonly List<Action> resetVariableList = new List<Action>();
        private bool initialized;

        private void Start()
        {
            InitComponents();
            if (AutoPlay)
                Play();
        }

        private void InitComponents()
        {
            foreach (var Binding in EventList)
                eventDict.Add(Binding.Id, Binding);
            foreach (var Variable in Flowchart.Variables)
            {
                if (Variable is IntegerVariable Integer)
                {
                    int Initial = Integer.Value;
                    resetVariableList.Add(() => Integer.Value = Initial);
                }
                else if (Variable is FloatVariable Float)
                {
                    float Initial = Float.Value;
                    resetVariableList.Add(() => Float.Value = Initial);
                }
                else if (Variable is BooleanVariable Boolean)
                {
                    bool Initial = Boolean.Value;
                    resetVariableList.Add(() => Boolean.Value = Initial);
                }
                else if (Variable is StringVariable String)
                {
                    string Initial = String.Value;
                    resetVariableList.Add(() => String.Value = Initial);
                }
            }
            if (EventSystem.current == null)
                Instantiate(EventSystemPrefab, transform);
            SayDialog.gameObject.SetActive(true);
            MenuDialog.gameObject.SetActive(true);
            SayDialog.Clear();
            MenuDialog.Clear();
            SayDialog.gameObject.SetActive(false);
            MenuDialog.gameObject.SetActive(false);
            initialized = true;
        }

        public void Play()
        {
            if (!initialized)
                throw new InvalidOperationException("网页剧情宿主需要在 Start 完成初始化后才能播放");
            Flowchart.StopAllBlocks();
            Flowchart.ResetFlowchart(true, false);
            foreach (var Reset in resetVariableList)
                Reset();
            SayDialog.Clear();
            MenuDialog.Clear();
            Entry.StartExecution();
        }

        public void Raise(string Id, string Value)
        {
            var Binding = eventDict[Id];
            if (Binding.ListenerCount == 0)
                throw new InvalidOperationException($"剧情事件 {Id} 尚未绑定游戏逻辑");
            switch (Binding.Kind)
            {
                case eStoryValueKind.None: Binding.OnInvoke.Invoke(); break;
                case eStoryValueKind.String: Binding.OnString.Invoke(Value); break;
                case eStoryValueKind.Integer: Binding.OnInteger.Invoke(int.Parse(Value, CultureInfo.InvariantCulture)); break;
                case eStoryValueKind.Float: Binding.OnFloat.Invoke(float.Parse(Value, CultureInfo.InvariantCulture)); break;
                case eStoryValueKind.Boolean: Binding.OnBoolean.Invoke(bool.Parse(Value)); break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public void LogMarker(string Value)
        {
            Debug.Log($"[网页剧情 {StoryId}] {Value}", this);
        }
    }
}

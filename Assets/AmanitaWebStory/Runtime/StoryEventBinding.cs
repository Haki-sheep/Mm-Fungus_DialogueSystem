using System;
using Fungus.Inspector;
using UnityEngine;
using UnityEngine.Events;

namespace AmanitaWebStory
{
    public enum eStoryValueKind
    {
        [InspectorName("不需要参数")] None,
        [InspectorName("文字")] String,
        [InspectorName("整数")] Integer,
        [InspectorName("小数")] Float,
        [InspectorName("开关")] Boolean
    }

    [Serializable]
    public sealed class StoryEventBinding
    {
        [LabelText("事件标识")]
        public string Id;
        [LabelText("策划看到的名称")]
        public string Title;
        [LabelText("什么时候使用"), TextArea]
        public string Help;
        [LabelText("参数类型")]
        public eStoryValueKind Kind;
        [LabelText("参数名称")]
        public string ParameterLabel;
        [LabelText("参数填写说明"), TextArea]
        public string ParameterHelp;
        [LabelText("参数默认值")]
        public string DefaultValue;
        [LabelText("无参数回调")]
        public UnityEvent OnInvoke = new UnityEvent();
        [LabelText("文字回调")]
        public UnityEvent<string> OnString = new UnityEvent<string>();
        [LabelText("整数回调")]
        public UnityEvent<int> OnInteger = new UnityEvent<int>();
        [LabelText("小数回调")]
        public UnityEvent<float> OnFloat = new UnityEvent<float>();
        [LabelText("开关回调")]
        public UnityEvent<bool> OnBoolean = new UnityEvent<bool>();

        public int ListenerCount => Kind == eStoryValueKind.None ? OnInvoke.GetPersistentEventCount() :
            Kind == eStoryValueKind.String ? OnString.GetPersistentEventCount() :
            Kind == eStoryValueKind.Integer ? OnInteger.GetPersistentEventCount() :
            Kind == eStoryValueKind.Float ? OnFloat.GetPersistentEventCount() : OnBoolean.GetPersistentEventCount();
    }
}

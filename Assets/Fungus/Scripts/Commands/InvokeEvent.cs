using Cysharp.Threading.Tasks;
using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System;
using UnityEngine.Events;

namespace Fungus
{
    /// <summary>
    /// Supported types of method invocation.
    /// </summary>
    public enum InvokeType
    {
        /// <summary> Call a method with an optional constant value parameter. </summary>
        [UnityEngine.InspectorName("固定值事件")]
        Static,         // 
        /// <summary> Call a method with an optional boolean constant / variable parameter. </summary>
        [UnityEngine.InspectorName("传入布尔值")]
        DynamicBoolean,
        /// <summary> Call a method with an optional integer constant / variable parameter. </summary>
        [UnityEngine.InspectorName("传入整数")]
        DynamicInteger,
        /// <summary> Call a method with an optional float constant / variable parameter. </summary>
        [UnityEngine.InspectorName("传入小数")]
        DynamicFloat,
        /// <summary> Call a method with an optional string constant / variable parameter. </summary>
        [UnityEngine.InspectorName("传入文字")]
        DynamicString
    }

    /// <summary>
    /// Calls a list of component methods via the Unity Event System (as used in the Unity UI)
    /// This command is more efficient than the Invoke Method command but can only pass a single parameter and doesn't support return values.
    /// This command uses the UnityEvent system to call methods in script. http://docs.unity3d.com/Manual/UnityEvents.html
    /// </summary>
    [CommandInfo("脚本与事件", "触发事件", "触发配置的 Unity 事件 用来连接游戏业务" + 
                 "This command is more efficient than the Invoke Method command but can only pass a single parameter and doesn't support return values.")]
    [AddComponentMenu("")]
    public class InvokeEvent : Command
    {
        [Tooltip("填写这段流程或对白的用途 只用于编辑时说明")]
        [LabelText("用途说明")]
        [SerializeField]
        protected string description = "";

        [Tooltip("设置本步骤使用的开始前等待（秒）")]
        [LabelText("开始前等待（秒）")]
        [SerializeField]
        protected float delay;

        [Tooltip("设置本步骤使用的调用类型")]
        [LabelText("调用类型")]
        [SerializeField]
        protected InvokeType invokeType;

        [Tooltip("设置本步骤使用的无参数事件")]
        [LabelText("无参数事件")]
        [SerializeField]
        protected UnityEvent staticEvent = new UnityEvent();

        [Tooltip("设置本步骤使用的布尔参数")]
        [LabelText("布尔参数")]
        [SerializeField]
        protected BooleanData booleanParameter;

        [Tooltip("设置本步骤使用的布尔值事件")]
        [LabelText("布尔值事件")]
        [SerializeField]
        protected BooleanEvent booleanEvent = new BooleanEvent();

        [Tooltip("设置本步骤使用的整数参数")]
        [LabelText("整数参数")]
        [SerializeField]
        protected IntegerData integerParameter;
        
        [Tooltip("设置本步骤使用的整数事件")]
        [LabelText("整数事件")]
        [SerializeField]
        protected IntegerEvent integerEvent = new IntegerEvent();

        [Tooltip("设置本步骤使用的小数参数")]
        [LabelText("小数参数")]
        [SerializeField]
        protected FloatData floatParameter;
        
        [Tooltip("设置本步骤使用的小数事件")]
        [LabelText("小数事件")]
        [SerializeField]
        protected FloatEvent floatEvent = new FloatEvent();

        [Tooltip("设置本步骤使用的文字参数")]
        [LabelText("文字参数")]
        [SerializeField]
        protected StringDataMulti stringParameter;

        [Tooltip("设置本步骤使用的文字事件")]
        [LabelText("文字事件")]
        [SerializeField]
        protected StringEvent stringEvent = new StringEvent();

        protected virtual void DoInvoke()
        {
            switch (invokeType)
            {
                default:
                case InvokeType.Static:
                    staticEvent.Invoke();
                    break;
                case InvokeType.DynamicBoolean:
                    booleanEvent.Invoke(booleanParameter.Value);
                    break;
                case InvokeType.DynamicInteger:
                    integerEvent.Invoke(integerParameter.Value);
                    break;
                case InvokeType.DynamicFloat:
                    floatEvent.Invoke(floatParameter.Value);
                    break;
                case InvokeType.DynamicString:
                    stringEvent.Invoke(stringParameter.Value);
                    break;
            }
        }

        #region Public members

        [Serializable] public class BooleanEvent : UnityEvent<bool> {}
        [Serializable] public class IntegerEvent : UnityEvent<int> {}
        [Serializable] public class FloatEvent : UnityEvent<float> {}
        [Serializable] public class StringEvent : UnityEvent<string> {}

        public override void OnEnter()
        {
            if (Mathf.Approximately(delay, 0f))
            {
                DoInvoke();
            }
            else
            {
                asyncScope.RunAfterDelay(this, delay, DoInvoke).Forget();
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (!string.IsNullOrEmpty(description))
            {
                return description;
            }

            string summary = AuthoringValueLabel.Format(invokeType) + " ";

            switch (invokeType)
            {
            default:
            case InvokeType.Static:
                summary += staticEvent.GetPersistentEventCount();
                break;
            case InvokeType.DynamicBoolean:
                summary += booleanEvent.GetPersistentEventCount();
                break;
            case InvokeType.DynamicInteger:
                summary += integerEvent.GetPersistentEventCount();
                break;
            case InvokeType.DynamicFloat:
                summary += floatEvent.GetPersistentEventCount();
                break;
            case InvokeType.DynamicString:
                summary += stringEvent.GetPersistentEventCount();
                break;
            }

            return summary + " 个方法";
        }
        
        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return booleanParameter.booleanRef == variable || integerParameter.integerRef == variable ||
                floatParameter.floatRef == variable || stringParameter.stringRef == variable ||
                base.HasReference(variable);
        }

        #endregion
    }
}
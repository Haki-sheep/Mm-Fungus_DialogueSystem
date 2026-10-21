using Cysharp.Threading.Tasks;
using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Calls a named method on a GameObject using the GameObject.SendMessage() system.
    /// This command is called "Call Method" because a) it's more descriptive than Send Message and we're already have
    /// a Send Message command for sending messages to trigger block execution.
    /// </summary>
    [CommandInfo("脚本与事件", "发送方法调用", "向目标对象发送方法调用")]
    [AddComponentMenu("")]
    public class CallMethod : Command
    {
        [Tooltip("设置本步骤使用的目标对象")]
        [LabelText("目标对象")]
        [SerializeField]
        protected GameObject targetObject;

        [Tooltip("设置本步骤使用的方法名称")]
        [LabelText("方法名称")]
        [SerializeField]
        protected string methodName = "";

        [Tooltip("设置本步骤使用的开始前等待（秒）")]
        [LabelText("开始前等待（秒）")]
        [SerializeField]
        protected float delay;

        protected virtual void CallTheMethod()
        {
            targetObject.SendMessage(methodName, SendMessageOptions.DontRequireReceiver);
        }

        #region Public members

        public override void OnEnter()
        {
            if (targetObject == null ||
                methodName.Length == 0)
            {
                Continue();
                return;
            }

            if (Mathf.Approximately(delay, 0f))
            {
                CallTheMethod();
            }
            else
            {
                asyncScope.RunAfterDelay(this, delay, CallTheMethod).Forget();
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (targetObject == null)
            {
                return "请先设置目标场景对象";
            }

            if (methodName.Length == 0)
            {
                return "请先设置方法名称";
            }

            return targetObject.name + " : " + methodName;
        }
        
        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        #endregion
    }
}
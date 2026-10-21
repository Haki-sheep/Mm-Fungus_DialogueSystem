using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    [System.Serializable]
    public class FungusException : System.Exception
    {
        public FungusException()
        {
        }

        public FungusException(string message) : base(message)
        {
        }

        public FungusException(string message, System.Exception inner) : base(message, inner)
        {
        }

        protected FungusException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// Throw a Fungus.Exception
    /// </summary>
    [CommandInfo("脚本与事件", "抛出错误", "主动抛出错误 用于验证错误处理或中断异常流程")]
    [AddComponentMenu("")]
    public class ThrowException : Command
    {
        [LabelText("消息内容")]
        [SerializeField]
        protected StringData message;

        public override void OnEnter()
        {
            throw new FungusException(GetLocationIdentifier() + " " + message.Value);

#pragma warning disable CS0162 // Unreachable code detected
            Continue();
#pragma warning restore CS0162 // Unreachable code detected
        }

        public override string GetSummary()
        {
            return message.Value;
        }

        public override bool HasReference(Variable variable)
        {
            return variable == message.stringRef || base.HasReference(variable);
        }
    }
}
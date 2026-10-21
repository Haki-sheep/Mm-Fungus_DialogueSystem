using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;

namespace Fungus
{
    /// <summary>
    /// Supported target types for messages.
    /// </summary>
    public enum MessageTarget
    {
        /// <summary>
        /// Send message to the Flowchart containing the SendMessage command.
        /// </summary>
        [UnityEngine.InspectorName("当前流程图")]
        SameFlowchart,
        /// <summary>
        /// Broadcast message to all Flowcharts.
        /// </summary>
        [UnityEngine.InspectorName("所有流程图")]
        AllFlowcharts
    }

    /// <summary>
    /// Sends a message to either the owner Flowchart or all Flowcharts in the scene. Blocks can listen for this message using a Message Received event handler.
    /// </summary>
    [CommandInfo("流程控制", "发送剧情消息", "发送剧情消息 让监听该消息的流程开始执行")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class SendMessage : Command
    {
        [Tooltip("设置本步骤使用的消息发送范围")]
        [LabelText("消息发送范围")]
        [SerializeField]
        protected MessageTarget messageTarget;

        [Tooltip("设置本步骤使用的消息内容")]
        [LabelText("消息内容")]
        [SerializeField]
        protected StringData _message = new StringData("");

        #region Public members

        public override void OnEnter()
        {
            if (_message.Value.Length == 0)
            {
                Continue();
                return;
            }

            MessageReceived[] receivers = null;
            if (messageTarget == MessageTarget.SameFlowchart)
            {
                receivers = GetComponents<MessageReceived>();
            }
            else
            {
            #if UNITY_6000
                receivers = GameObject.FindObjectsByType<MessageReceived>(FindObjectsSortMode.None);
            #else
                receivers = GameObject.FindObjectsOfType<MessageReceived>();
            #endif
            }

            if (receivers != null)
            {
                for (int i = 0; i < receivers.Length; i++)
                {
                    var receiver = receivers[i];
                    receiver.OnSendFungusMessage(_message.Value);
                }
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (_message.Value.Length == 0)
            {
                return "请先设置消息内容";
            }
            
            return _message.Value;
        }
        
        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return _message.stringRef == variable || base.HasReference(variable);
        }

        #endregion

        #region Backwards compatibility

        [HideInInspector] [FormerlySerializedAs("message")] public string messageOLD = "";

        protected virtual void OnEnable()
        {
            if (messageOLD != "")
            {
                _message.Value = messageOLD;
                messageOLD = "";
            }
        }

        #endregion
    }
}
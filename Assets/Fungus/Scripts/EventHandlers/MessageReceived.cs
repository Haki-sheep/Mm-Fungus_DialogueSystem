using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// The block will execute when the specified message is received from a Send Message command.
    /// </summary>
    [EventHandlerInfo("场景", "收到剧情消息", "在收到剧情消息时执行所属流程")]
    [AddComponentMenu("")]
    public class MessageReceived : EventHandler 
    {
        [Tooltip("设置本步骤使用的消息内容")]
        [LabelText("消息内容")]
        [SerializeField]
        protected string message = "";

        #region Public members

        /// <summary>
        /// Called from Flowchart when a message is sent.
        /// </summary>
        /// <param name="message">Message.</param>
        public void OnSendFungusMessage(string message)
        {
            if (this.message == message)
            {
                ExecuteBlock();
            }
        }

        public override string GetSummary()
        {
            return message;
        }

        #endregion
    }
}
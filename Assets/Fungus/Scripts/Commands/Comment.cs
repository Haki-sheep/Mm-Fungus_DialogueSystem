using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Use comments to record design notes and reminders about your game.
    /// </summary>
    [CommandInfo("其他", "备注", "添加创作备注 不会执行游戏行为")]
    [AddComponentMenu("")]
    public class Comment : Command
    {   
        [Tooltip("设置本步骤使用的备注人")]
        [LabelText("备注人")]
        [SerializeField]
        protected string commenterName = "";

        [Tooltip("设置本步骤使用的备注内容")]
        [TextArea(2,4)]
        [LabelText("备注内容")]
        [SerializeField]
        protected string commentText = "";

        #region Public members

        public override void OnEnter()
        {
            Continue();
        }

        public override string GetSummary()
        {
            if (commenterName != "")
            {
                return commenterName + ": " + commentText;
            }
            return commentText;
        }

        public override Color GetButtonColor()
        {
            return new Color32(220, 220, 220, 255);
        }

        #endregion
    }
}
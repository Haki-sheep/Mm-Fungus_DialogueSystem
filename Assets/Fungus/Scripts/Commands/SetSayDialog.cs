using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Sets a custom say dialog to use when displaying story text.
    /// </summary>
    [CommandInfo("剧情对白", "指定对白界面", "指定后续对白使用的界面")]
    [AddComponentMenu("")]
    public class SetSayDialog : Command 
    {
        [Tooltip("设置本步骤使用的对白界面")]
        [LabelText("对白界面")]
        [SerializeField]
        protected SayDialog sayDialog;

        #region Public members

        public override void OnEnter()
        {
            if (sayDialog != null)
            {
                SayDialog.ActiveSayDialog = sayDialog;
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (sayDialog == null)
            {
                return "请先设置对白界面";
            }

            return sayDialog.name;
        }

        public override Color GetButtonColor()
        {
            return new Color32(184, 210, 235, 255);
        }

        #endregion
    }
}
using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Sets a custom menu dialog to use when displaying multiple choice menus.
    /// </summary>
    [CommandInfo("剧情对白", "指定选项界面", "指定后续选项使用的界面")]
    [AddComponentMenu("")]
    public class SetMenuDialog : Command 
    {
        [Tooltip("设置本步骤使用的选项界面")]
        [LabelText("选项界面")]
        [SerializeField]
        protected MenuDialog menuDialog;

        #region Public members

        public override void OnEnter()
        {
            if (menuDialog != null)
            {
                MenuDialog.ActiveMenuDialog = menuDialog;
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (menuDialog == null)
            {
                return "请先设置选项界面";
            }

            return menuDialog.name;
        }

        public override Color GetButtonColor()
        {
            return new Color32(184, 210, 235, 255);
        }

        #endregion
    }
}
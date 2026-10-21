using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Clears the options from a menu dialogue.
    /// </summary>
    [CommandInfo("剧情对白", "清空选项", "移除当前显示的所有选项 并取消倒计时")]
    public class ClearMenu : Command 
    {
        [Tooltip("设置本步骤使用的选项界面")]
        [LabelText("选项界面")]
        [SerializeField]
        protected MenuDialog menuDialog;

        #region Public members

        public override void OnEnter()
        {
            menuDialog.Clear();

            Continue();
        }

        public override string GetSummary()
        {
            if (menuDialog == null)
            {
                return "请先设置选项界面对象";
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
using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using Fungus;

namespace Fungus
{
    /// <summary>
    /// Opens the specified URL in the browser.
    /// </summary>
    [CommandInfo("脚本与事件", "打开网页", "对配置的目标执行打开网页 使用下方参数设置具体行为")]
    public class OpenURL : Command
    {
        [Tooltip("设置本步骤使用的网页地址")]
        [LabelText("网页地址")]
        [SerializeField]
        protected StringData url = new StringData();

        #region Public members

        public override void OnEnter()
        {
            Application.OpenURL(url.Value);

            Continue();
        }

        public override string GetSummary()
        {
            return url.Value;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return url.stringRef == variable || base.HasReference(variable);
        }

        #endregion
    }
}
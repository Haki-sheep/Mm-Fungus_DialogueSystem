using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using UnityEngine.UI;

namespace Fungus
{
    /// <summary>
    /// Sets the state of a toggle UI object.
    /// </summary>
    [CommandInfo("界面", "设置开关状态", "对配置的目标执行设置开关状态 使用下方参数设置具体行为")]
    public class SetToggleState : Command 
    {
        [Tooltip("设置本步骤使用的开关对象")]
        [LabelText("开关对象")]
        [SerializeField]
        protected Toggle toggle;

        [Tooltip("设置本步骤使用的数值")]
        [LabelText("数值")]
        [SerializeField]
        protected BooleanData value;

        #region Public members

        public override void OnEnter() 
        {
            if (toggle != null)
            {
                toggle.isOn = value.Value;
            }

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override string GetSummary()
        {
            if (toggle == null)
            {
                return "请先设置开关对象";
            }

            return toggle.name + " = " + value.GetDescription();
        }

        public override bool HasReference(Variable variable)
        {
            return value.booleanRef == variable || base.HasReference(variable);
        }

        #endregion
    }
}
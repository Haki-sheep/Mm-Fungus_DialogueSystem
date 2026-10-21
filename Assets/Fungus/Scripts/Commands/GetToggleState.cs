using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using UnityEngine.UI;

namespace Fungus
{
    /// <summary>
    /// Gets the state of a toggle UI object and stores it in a boolean variable.
    /// </summary>
    [CommandInfo("界面", "读取开关状态", "对配置的目标执行读取开关状态 使用下方参数设置具体行为")]
    public class GetToggleState : Command 
    {
        [Tooltip("设置本步骤使用的开关对象")]
        [LabelText("开关对象")]
        [SerializeField]
        protected Toggle toggle;

        [Tooltip("设置本步骤使用的开关状态")]
        [VariableProperty(typeof(BooleanVariable))]
        [LabelText("开关状态")]
        [SerializeField]
        protected BooleanVariable toggleState;

        #region Public members

        public override void OnEnter() 
        {
            if (toggle != null &&
                toggleState != null)
            {
                toggleState.Value = toggle.isOn;
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

            if (toggleState == null)
            {
                return "请先设置接收开关状态的变量";
            }

            return toggle.name;
        }

        public override bool HasReference(Variable variable)
        {
            return toggleState == variable || 
                base.HasReference(variable);
        }

        #endregion
    }
}
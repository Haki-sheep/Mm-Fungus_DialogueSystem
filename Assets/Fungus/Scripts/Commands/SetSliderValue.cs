using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using UnityEngine.UI;

namespace Fungus
{
    /// <summary>
    /// Sets or Gets the value property of a slider object.
    /// </summary>
    [CommandInfo("界面", "读写滑动条数值", "对配置的目标执行读写滑动条数值 使用下方参数设置具体行为")]
    public class SetSliderValue : Command 
    {
        [Tooltip("设置本步骤使用的滑动条")]
        [LabelText("滑动条")]
        [SerializeField]
        protected Slider slider;

        [Tooltip("设置本步骤使用的数值")]
        [LabelText("数值")]
        [SerializeField]
        protected FloatData value;

        [LabelText("读取或写入")]
        protected BaseVariableProperty.GetSet getOrSet = BaseVariableProperty.GetSet.Set;

        #region Public members

        public override void OnEnter() 
        {
            if (slider != null)
            {
                switch (getOrSet)
                {
                case BaseVariableProperty.GetSet.Get:
                    value.Value = slider.value;
                    break;
                case BaseVariableProperty.GetSet.Set:
                    slider.value = value.Value;
                    break;
                default:
                    break;
                }
            }

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override string GetSummary()
        {
            if (slider == null)
            {
                return "请先设置滑动条";
            }

            return  getOrSet == BaseVariableProperty.GetSet.Set ? 
                slider.name + " = " + value.GetDescription() :
                value.GetDescription() + " = " + slider.name;
        }

        public override bool HasReference(Variable variable)
        {
            return value.floatRef == variable || base.HasReference(variable);
        }

        #endregion
    }
}
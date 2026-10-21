using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using UnityEngine.UI;

namespace Fungus 
{
    /// <summary>
    /// The block will execute when the user toggles on the target UI toggle object.
    /// </summary>
    [EventHandlerInfo("界面", "开关状态改变", "在开关状态改变时执行所属流程")]
    [AddComponentMenu("")]
    public class ToggleChanged : EventHandler
    {   
        [Tooltip("设置本步骤使用的目标开关")]
        [LabelText("目标开关")]
        [SerializeField]
        protected Toggle targetToggle;

        [Tooltip("设置本步骤使用的开关状态")]
        [VariableProperty(typeof(BooleanVariable))]
        [LabelText("开关状态")]
        [SerializeField]
        protected BooleanVariable toggleState;

        #region Public members

        public virtual void Start()
        {
            if (targetToggle != null)
            {
                targetToggle.onValueChanged.AddListener(OnToggleChanged);
            }
        }
        
        protected virtual void OnToggleChanged(bool state)
        {
            if (toggleState != null)
            {
                toggleState.Value = state;
            }

            ExecuteBlock();
        }

        public override string GetSummary()
        {
            if (targetToggle != null)
            {
                return targetToggle.name;
            }

            return "请先设置目标开关";
        }

        #endregion
    }
}

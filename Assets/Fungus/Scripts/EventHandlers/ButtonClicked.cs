using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using UnityEngine.UI;

namespace Fungus 
{
    /// <summary>
    /// The block will execute when the user clicks on the target UI button object.
    /// </summary>
    [EventHandlerInfo("界面", "按钮被点击", "在按钮被点击时执行所属流程")]
    [AddComponentMenu("")]
    public class ButtonClicked : EventHandler
    {   
        [Tooltip("设置本步骤使用的目标按钮")]
        [LabelText("目标按钮")]
        [SerializeField]
        protected Button targetButton;

        #region Public members

        public virtual void Start()
        {
            if (targetButton != null)
            {
                targetButton.onClick.AddListener(OnButtonClick);
            }
        }
        
        protected virtual void OnButtonClick()
        {
            ExecuteBlock();
        }

        public override string GetSummary()
        {
            if (targetButton != null)
            {
                return targetButton.name;
            }

            return "请先设置目标按钮";
        }

        #endregion
    }
}

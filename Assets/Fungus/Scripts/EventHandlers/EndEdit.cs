using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using UnityEngine.UI;

namespace Fungus 
{
    /// <summary>
    /// The block will execute when the user finishes editing the text in the input field.
    /// </summary>
    [EventHandlerInfo("界面", "输入框结束编辑", "在输入框结束编辑时执行所属流程")]
    [AddComponentMenu("")]
    public class EndEdit : EventHandler
    {   
        [Tooltip("设置本步骤使用的目标输入框")]
        [LabelText("目标输入框")]
        [SerializeField]
        protected InputField targetInputField;
        
        protected virtual void Start()
        {
            targetInputField.onEndEdit.AddListener(OnEndEdit);
        }
        
        protected virtual void OnEndEdit(string text)
        {
            ExecuteBlock();
        }

        #region Public members

        public override string GetSummary()
        {
            if (targetInputField != null)
            {
                return targetInputField.name;
            }

            return "请先设置目标输入框";
        }

        #endregion
    }
}

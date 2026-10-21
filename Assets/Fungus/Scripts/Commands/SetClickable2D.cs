using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Sets a Clickable2D component to be clickable / non-clickable.
    /// </summary>
    [CommandInfo("精灵图片", "设置二维点击交互", "对配置的目标执行设置二维点击交互 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    public class SetClickable2D : Command
    {       
        [Tooltip("设置本步骤使用的目标可点击对象")]
        [LabelText("目标可点击对象")]
        [SerializeField]
        protected Clickable2D targetClickable2D;

        [Tooltip("设置本步骤使用的启用状态")]
        [LabelText("启用状态")]
        [SerializeField]
        protected BooleanData activeState;

        #region Public members

        public override void OnEnter()  
        {
            if (targetClickable2D != null)      
            {
                targetClickable2D.ClickEnabled = activeState.Value; 
            }
            
            Continue();
        }
        
        public override string GetSummary()
        {
            if (targetClickable2D == null)      
            {
                return "请先设置可点击组件";  
            }
            
            return targetClickable2D.gameObject.name;
        }
        
        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return activeState.booleanRef == variable || base.HasReference(variable);
        }

        #endregion
    }
        
}
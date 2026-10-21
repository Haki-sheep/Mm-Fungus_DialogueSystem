using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Plays a state of an animator according to the state name.
    /// </summary>
    [CommandInfo("动画", "播放动画状态", "对配置的目标执行播放动画状态 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    public class PlayAnimState : Command 
    {
        [Tooltip("设置本步骤使用的动画控制器")]
        [LabelText("动画控制器")]
        [SerializeField]
        protected AnimatorData animator = new AnimatorData();

        [Tooltip("设置本步骤使用的动画状态名称")]
        [LabelText("动画状态名称")]
        [SerializeField]
        protected StringData stateName = new StringData();

        [Tooltip("设置本步骤使用的动画层")]
        [LabelText("动画层")]
        [SerializeField]
        protected IntegerData layer = new IntegerData(-1);

        [Tooltip("设置本步骤使用的时间（秒）")]
        [LabelText("时间（秒）")]
        [SerializeField]
        protected FloatData time = new FloatData(0f);

        #region Public members

        public override void OnEnter()
        {
            if (animator.Value != null)
            {
                animator.Value.Play(stateName.Value, layer.Value, time.Value);
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (animator.Value == null)
            {
                return "请先设置动画控制器";
            }

            return animator.Value.name + " (" + stateName.Value + ")";
        }

        public override Color GetButtonColor()
        {
            return new Color32(170, 204, 169, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return animator.animatorRef == variable || stateName.stringRef == variable || 
                layer.integerRef == variable || time.floatRef == variable || 
                base.HasReference(variable);
        }

        #endregion
    }    
}


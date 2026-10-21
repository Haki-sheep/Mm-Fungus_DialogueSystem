using PrimeTween;
using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// Applies a camera shake effect to the main camera.
    /// </summary>
    [CommandInfo("镜头", "震动镜头", "对配置的目标执行震动镜头 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    [System.Obsolete("Deprecated, consider using the LeanTween based Tween command instead.")]
    public class ShakeCamera : Command 
    {
        [Tooltip("设置效果持续多久 单位为秒")]
        [LabelText("持续时间（秒）")]
        [SerializeField]
        protected float duration = 0.5f;
        
        [Tooltip("设置本步骤使用的变化量")]
        [LabelText("变化量")]
        [SerializeField]
        protected Vector2 amount = new Vector2(1, 1);
        
        [Tooltip("勾选后等待本步骤完成再继续 不勾选则立即执行下一步")]
        [LabelText("等完成后再执行下一步")]
        [SerializeField]
        protected bool waitUntilFinished;

        protected virtual void OniTweenComplete(object param)
        {
            Command command = param as Command;
            if (command != null && command.Equals(this))
            {
                if (waitUntilFinished)
                {
                    Continue();
                }
            }
        }

        #region Public members

        public override void OnEnter()
        {
            var Target = Camera.main.transform;
            var Settings = new ShakeSettings(new Vector3(amount.x, amount.y, 0), duration, useUnscaledTime: false);
            var Handle = TrackTween(FungusTween.Track(Target.gameObject, Tween.ShakeCustom(Target, Target.position, Settings, (Object, Value) => Object.position = Value)));
            if (waitUntilFinished) Handle.OnComplete(CreateContinueCallback());
            else Continue();
        }

        public override string GetSummary()
        {
            return "循环" + duration + " 秒";
        }
        
        public override Color GetButtonColor()
        {
            return new Color32(216, 228, 170, 255);
        }

        #endregion
    }    
}
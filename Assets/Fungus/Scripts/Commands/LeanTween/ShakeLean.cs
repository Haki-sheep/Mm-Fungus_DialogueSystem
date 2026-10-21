using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using PrimeTween;

namespace Fungus
{
    /// <summary>
    /// Base class to account for the slight differences between regular leantween command
    /// and ones that use the shake helpers.
    /// </summary>
    [AddComponentMenu("")]
    public abstract class ShakeLean : Command
    {
        [Tooltip("设置本步骤使用的目标对象")]
        [LabelText("目标对象")]
        [SerializeField]
        protected GameObjectData _targetObject;

        [Tooltip("设置效果持续多久 单位为秒")]
        [LabelText("持续时间（秒）")]
        [SerializeField]
        protected FloatData _duration = new FloatData(1f);

        [Tooltip("设置本步骤使用的轴向倍率")]
        [LabelText("轴向倍率")]
        [SerializeField]
        protected Vector3Data _axisScale;

        [Tooltip("设置本步骤使用的速度范围")]
        [LabelText("速度范围")]
        [SerializeField]
        protected Vector2Data _axisSpeedRange = new Vector2Data(new Vector2(5, 10));

        [Tooltip("设置本步骤使用的使用局部坐标")]
        [LabelText("使用局部坐标")]
        [SerializeField]
        protected bool isLocal;

        [Tooltip("设置本步骤使用的缓动曲线")]
        [LabelText("缓动曲线")]
        [SerializeField]
        protected eTweenEase easeType = eTweenEase.linear;

        [Tooltip("开始新补间动画前取消同一对象上已有的补间动画")]
        [LabelText("开始前停止已有动画")]
        [SerializeField]
        protected bool stopPreviousTweens = false;

        [Tooltip("勾选后等待本步骤完成再继续 不勾选则立即执行下一步")]
        [LabelText("等完成后再执行下一步")]
        [SerializeField]
        protected bool waitUntilFinished = true;

        [HideInInspector] protected Tween ourTween;

        protected virtual void OnTweenComplete()
        {
            Continue();
        }

        public override void OnEnter()
        {
            if (_targetObject.Value == null)
            {
                Continue();
                return;
            }

            if (stopPreviousTweens)
            {
                FungusTween.Stop(_targetObject.Value);
            }

            ourTween = TrackTween(ExecuteTween());



            if (!waitUntilFinished)
            {
                Continue();
            }
        }

        public override string GetSummary()
        {
            if (_targetObject.Value == null)
            {
                return "请先设置目标对象";
            }

            return _targetObject.Value.name + " 持续 " + _duration.Value + " 秒";
        }

        public override Color GetButtonColor()
        {
            return new Color32(233, 163, 180, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return variable == _targetObject.gameObjectRef || variable == _duration.floatRef ||
                _axisScale.vector3Ref == variable || _axisSpeedRange.vector2Ref == variable;
        }

        public abstract Tween ExecuteTween();
    }
}

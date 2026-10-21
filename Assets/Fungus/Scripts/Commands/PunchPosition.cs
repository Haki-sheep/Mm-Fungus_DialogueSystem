using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;
using PrimeTween;

namespace Fungus
{
    /// <summary>
    /// Applies a jolt of force to a GameObject's position and wobbles it back to its initial position.
    /// </summary>
    [CommandInfo("旧版补间动画", "位置弹动", "对配置的目标执行位置弹动 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    [System.Obsolete("Deprecated, 底层已迁移到 PrimeTween 建议新剧情使用补间动画分类中的命令")]
    [ExecuteInEditMode]
    public class PunchPosition : iTweenCommand
    {
        [Tooltip("设置本步骤使用的变化量")]
        [LabelText("变化量")]
        [SerializeField]
        protected Vector3Data _amount;

        [Tooltip("设置本步骤使用的使用的坐标系")]
        [LabelText("使用的坐标系")]
        [SerializeField]
        protected Space space = Space.Self;

        #region Public members

        public override Tween DoTween()
        {
            var Target = _targetObject.Value.transform;
            var Settings = CreateTweenSettings();
            var Amount = _amount.Value;
            var ShakeSettings = new ShakeSettings(Amount, _duration.Value, cycles: loopType == eLegacyTweenLoop.None ? 1 : -1, useUnscaledTime: false);
            return space == Space.Self ? Tween.PunchLocalPosition(Target, ShakeSettings) : Tween.PunchCustom(Target, Target.position, ShakeSettings, (Object, Value) => Object.position = Value);
        }

        public override bool HasReference(Variable variable)
        {
            return variable == _amount.vector3Ref;
        }

        #endregion

        #region Backwards compatibility

        [HideInInspector] [FormerlySerializedAs("amount")] public Vector3 amountOLD;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (amountOLD != default(Vector3))
            {
                _amount.Value = amountOLD;
                amountOLD = default(Vector3);
            }
        }

        #endregion
    }
}
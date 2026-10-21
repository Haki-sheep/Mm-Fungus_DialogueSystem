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
    /// Randomly shakes a GameObject's position by a diminishing amount over time.
    /// </summary>
    [CommandInfo("旧版补间动画", "位置抖动", "对配置的目标执行位置抖动 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    [System.Obsolete("Deprecated, 底层已迁移到 PrimeTween 建议新剧情使用补间动画分类中的命令")]
    [ExecuteInEditMode]
    public class ShakePosition : iTweenCommand
    {
        [Tooltip("设置本步骤使用的变化量")]
        [LabelText("变化量")]
        [SerializeField]
        protected Vector3Data _amount;

        [Tooltip("设置本步骤使用的使用局部坐标")]
        [LabelText("使用局部坐标")]
        [SerializeField]
        protected bool isLocal;

        [Tooltip("设置本步骤使用的坐标轴")]
        [LabelText("坐标轴")]
        [SerializeField]
        protected eTweenAxis axis;

        #region Public members

        public override Tween DoTween()
        {
            var Target = _targetObject.Value.transform;
            var Settings = CreateTweenSettings();
            var Amount = _amount.Value;
            if (axis == eTweenAxis.X) Amount = new Vector3(Amount.x, 0, 0);
            else if (axis == eTweenAxis.Y) Amount = new Vector3(0, Amount.y, 0);
            else if (axis == eTweenAxis.Z) Amount = new Vector3(0, 0, Amount.z);
            var ShakeSettings = new ShakeSettings(Amount, _duration.Value, cycles: loopType == eLegacyTweenLoop.None ? 1 : -1, useUnscaledTime: false);
            return isLocal ? Tween.ShakeLocalPosition(Target, ShakeSettings) : Tween.ShakeCustom(Target, Target.position, ShakeSettings, (Object, Value) => Object.position = Value);
        }

        public override bool HasReference(Variable variable)
        {
            return _amount.vector3Ref == variable || base.HasReference(variable);
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
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
    /// Rotates a game object to the specified angles over time.
    /// </summary>
    [CommandInfo("旧版补间动画", "旋转到角度", "对配置的目标执行旋转到角度 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    [System.Obsolete("Deprecated, 底层已迁移到 PrimeTween 建议新剧情使用补间动画分类中的命令")]
    [ExecuteInEditMode]
    public class RotateTo : iTweenCommand
    {
        [Tooltip("设置本步骤使用的目标对象")]
        [LabelText("目标对象")]
        [SerializeField]
        protected TransformData _toTransform;

        [Tooltip("设置本步骤使用的目标旋转")]
        [LabelText("目标旋转")]
        [SerializeField]
        protected Vector3Data _toRotation;

        [Tooltip("设置本步骤使用的使用局部坐标")]
        [LabelText("使用局部坐标")]
        [SerializeField]
        protected bool isLocal;

        #region Public members

        public override Tween DoTween()
        {
            var Target = _targetObject.Value.transform;
            var Settings = CreateTweenSettings();
            var Start = (isLocal ? Target.localEulerAngles : Target.eulerAngles);
            var End = _toTransform.Value == null ? _toRotation.Value : (isLocal ? _toTransform.Value.localEulerAngles : _toTransform.Value.eulerAngles);
            var Current = (isLocal ? Target.localEulerAngles : Target.eulerAngles);
            End = Current + new Vector3(Mathf.DeltaAngle(Current.x, End.x), Mathf.DeltaAngle(Current.y, End.y), Mathf.DeltaAngle(Current.z, End.z));
            return isLocal ? Tween.LocalEulerAngles(Target, new TweenSettings<Vector3>(End, Settings)) : Tween.EulerAngles(Target, new TweenSettings<Vector3>(End, Settings));
        }

        public override bool HasReference(Variable variable)
        {
            return _toTransform.transformRef == variable || _toRotation.vector3Ref == variable ||
                base.HasReference(variable);
        }

        #endregion

        #region Backwards compatibility

        [HideInInspector] [FormerlySerializedAs("toTransform")] public Transform toTransformOLD;
        [HideInInspector] [FormerlySerializedAs("toRotation")] public Vector3 toRotationOLD;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (toTransformOLD != null)
            {
                _toTransform.Value = toTransformOLD;
                toTransformOLD = null;
            }

            if (toRotationOLD != default(Vector3))
            {
                _toRotation.Value = toRotationOLD;
                toRotationOLD = default(Vector3);
            }
        }

        #endregion
    }
}
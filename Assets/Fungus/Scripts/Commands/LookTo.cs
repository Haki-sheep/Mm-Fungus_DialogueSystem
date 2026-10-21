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
    /// Rotates a GameObject to look at a supplied Transform or Vector3 over time.
    /// </summary>
    [CommandInfo("旧版补间动画", "看向目标", "对配置的目标执行看向目标 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class LookTo : iTweenCommand
    {
        [Tooltip("设置本步骤使用的目标对象")]
        [LabelText("目标对象")]
        [SerializeField]
        protected TransformData _toTransform;

        [Tooltip("设置本步骤使用的目标站位")]
        [LabelText("目标站位")]
        [SerializeField]
        protected Vector3Data _toPosition;

        [Tooltip("设置本步骤使用的坐标轴")]
        [LabelText("坐标轴")]
        [SerializeField]
        protected eTweenAxis axis;

        #region Public members

        public override Tween DoTween()
        {
            var Target = _targetObject.Value.transform;
            var Settings = CreateTweenSettings();
            var Point = _toTransform.Value == null ? _toPosition.Value : _toTransform.Value.position;
            var Start = Target.eulerAngles;
            var End = Quaternion.LookRotation(Point - Target.position).eulerAngles;
            if (axis == eTweenAxis.X) End = new Vector3(End.x, Start.y, Start.z);
            else if (axis == eTweenAxis.Y) End = new Vector3(Start.x, End.y, Start.z);
            else if (axis == eTweenAxis.Z) End = new Vector3(Start.x, Start.y, End.z);
            var Current = Target.eulerAngles;
            End = Current + new Vector3(Mathf.DeltaAngle(Current.x, End.x), Mathf.DeltaAngle(Current.y, End.y), Mathf.DeltaAngle(Current.z, End.z));
            return Tween.EulerAngles(Target, new TweenSettings<Vector3>(End, Settings));
        }

        public override bool HasReference(Variable variable)
        {
            return _toTransform.transformRef == variable || _toPosition.vector3Ref == variable ||
                base.HasReference(variable);
        }

        #endregion

        #region Backwards compatibility

        [HideInInspector] [FormerlySerializedAs("toTransform")] public Transform toTransformOLD;
        [HideInInspector] [FormerlySerializedAs("toPosition")] public Vector3 toPositionOLD;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (toTransformOLD != null)
            {
                _toTransform.Value = toTransformOLD;
                toTransformOLD = null;
            }

            if (toPositionOLD != default(Vector3))
            {
                _toPosition.Value = toPositionOLD;
                toPositionOLD = default(Vector3);
            }
        }

        #endregion
    }
}
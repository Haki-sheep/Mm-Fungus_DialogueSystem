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
    /// Instantly rotates a GameObject to look at the supplied Vector3 then returns it to it's starting rotation over time.
    /// </summary>
    [CommandInfo("旧版补间动画", "从指定方向转回", "对配置的目标执行从指定方向转回 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class LookFrom : iTweenCommand
    {
        [Tooltip("设置本步骤使用的起始对象")]
        [LabelText("起始对象")]
        [SerializeField]
        protected TransformData _fromTransform;

        [Tooltip("设置本步骤使用的起始站位")]
        [LabelText("起始站位")]
        [SerializeField]
        protected Vector3Data _fromPosition;

        [Tooltip("设置本步骤使用的坐标轴")]
        [LabelText("坐标轴")]
        [SerializeField]
        protected eTweenAxis axis;

        #region Public members

        public override Tween DoTween()
        {
            var Target = _targetObject.Value.transform;
            var Settings = CreateTweenSettings();
            var Point = _fromTransform.Value == null ? _fromPosition.Value : _fromTransform.Value.position;
            var Start = Target.eulerAngles;
            var End = Quaternion.LookRotation(Point - Target.position).eulerAngles;
            if (axis == eTweenAxis.X) End = new Vector3(End.x, Start.y, Start.z);
            else if (axis == eTweenAxis.Y) End = new Vector3(Start.x, End.y, Start.z);
            else if (axis == eTweenAxis.Z) End = new Vector3(Start.x, Start.y, End.z);
            Target.eulerAngles = End;
            End = Start;
            var Current = Target.eulerAngles;
            End = Current + new Vector3(Mathf.DeltaAngle(Current.x, End.x), Mathf.DeltaAngle(Current.y, End.y), Mathf.DeltaAngle(Current.z, End.z));
            return Tween.EulerAngles(Target, new TweenSettings<Vector3>(End, Settings));
        }

        public override bool HasReference(Variable variable)
        {
            return _fromTransform.transformRef == variable || _fromPosition.vector3Ref == variable ||
                base.HasReference(variable);
        }

        #endregion

        #region Backwards compatibility

        [HideInInspector] [FormerlySerializedAs("fromTransform")] public Transform fromTransformOLD;
        [HideInInspector] [FormerlySerializedAs("fromPosition")] public Vector3 fromPositionOLD;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (fromTransformOLD != null)
            {
                _fromTransform.Value = fromTransformOLD;
                fromTransformOLD = null;
            }

            if (fromPositionOLD != default(Vector3))
            {
                _fromPosition.Value = fromPositionOLD;
                fromPositionOLD = default(Vector3);
            }
        }

        #endregion
    }
}
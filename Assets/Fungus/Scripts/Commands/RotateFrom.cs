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
    /// Rotates a game object from the specified angles back to its starting orientation over time.
    /// </summary>
    [CommandInfo("旧版补间动画", "从指定角度转回", "对配置的目标执行从指定角度转回 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    [System.Obsolete("Deprecated, 底层已迁移到 PrimeTween 建议新剧情使用补间动画分类中的命令")]
    [ExecuteInEditMode]
    public class RotateFrom : iTweenCommand
    {
        [Tooltip("设置本步骤使用的起始对象")]
        [LabelText("起始对象")]
        [SerializeField]
        protected TransformData _fromTransform;

        [Tooltip("设置本步骤使用的起始旋转")]
        [LabelText("起始旋转")]
        [SerializeField]
        protected Vector3Data _fromRotation;

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
            var End = _fromTransform.Value == null ? _fromRotation.Value : (isLocal ? _fromTransform.Value.localEulerAngles : _fromTransform.Value.eulerAngles);
            if (isLocal) Target.localEulerAngles = End;
            else Target.eulerAngles = End;
            End = Start;
            var Current = (isLocal ? Target.localEulerAngles : Target.eulerAngles);
            End = Current + new Vector3(Mathf.DeltaAngle(Current.x, End.x), Mathf.DeltaAngle(Current.y, End.y), Mathf.DeltaAngle(Current.z, End.z));
            return isLocal ? Tween.LocalEulerAngles(Target, new TweenSettings<Vector3>(End, Settings)) : Tween.EulerAngles(Target, new TweenSettings<Vector3>(End, Settings));
        }

        public override bool HasReference(Variable variable)
        {
            return _fromTransform.transformRef == variable || _fromRotation.vector3Ref == variable ||
                base.HasReference(variable);
        }

        #endregion

        #region Backwards compatibility

        [HideInInspector] [FormerlySerializedAs("fromTransform")] public Transform fromTransformOLD;
        [HideInInspector] [FormerlySerializedAs("fromRotation")] public Vector3 fromRotationOLD;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (fromTransformOLD != null)
            {
                _fromTransform.Value = fromTransformOLD;
                fromTransformOLD = null;
            }

            if (fromRotationOLD != default(Vector3))
            {
                _fromRotation.Value = fromRotationOLD;
                fromRotationOLD = default(Vector3);
            }
        }

        #endregion
    }
}
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
    /// Moves a game object from a specified position back to its starting position over time. The position can be defined by a transform in another object (using To Transform) or by setting an absolute position (using To Position, if To Transform is set to None).
    /// </summary>
    [CommandInfo("旧版补间动画", "从指定位置移回", "对配置的目标执行从指定位置移回 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    [System.Obsolete("Deprecated, 底层已迁移到 PrimeTween 建议新剧情使用补间动画分类中的命令")]
    [ExecuteInEditMode]
    public class MoveFrom : iTweenCommand
    {
        [Tooltip("设置本步骤使用的起始对象")]
        [LabelText("起始对象")]
        [SerializeField]
        protected TransformData _fromTransform;

        [Tooltip("设置本步骤使用的起始站位")]
        [LabelText("起始站位")]
        [SerializeField]
        protected Vector3Data _fromPosition;

        [Tooltip("设置本步骤使用的使用局部坐标")]
        [LabelText("使用局部坐标")]
        [SerializeField]
        protected bool isLocal;

        #region Public members

        public override Tween DoTween()
        {
            var Target = _targetObject.Value.transform;
            var Settings = CreateTweenSettings();
            var Start = (isLocal ? Target.localPosition : Target.position);
            var End = _fromTransform.Value == null ? _fromPosition.Value : (isLocal ? _fromTransform.Value.localPosition : _fromTransform.Value.position);
            if (isLocal) Target.localPosition = End;
            else Target.position = End;
            End = Start;
            return isLocal ? Tween.LocalPosition(Target, new TweenSettings<Vector3>(End, Settings)) : Tween.Position(Target, new TweenSettings<Vector3>(End, Settings));
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
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
    /// Moves a game object to a specified position over time. The position can be defined by a transform in another object (using To Transform) or by setting an absolute position (using To Position, if To Transform is set to None).
    /// </summary>
    [CommandInfo("旧版补间动画", "移动到位置", "对配置的目标执行移动到位置 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    [System.Obsolete("Deprecated, 底层已迁移到 PrimeTween 建议新剧情使用补间动画分类中的命令")]
    [ExecuteInEditMode]
    public class MoveTo : iTweenCommand
    {
        [Tooltip("设置本步骤使用的目标对象")]
        [LabelText("目标对象")]
        [SerializeField]
        protected TransformData _toTransform;

        [Tooltip("设置本步骤使用的目标站位")]
        [LabelText("目标站位")]
        [SerializeField]
        protected Vector3Data _toPosition;

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
            var End = _toTransform.Value == null ? _toPosition.Value : (isLocal ? _toTransform.Value.localPosition : _toTransform.Value.position);
            return isLocal ? Tween.LocalPosition(Target, new TweenSettings<Vector3>(End, Settings)) : Tween.Position(Target, new TweenSettings<Vector3>(End, Settings));
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
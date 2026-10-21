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
    /// Changes a game object's scale to the specified value and back to its original scale over time.
    /// </summary>
    [CommandInfo("旧版补间动画", "从指定大小缩回", "对配置的目标执行从指定大小缩回 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    [System.Obsolete("Deprecated, 底层已迁移到 PrimeTween 建议新剧情使用补间动画分类中的命令")]
    [ExecuteInEditMode]
    public class ScaleFrom : iTweenCommand
    {
        [Tooltip("设置本步骤使用的起始对象")]
        [LabelText("起始对象")]
        [SerializeField]
        protected TransformData _fromTransform;

        [Tooltip("设置本步骤使用的起始缩放")]
        [LabelText("起始缩放")]
        [SerializeField]
        protected Vector3Data _fromScale;

        #region Public members

        public override Tween DoTween()
        {
            var Target = _targetObject.Value.transform;
            var Settings = CreateTweenSettings();
            var Start = Target.localScale;
            var End = _fromTransform.Value == null ? _fromScale.Value : _fromTransform.Value.localScale;
            Target.localScale = End;
            End = Start;
            return Tween.Scale(Target, new TweenSettings<Vector3>(End, Settings));
        }

        public override bool HasReference(Variable variable)
        {
            return _fromTransform.transformRef == variable || _fromScale.vector3Ref == variable ||
                base.HasReference(variable);
        }

        #endregion

        #region Backwards compatibility

        [HideInInspector] [FormerlySerializedAs("fromTransform")] public Transform fromTransformOLD;
        [HideInInspector] [FormerlySerializedAs("fromScale")] public Vector3 fromScaleOLD;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (fromTransformOLD != null)
            {
                _fromTransform.Value = fromTransformOLD;
                fromTransformOLD = null;
            }

            if (fromScaleOLD != default(Vector3))
            {
                _fromScale.Value = fromScaleOLD;
                fromScaleOLD = default(Vector3);
            }
        }

        #endregion
    }
}
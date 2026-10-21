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
    /// Changes a game object's scale to a specified value over time.
    /// </summary>
    [CommandInfo("旧版补间动画", "缩放到大小", "对配置的目标执行缩放到大小 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    [System.Obsolete("Deprecated, 底层已迁移到 PrimeTween 建议新剧情使用补间动画分类中的命令")]
    [ExecuteInEditMode]
    public class ScaleTo : iTweenCommand
    {
        [Tooltip("设置本步骤使用的目标对象")]
        [LabelText("目标对象")]
        [SerializeField]
        protected TransformData _toTransform;

        [Tooltip("设置本步骤使用的目标缩放")]
        [LabelText("目标缩放")]
        [SerializeField]
        protected Vector3Data _toScale = new Vector3Data(Vector3.one);

        #region Public members

        public override Tween DoTween()
        {
            var Target = _targetObject.Value.transform;
            var Settings = CreateTweenSettings();
            var Start = Target.localScale;
            var End = _toTransform.Value == null ? _toScale.Value : _toTransform.Value.localScale;
            return Tween.Scale(Target, new TweenSettings<Vector3>(End, Settings));
        }

        public override bool HasReference(Variable variable)
        {
            return _toTransform.transformRef == variable || _toScale.vector3Ref == variable ||
                base.HasReference(variable);
        }

        #endregion

        #region Backwards compatibility

        [HideInInspector] [FormerlySerializedAs("toTransform")] public Transform toTransformOLD;
        [HideInInspector] [FormerlySerializedAs("toScale")] public Vector3 toScaleOLD;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (toTransformOLD != null)
            {
                _toTransform.Value = toTransformOLD;
                toTransformOLD = null;
            }

            if (toScaleOLD != default(Vector3))
            {
                _toScale.Value = toScaleOLD;
                toScaleOLD = default(Vector3);
            }
        }

        #endregion
    }
}
using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;
using System;
using PrimeTween;

namespace Fungus
{
    /// <summary>
    /// Changes a game object's scale to a specified value over time.
    /// </summary>
    [CommandInfo("补间动画", "缩放对象", "对配置的目标执行缩放对象 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class ScaleLean : BaseLeanTweenCommand
    {
        [Tooltip("设置本步骤使用的目标对象")]
        [LabelText("目标对象")]
        [SerializeField]
        protected TransformData _toTransform;

        [Tooltip("设置本步骤使用的目标缩放")]
        [LabelText("目标缩放")]
        [SerializeField]
        protected Vector3Data _toScale = new Vector3Data(Vector3.one);

        public override Tween ExecuteTween()
        {
            var sc = _toTransform.Value == null ? _toScale.Value : _toTransform.Value.localScale;

            if (IsInAddativeMode)
            {
                sc += _targetObject.Value.transform.localScale;
            }

            if (IsInFromMode)
            {
                var cur = _targetObject.Value.transform.localScale;
                _targetObject.Value.transform.localScale = sc;
                sc = cur;
            }

            return FungusTween.Track(_targetObject.Value, Tween.Scale(_targetObject.Value.transform, new TweenSettings<Vector3>(sc, CreateTweenSettings())));
        }
        
        public override bool HasReference(Variable variable)
        {
            return variable == _toTransform.transformRef || _toScale.vector3Ref == variable || base.HasReference(variable);
        }
    }
}
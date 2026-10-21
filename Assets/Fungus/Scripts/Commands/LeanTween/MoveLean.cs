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
    /// Moves a game object to a specified position over time. The position can be defined by a transform in another object (using To Transform) or by setting an absolute position (using To Position, if To Transform is set to None).
    /// </summary>
    [CommandInfo("补间动画", "移动对象", "对配置的目标执行移动对象 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class MoveLean : BaseLeanTweenCommand
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
        

        public override Tween ExecuteTween()
        {
            var Target = _targetObject.Value.transform;
            var Start = isLocal ? Target.localPosition : Target.position;
            var End = _toTransform.Value == null ? _toPosition.Value : isLocal ? _toTransform.Value.localPosition : _toTransform.Value.position;
            if (IsInAddativeMode) End += Start;
            if (IsInFromMode)
            {
                if (isLocal) Target.localPosition = End;
                else Target.position = End;
                End = Start;
            }
            var Settings = new TweenSettings<Vector3>(End, CreateTweenSettings());
            return FungusTween.Track(Target.gameObject, isLocal ? Tween.LocalPosition(Target, Settings) : Tween.Position(Target, Settings));
        }

        public override bool HasReference(Variable variable)
        {
            return _toTransform.transformRef == variable || _toPosition.vector3Ref == variable || base.HasReference(variable);
        }
    }
}
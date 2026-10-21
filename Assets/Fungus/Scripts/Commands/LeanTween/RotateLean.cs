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
    /// Rotates a game object to the specified angles over time.
    /// </summary>
    [CommandInfo("补间动画", "旋转对象", "对配置的目标执行旋转对象 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class RotateLean : BaseLeanTweenCommand
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

        public enum RotateMode { [UnityEngine.InspectorName("只旋转")]
        PureRotate, [UnityEngine.InspectorName("二维看向目标")]
        LookAt2D, [UnityEngine.InspectorName("三维看向目标")]
        LookAt3D}
        [Tooltip("设置本步骤使用的旋转方式")]
        [LabelText("旋转方式")]
        [SerializeField]
        protected RotateMode rotateMode = RotateMode.PureRotate;


        public override Tween ExecuteTween()
        {
            var Target = _targetObject.Value.transform;
            var Start = isLocal ? Target.localEulerAngles : Target.eulerAngles;
            var End = _toTransform.Value == null ? _toRotation.Value : isLocal ? _toTransform.Value.localEulerAngles : _toTransform.Value.eulerAngles;
            if (rotateMode == RotateMode.LookAt3D || rotateMode == RotateMode.LookAt2D)
            {
                var Point = _toTransform.Value == null ? _toRotation.Value : _toTransform.Value.position;
                var Direction = Point - Target.position;
                var Rotation = rotateMode == RotateMode.LookAt3D ? Quaternion.LookRotation(Direction) : Quaternion.Euler(0, 0, Mathf.Atan2(Direction.y, Direction.x) * Mathf.Rad2Deg);
                if (isLocal && Target.parent != null) Rotation = Quaternion.Inverse(Target.parent.rotation) * Rotation;
                End = Rotation.eulerAngles;
            }
            if (IsInAddativeMode) End += Start;
            if (IsInFromMode)
            {
                if (isLocal) Target.localEulerAngles = End;
                else Target.eulerAngles = End;
                End = Start;
            }
            if (!IsInAddativeMode)
            {
                var Current = isLocal ? Target.localEulerAngles : Target.eulerAngles;
                End = Current + new Vector3(Mathf.DeltaAngle(Current.x, End.x), Mathf.DeltaAngle(Current.y, End.y), Mathf.DeltaAngle(Current.z, End.z));
            }
            var Settings = new TweenSettings<Vector3>(End, CreateTweenSettings());
            return FungusTween.Track(Target.gameObject, isLocal ? Tween.LocalEulerAngles(Target, Settings) : Tween.EulerAngles(Target, Settings));
        }

        public override bool HasReference(Variable variable)
        {
            return variable == _toTransform.transformRef || _toRotation.vector3Ref == variable || base.HasReference(variable);
        }
    }
}
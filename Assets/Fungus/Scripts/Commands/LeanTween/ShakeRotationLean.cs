// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using PrimeTween;

namespace Fungus
{
    /// <summary>
    /// Impulse style shake of an object's rotation, using PrimeTween internally.
    /// </summary>
    [CommandInfo("补间动画", "旋转抖动", "对配置的目标执行旋转抖动 使用下方参数设置具体行为")]
    [AddComponentMenu("Impulse style shake of an object's rotation, using PrimeTween internally.")]
    public class ShakeRotationLean : ShakeLean
    {
        public override Tween ExecuteTween()
        {
            if (isLocal)
                return PrimeTweenHelpers.ShakeEulerLocal(
                    _targetObject.Value.transform,
                    _axisScale.Value,
                    _axisSpeedRange.Value,
                    _duration.Value, easeType, waitUntilFinished ? CreateContinueCallback() : null);
            else
                return PrimeTweenHelpers.ShakeEuler(
                    _targetObject.Value.transform,
                    _axisScale.Value,
                    _axisSpeedRange.Value,
                    _duration.Value, easeType, waitUntilFinished ? CreateContinueCallback() : null);
        }
    }
}

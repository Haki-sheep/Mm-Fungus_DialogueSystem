// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using PrimeTween;

namespace Fungus
{
    /// <summary>
    /// Impulse style shake of an object's scale, using PrimeTween internally.
    /// </summary>
    [CommandInfo("补间动画", "缩放抖动", "对配置的目标执行缩放抖动 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    public class ShakeScaleLean : ShakeLean
    {
        public override Tween ExecuteTween()
        {
            return PrimeTweenHelpers.ShakeScale(
                _targetObject.Value.transform,
                _axisScale.Value,
                _axisSpeedRange.Value,
                _duration.Value, easeType, waitUntilFinished ? CreateContinueCallback() : null);
        }
    }
}

using System;
using PrimeTween;
using UnityEngine;

namespace Fungus
{
    public enum eTweenEase
    {
        [InspectorName("默认（线性）")] notUsed = 0,
        [InspectorName("线性")] linear = 1,
        [InspectorName("缓出二次")] easeOutQuad = 2,
        [InspectorName("缓入二次")] easeInQuad = 3,
        [InspectorName("缓入缓出二次")] easeInOutQuad = 4,
        [InspectorName("缓入三次")] easeInCubic = 5,
        [InspectorName("缓出三次")] easeOutCubic = 6,
        [InspectorName("缓入缓出三次")] easeInOutCubic = 7,
        [InspectorName("缓入四次")] easeInQuart = 8,
        [InspectorName("缓出四次")] easeOutQuart = 9,
        [InspectorName("缓入缓出四次")] easeInOutQuart = 10,
        [InspectorName("缓入五次")] easeInQuint = 11,
        [InspectorName("缓出五次")] easeOutQuint = 12,
        [InspectorName("缓入缓出五次")] easeInOutQuint = 13,
        [InspectorName("缓入正弦")] easeInSine = 14,
        [InspectorName("缓出正弦")] easeOutSine = 15,
        [InspectorName("缓入缓出正弦")] easeInOutSine = 16,
        [InspectorName("缓入指数")] easeInExpo = 17,
        [InspectorName("缓出指数")] easeOutExpo = 18,
        [InspectorName("缓入缓出指数")] easeInOutExpo = 19,
        [InspectorName("缓入圆形")] easeInCirc = 20,
        [InspectorName("缓出圆形")] easeOutCirc = 21,
        [InspectorName("缓入缓出圆形")] easeInOutCirc = 22,
        [InspectorName("缓入弹跳")] easeInBounce = 23,
        [InspectorName("缓出弹跳")] easeOutBounce = 24,
        [InspectorName("缓入缓出弹跳")] easeInOutBounce = 25,
        [InspectorName("缓入回退")] easeInBack = 26,
        [InspectorName("缓出回退")] easeOutBack = 27,
        [InspectorName("缓入缓出回退")] easeInOutBack = 28,
        [InspectorName("缓入弹性")] easeInElastic = 29,
        [InspectorName("缓出弹性")] easeOutElastic = 30,
        [InspectorName("缓入缓出弹性")] easeInOutElastic = 31,
        [InspectorName("弹簧")] easeSpring = 32,
        [InspectorName("抖动")] easeShake = 33,
        [InspectorName("冲击回弹")] punch = 34,
        [InspectorName("历史兼容：单次")] once = 35,
        [InspectorName("历史兼容：循环")] clamp = 36,
        [InspectorName("历史兼容：往返")] pingPong = 37,
        [InspectorName("历史兼容：线性")] animationCurve = 38
    }

    public enum eLegacyTweenEase
    {
        [InspectorName("缓入二次")] easeInQuad = 0,
        [InspectorName("缓出二次")] easeOutQuad = 1,
        [InspectorName("缓入缓出二次")] easeInOutQuad = 2,
        [InspectorName("缓入三次")] easeInCubic = 3,
        [InspectorName("缓出三次")] easeOutCubic = 4,
        [InspectorName("缓入缓出三次")] easeInOutCubic = 5,
        [InspectorName("缓入四次")] easeInQuart = 6,
        [InspectorName("缓出四次")] easeOutQuart = 7,
        [InspectorName("缓入缓出四次")] easeInOutQuart = 8,
        [InspectorName("缓入五次")] easeInQuint = 9,
        [InspectorName("缓出五次")] easeOutQuint = 10,
        [InspectorName("缓入缓出五次")] easeInOutQuint = 11,
        [InspectorName("缓入正弦")] easeInSine = 12,
        [InspectorName("缓出正弦")] easeOutSine = 13,
        [InspectorName("缓入缓出正弦")] easeInOutSine = 14,
        [InspectorName("缓入指数")] easeInExpo = 15,
        [InspectorName("缓出指数")] easeOutExpo = 16,
        [InspectorName("缓入缓出指数")] easeInOutExpo = 17,
        [InspectorName("缓入圆形")] easeInCirc = 18,
        [InspectorName("缓出圆形")] easeOutCirc = 19,
        [InspectorName("缓入缓出圆形")] easeInOutCirc = 20,
        [InspectorName("线性")] linear = 21,
        [InspectorName("弹簧")] spring = 22,
        [InspectorName("缓入弹跳")] easeInBounce = 23,
        [InspectorName("缓出弹跳")] easeOutBounce = 24,
        [InspectorName("缓入缓出弹跳")] easeInOutBounce = 25,
        [InspectorName("缓入回退")] easeInBack = 26,
        [InspectorName("缓出回退")] easeOutBack = 27,
        [InspectorName("缓入缓出回退")] easeInOutBack = 28,
        [InspectorName("缓入弹性")] easeInElastic = 29,
        [InspectorName("缓出弹性")] easeOutElastic = 30,
        [InspectorName("缓入缓出弹性")] easeInOutElastic = 31,
        [InspectorName("冲击回弹")] punch = 32
    }

    public enum eLegacyTweenLoop
    {
        [InspectorName("不循环")] None = 0,
        [InspectorName("循环")] Loop = 1,
        [InspectorName("往返")] PingPong = 2
    }
    public enum eTweenLoop
    {
        [InspectorName("不循环")] Once = 35,
        [InspectorName("循环")] Repeat = 36,
        [InspectorName("往返")] PingPong = 37
    }
    public enum eTweenAxis
    {
        [InspectorName("不限制")] None = 0,
        [InspectorName("仅 X 轴")] X = 1,
        [InspectorName("仅 Y 轴")] Y = 2,
        [InspectorName("仅 Z 轴")] Z = 3
    }

    public static class TweenEasing
    {
        private static readonly AnimationCurve punchCurve = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(0.112586f, 0.9976035f), new Keyframe(0.3120486f, -0.1720615f), new Keyframe(0.4316337f, 0.07030682f), new Keyframe(0.5524869f, -0.03141804f), new Keyframe(0.6549395f, 0.003909959f), new Keyframe(0.770987f, -0.009817753f), new Keyframe(0.8838775f, 0.001939224f), new Keyframe(1.0f, 0.0f));
        private static readonly AnimationCurve shakeCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.25f, 1f), new Keyframe(0.75f, -1f), new Keyframe(1f, 0f));
        private static readonly AnimationCurve springCurve = CreateSpringCurve();
        private static readonly eTweenEase[] legacyEaseList = { eTweenEase.easeInQuad, eTweenEase.easeOutQuad, eTweenEase.easeInOutQuad, eTweenEase.easeInCubic, eTweenEase.easeOutCubic, eTweenEase.easeInOutCubic, eTweenEase.easeInQuart, eTweenEase.easeOutQuart, eTweenEase.easeInOutQuart, eTweenEase.easeInQuint, eTweenEase.easeOutQuint, eTweenEase.easeInOutQuint, eTweenEase.easeInSine, eTweenEase.easeOutSine, eTweenEase.easeInOutSine, eTweenEase.easeInExpo, eTweenEase.easeOutExpo, eTweenEase.easeInOutExpo, eTweenEase.easeInCirc, eTweenEase.easeOutCirc, eTweenEase.easeInOutCirc, eTweenEase.linear, eTweenEase.easeSpring, eTweenEase.easeInBounce, eTweenEase.easeOutBounce, eTweenEase.easeInOutBounce, eTweenEase.easeInBack, eTweenEase.easeOutBack, eTweenEase.easeInOutBack, eTweenEase.easeInElastic, eTweenEase.easeOutElastic, eTweenEase.easeInOutElastic, eTweenEase.punch };

        public static Easing Get(eLegacyTweenEase Type) => Get(legacyEaseList[(int)Type]);

        public static Easing Get(eTweenEase Type)
        {
            switch (Type)
            {
                case eTweenEase.easeOutQuad: return PrimeTween.Ease.OutQuad;
                case eTweenEase.easeInQuad: return PrimeTween.Ease.InQuad;
                case eTweenEase.easeInOutQuad: return PrimeTween.Ease.InOutQuad;
                case eTweenEase.easeInCubic: return PrimeTween.Ease.InCubic;
                case eTweenEase.easeOutCubic: return PrimeTween.Ease.OutCubic;
                case eTweenEase.easeInOutCubic: return PrimeTween.Ease.InOutCubic;
                case eTweenEase.easeInQuart: return PrimeTween.Ease.InQuart;
                case eTweenEase.easeOutQuart: return PrimeTween.Ease.OutQuart;
                case eTweenEase.easeInOutQuart: return PrimeTween.Ease.InOutQuart;
                case eTweenEase.easeInQuint: return PrimeTween.Ease.InQuint;
                case eTweenEase.easeOutQuint: return PrimeTween.Ease.OutQuint;
                case eTweenEase.easeInOutQuint: return PrimeTween.Ease.InOutQuint;
                case eTweenEase.easeInSine: return PrimeTween.Ease.InSine;
                case eTweenEase.easeOutSine: return PrimeTween.Ease.OutSine;
                case eTweenEase.easeInOutSine: return PrimeTween.Ease.InOutSine;
                case eTweenEase.easeInExpo: return PrimeTween.Ease.InExpo;
                case eTweenEase.easeOutExpo: return PrimeTween.Ease.OutExpo;
                case eTweenEase.easeInOutExpo: return PrimeTween.Ease.InOutExpo;
                case eTweenEase.easeInCirc: return PrimeTween.Ease.InCirc;
                case eTweenEase.easeOutCirc: return PrimeTween.Ease.OutCirc;
                case eTweenEase.easeInOutCirc: return PrimeTween.Ease.InOutCirc;
                case eTweenEase.easeInBounce: return PrimeTween.Ease.InBounce;
                case eTweenEase.easeOutBounce: return PrimeTween.Ease.OutBounce;
                case eTweenEase.easeInOutBounce: return PrimeTween.Ease.InOutBounce;
                case eTweenEase.easeInBack: return PrimeTween.Ease.InBack;
                case eTweenEase.easeOutBack: return PrimeTween.Ease.OutBack;
                case eTweenEase.easeInOutBack: return PrimeTween.Ease.InOutBack;
                case eTweenEase.easeInElastic: return PrimeTween.Ease.InElastic;
                case eTweenEase.easeOutElastic: return PrimeTween.Ease.OutElastic;
                case eTweenEase.easeInOutElastic: return PrimeTween.Ease.InOutElastic;
                case eTweenEase.easeSpring: return Easing.Curve(springCurve);
                case eTweenEase.easeShake: return Easing.Curve(shakeCurve);
                case eTweenEase.punch: return Easing.Curve(punchCurve);
                case eTweenEase.notUsed:
                case eTweenEase.linear:
                case eTweenEase.once:
                case eTweenEase.clamp:
                case eTweenEase.pingPong:
                case eTweenEase.animationCurve: return PrimeTween.Ease.Linear;
                default: throw new ArgumentOutOfRangeException(nameof(Type), Type, null);
            }
        }

        private static AnimationCurve CreateSpringCurve()
        {
            const int SampleCount = 128;
            var KeyList = new Keyframe[SampleCount + 1];
            for (int Index = 0; Index <= SampleCount; Index++)
            {
                float Progress = (float)Index / SampleCount;
                float Value = (Mathf.Sin(Progress * Mathf.PI * (0.2f + 2.5f * Progress * Progress * Progress)) * Mathf.Pow(1f - Progress, 2.2f) + Progress) * (1f + 1.2f * (1f - Progress));
                KeyList[Index] = new Keyframe(Progress, Value);
            }
            return new AnimationCurve(KeyList);
        }
    }
}

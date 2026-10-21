using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.UI;
using PrimeTween;

namespace Fungus
{
    /// <summary>
    /// Select which type of fade will be applied.
    /// </summary>
    public enum FadeMode
    {
        /// <summary> Fade the alpha color component only. </summary>
        [UnityEngine.InspectorName("不透明度")]
        Alpha,
        /// <summary> Fade all color components (RGBA). </summary>
        [UnityEngine.InspectorName("颜色")]
        Color
    }

    /// <summary>
    /// Fades a UI object.
    /// </summary>
    [CommandInfo("界面", "界面淡入淡出", "对配置的目标执行界面淡入淡出 使用下方参数设置具体行为")]
    public class FadeUI : TweenUI 
    {
        [LabelText("淡入淡出方式")]
        [SerializeField]
        protected FadeMode fadeMode = FadeMode.Alpha;

        [LabelText("目标颜色")]
        [SerializeField]
        protected ColorData targetColor = new ColorData(Color.white);

        [LabelText("目标不透明度")]
        [SerializeField]
        protected FloatData targetAlpha = new FloatData(1f);

        protected override void ApplyTween(GameObject go)
        {
            var Graphics = go.GetComponentsInChildren<Graphic>();
            foreach (var Graphic in Graphics)
            {
                if (Mathf.Approximately(duration.Value, 0f))
                {
                    var Color = fadeMode == FadeMode.Color ? targetColor.Value : Graphic.color;
                    if (fadeMode == FadeMode.Alpha) Color.a = targetAlpha.Value;
                    Graphic.color = Color;
                }
                else
                {
                    var Animation = fadeMode == FadeMode.Alpha
                        ? Tween.Alpha(Graphic, targetAlpha.Value, duration.Value, TweenEasing.Get(tweenType))
                        : Tween.Color(Graphic, targetColor.Value, duration.Value, TweenEasing.Get(tweenType));
                    TrackTween(FungusTween.Track(Graphic.gameObject, Animation));
                }
            }

            var TextMeshes = go.GetComponentsInChildren<TextMesh>();
            foreach (var TextMesh in TextMeshes)
            {
                if (Mathf.Approximately(duration.Value, 0f))
                {
                    var Color = fadeMode == FadeMode.Color ? targetColor.Value : TextMesh.color;
                    if (fadeMode == FadeMode.Alpha) Color.a = targetAlpha.Value;
                    TextMesh.color = Color;
                }
                else if (fadeMode == FadeMode.Alpha)
                {
                    TrackTween(FungusTween.Value(TextMesh.gameObject, TextMesh.color.a, targetAlpha.Value, duration.Value,
                        Value =>
                        {
                            var Color = TextMesh.color;
                            Color.a = Value;
                            TextMesh.color = Color;
                        }, tweenType));
                }
                else
                {
                    TrackTween(FungusTween.Track(TextMesh.gameObject,
                        Tween.Custom(TextMesh, TextMesh.color, targetColor.Value, duration.Value,
                            (Target, Value) => Target.color = Value, TweenEasing.Get(tweenType))));
                }
            }

            //canvas groups don't support color but we can anim the alpha IN the color
            var CanvasGroups = go.GetComponentsInChildren<CanvasGroup>();
            foreach (var CanvasGroup in CanvasGroups)
            {
                float Alpha = fadeMode == FadeMode.Alpha ? targetAlpha.Value : targetColor.Value.a;
                if (Mathf.Approximately(duration.Value, 0f)) CanvasGroup.alpha = Alpha;
                else TrackTween(FungusTween.Track(CanvasGroup.gameObject,
                    Tween.Alpha(CanvasGroup, Alpha, duration.Value, TweenEasing.Get(tweenType))));
            }
        }

        protected override string GetSummaryValue()
        {
            if (fadeMode == FadeMode.Alpha)
            {
                return targetAlpha.Value.ToString() + " alpha";
            }
            else if (fadeMode == FadeMode.Color)
            {
                return targetColor.Value.ToString()  + " color";
            }

            return "";
        }

        #region Public members

        public override bool IsPropertyVisible(string propertyName)
        {
            if (fadeMode == FadeMode.Alpha &&
                propertyName == "targetColor")
            {
                return false;
            }

            if (fadeMode == FadeMode.Color &&
                propertyName == "targetAlpha")
            {
                return false;
            }

            return true;
        }

        public override bool HasReference(Variable variable)
        {
            return targetColor.colorRef == variable || targetAlpha.floatRef == variable ||
                base.HasReference(variable);
        }

        #endregion
    }
}

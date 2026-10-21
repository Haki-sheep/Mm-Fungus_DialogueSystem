using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using PrimeTween;

namespace Fungus
{
    /// <summary>
    /// Draws a fullscreen texture over the scene to give a fade effect. Setting Target Alpha to 1 will obscure the screen, alpha 0 will reveal the screen.
    /// If no Fade Texture is provided then a default flat color texture is used.
    /// </summary>
    [CommandInfo("镜头", "屏幕淡入淡出", "对配置的目标执行屏幕淡入淡出 使用下方参数设置具体行为" +
                 "If no Fade Texture is provided then a default flat color texture is used.")]
    [AddComponentMenu("")]
    public class FadeScreen : Command 
    {
        [Tooltip("设置效果持续多久 单位为秒")]
        [LabelText("持续时间（秒）")]
        [SerializeField]
        protected float duration = 1f;

        [Tooltip("设置本步骤使用的目标不透明度")]
        [LabelText("目标不透明度")]
        [SerializeField]
        protected float targetAlpha = 1f;

        [Tooltip("勾选后等待本步骤完成再继续 不勾选则立即执行下一步")]
        [LabelText("等完成后再执行下一步")]
        [SerializeField]
        protected bool waitUntilFinished = true;

        [Tooltip("设置本步骤使用的淡入淡出颜色")]
        [LabelText("淡入淡出颜色")]
        [SerializeField]
        protected Color fadeColor = Color.black;

        [Tooltip("设置本步骤使用的遮罩贴图")]
        [LabelText("遮罩贴图")]
        [SerializeField]
        protected Texture2D fadeTexture;

        [LabelText("淡入淡出缓动")]
        [SerializeField]
        protected eTweenEase fadeTweenType = eTweenEase.easeInOutQuad;

        #region Public members

        public override void OnEnter()
        {
            var cameraManager = FungusManager.Instance.CameraManager;
            
            if (fadeTexture)
            {
                cameraManager.ScreenFadeTexture = fadeTexture;
            }
            else
            {
                cameraManager.ScreenFadeTexture = CameraManager.CreateColorTexture(fadeColor, 32, 32);
            }
            
            var Complete = waitUntilFinished ? CreateContinueCallback() : (System.Action)null;
            cameraManager.Fade(targetAlpha, duration, Complete, fadeTweenType);
            
            if (!waitUntilFinished)
            {
                Continue();
            }
        }
        
        public override string GetSummary()
        {
            return "淡入淡出到" + targetAlpha + " 持续 " + duration + " 秒";
        }
        
        public override Color GetButtonColor()
        {
            return new Color32(216, 228, 170, 255);
        }

        #endregion
    }    
}
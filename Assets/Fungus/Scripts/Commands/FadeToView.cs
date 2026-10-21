using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using PrimeTween;

namespace Fungus
{
    /// <summary>
    /// Fades the camera out and in again at a position specified by a View object.
    /// </summary>
    [CommandInfo("镜头", "淡出并切换镜头", "对配置的目标执行淡出并切换镜头 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    public class FadeToView : Command 
    {
        [Tooltip("设置效果持续多久 单位为秒")]
        [LabelText("持续时间（秒）")]
        [SerializeField]
        protected float duration = 1f;

        [Tooltip("设置本步骤使用的淡出")]
        [LabelText("淡出")]
        [SerializeField]
        protected bool fadeOut = true;

        [Tooltip("设置本步骤使用的目标镜头位置")]
        [LabelText("目标镜头位置")]
        [SerializeField]
        protected View targetView;

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

        [Tooltip("设置本步骤使用的控制的摄像机")]
        [LabelText("控制的摄像机")]
        [SerializeField]
        protected Camera targetCamera;

        [LabelText("淡入淡出缓动")]
        [SerializeField]
        protected eTweenEase fadeTweenType = eTweenEase.easeInOutQuad;
        [LabelText("镜头缩放缓动")]
        [SerializeField]
        protected eTweenEase orthoSizeTweenType = eTweenEase.easeInOutQuad;
        [LabelText("位置缓动")]
        [SerializeField]
        protected eTweenEase posTweenType = eTweenEase.easeInOutQuad;
        [LabelText("旋转缓动")]
        [SerializeField]
        protected eTweenEase rotTweenType = eTweenEase.easeInOutQuad;

        protected virtual void Start()
        {
            AcquireCamera();
        }

        protected virtual void AcquireCamera()
        {
            if (targetCamera != null)
            {
                return;
            }

            targetCamera = Camera.main;
            if (targetCamera == null)
            {
            #if UNITY_6000
                targetCamera = GameObject.FindFirstObjectByType<Camera>();
            #else
                targetCamera = GameObject.FindObjectOfType<Camera>();
            #endif
            }
        }

        #region Public members

        /// <summary>
        /// View to transition to when Fade is complete
        /// </summary>
        public virtual View TargetView { get { return targetView; } }

        public override void OnEnter()
        {
            AcquireCamera();
            if (targetCamera == null ||
                targetView == null)
            {
                Continue();
                return;
            }

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
            cameraManager.FadeToView(targetCamera, targetView, duration, fadeOut, Complete, fadeTweenType, orthoSizeTweenType, posTweenType, rotTweenType);

            if (!waitUntilFinished)
            {
                Continue();
            }
        }

        public override void OnStopExecuting()
        {
            var cameraManager = FungusManager.Instance.CameraManager;

            cameraManager.Stop();
        }

        public override string GetSummary()
        {
            if (targetView == null)
            {
                return "请先设置镜头位置";
            }
            else
            {
                return targetView.name;
            }
        }

        public override Color GetButtonColor()
        {
            return new Color32(216, 228, 170, 255);
        }

        #endregion
    }
}
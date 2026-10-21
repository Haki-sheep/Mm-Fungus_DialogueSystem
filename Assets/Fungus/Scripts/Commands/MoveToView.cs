using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using PrimeTween;

namespace Fungus
{
    /// <summary>
    /// Moves the camera to a location specified by a View object.
    /// </summary>
    [CommandInfo("镜头", "移动镜头", "让摄像机平滑移动到指定镜头位置 并调整画面范围")]
    [AddComponentMenu("")]
    public class MoveToView : Command 
    {
        [Tooltip("设置效果持续多久 单位为秒")]
        [LabelText("持续时间（秒）")]
        [SerializeField]
        protected float duration = 1;

        [Tooltip("设置本步骤使用的目标镜头位置")]
        [LabelText("目标镜头位置")]
        [SerializeField]
        protected View targetView;
        public virtual View TargetView { get { return targetView; } }

        [Tooltip("勾选后等待本步骤完成再继续 不勾选则立即执行下一步")]
        [LabelText("等完成后再执行下一步")]
        [SerializeField]
        protected bool waitUntilFinished = true;

        [Tooltip("设置本步骤使用的控制的摄像机")]
        [LabelText("控制的摄像机")]
        [SerializeField]
        protected Camera targetCamera;

        [LabelText("镜头缩放缓动")]
        [SerializeField]
        protected eTweenEase orthoSizeTweenType = eTweenEase.easeInOutQuad;
        [LabelText("位置缓动")]
        [SerializeField]
        protected eTweenEase posTweenType = eTweenEase.easeInOutQuad;
        [LabelText("旋转缓动")]
        [SerializeField]
        protected eTweenEase rotTweenType = eTweenEase.easeInOutQuad;

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

        public virtual void Start()
        {
            AcquireCamera();
        }

        #region Public members

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

            Vector3 targetPosition = targetView.transform.position;
            Quaternion targetRotation = targetView.transform.rotation;
            float targetSize = targetView.ViewSize;

            var Complete = waitUntilFinished ? CreateContinueCallback() : (System.Action)null;
            cameraManager.PanToPosition(targetCamera, targetPosition, targetRotation, targetSize, duration, Complete, orthoSizeTweenType, posTweenType, rotTweenType);

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
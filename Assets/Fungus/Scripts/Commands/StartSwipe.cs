using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Activates swipe panning mode where the player can pan the camera within the area between viewA & viewB.
    /// </summary>
    [CommandInfo("镜头", "启用拖动画面", "对配置的目标执行启用拖动画面 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    public class StartSwipe : Command 
    {
        [Tooltip("设置本步骤使用的起始镜头")]
        [LabelText("起始镜头")]
        [SerializeField]
        protected View viewA;

        [Tooltip("设置本步骤使用的结束镜头")]
        [LabelText("结束镜头")]
        [SerializeField]
        protected View viewB;

        [Tooltip("设置效果持续多久 单位为秒")]
        [LabelText("持续时间（秒）")]
        [SerializeField]
        protected float duration = 0.5f;

        [Tooltip("设置本步骤使用的速度倍率")]
        [LabelText("速度倍率")]
        [SerializeField]
        protected float speedMultiplier = 1f;

        [Tooltip("设置本步骤使用的控制的摄像机")]
        [LabelText("控制的摄像机")]
        [SerializeField]
        protected Camera targetCamera;

        #region Public members

        public virtual void Start()
        {
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }
            if (targetCamera == null)
            {
            #if UNITY_6000
                targetCamera = GameObject.FindFirstObjectByType<Camera>();
            #else
                targetCamera = GameObject.FindObjectOfType<Camera>();
            #endif
            }
        }

        public override void OnEnter()
        {
            if (targetCamera == null ||
                viewA == null ||
                viewB == null)
            {
                Continue();
                return;
            }

            var cameraManager = FungusManager.Instance.CameraManager;

            cameraManager.StartSwipePan(targetCamera, viewA, viewB, duration, speedMultiplier, () => Continue() );
        }

        public override string GetSummary()
        {
            if (viewA == null)
            {
                return "请先设置起始镜头";
            }

            if (viewB == null)
            {
                return "请先设置结束镜头";
            }

            return viewA.name + " 到 " + viewB.name;
        }

        public override Color GetButtonColor()
        {
            return new Color32(216, 228, 170, 255);
        }

        #endregion
    }
}
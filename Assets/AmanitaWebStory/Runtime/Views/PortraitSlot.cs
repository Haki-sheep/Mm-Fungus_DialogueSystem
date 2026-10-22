using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace AmanitaWebStory
{
    /// <summary> 单个立绘站位 由 UguiDialogueView 按角色分配 </summary>
    public sealed class PortraitSlot : MonoBehaviour
    {
        [SerializeField]
        private Image image;
        [SerializeField]
        private Color dimColor = new Color(0.45f, 0.45f, 0.5f, 1f);
        [SerializeField, Min(0)]
        private float fadeSeconds = 0.2f;

        private Tween tween;

        public StoryActor Actor { get; private set; }
        /// <summary> 最近一次说话的序号 槽位不足时替换最久未说话者 </summary>
        public int LastSpoken { get; private set; }

        private void OnDestroy() { tween.Stop(); }

        public void Show(StoryActor Actor, Sprite Portrait, int Order)
        {
            bool Entering = this.Actor != Actor;
            this.Actor = Actor;
            LastSpoken = Order;
            image.sprite = Portrait;
            image.enabled = true;
            tween.Stop();
            image.color = Entering ? new Color(1f, 1f, 1f, 0f) : image.color;
            tween = Tween.Color(image, Color.white, fadeSeconds, useUnscaledTime: true);
        }

        public void Dim()
        {
            if (Actor == null)
                return;
            tween.Stop();
            tween = Tween.Color(image, dimColor, fadeSeconds, useUnscaledTime: true);
        }

        public void Clear()
        {
            tween.Stop();
            Actor = null;
            LastSpoken = 0;
            image.sprite = null;
            image.enabled = false;
        }
    }
}

using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace AmanitaWebStory.Samples
{
    public sealed class StorySeparationDemo : MonoBehaviour
    {
        [SerializeField]
        private StoryAsset story;
        [SerializeField]
        private StoryPlayer player;
        [SerializeField]
        private DialogueView dialogueView;
        [SerializeField]
        private DialogueView radioView;
        [SerializeField]
        private TMP_Text statusText;

        private bool switching;

        private void OnEnable() { player.EventRaised += ReceiveEvent; }
        private void OnDisable() { player.EventRaised -= ReceiveEvent; }

        public void PlayDialogue() { PlayAsync(dialogueView).Forget(); }
        public void PlayRadio() { PlayAsync(radioView).Forget(); }
        public void Stop() { player.Stop(); }

        private async UniTask PlayAsync(DialogueView View)
        {
            if (switching)
                return;
            switching = true;
            try
            {
                player.Stop();
                await UniTask.WaitUntil(() => !player.IsPlaying, cancellationToken: this.GetCancellationTokenOnDestroy());
            }
            finally { switching = false; }
            try
            {
                statusText.text = "当前界面：" + View.name + "  |  本次信任值从 0 开始";
                await player.PlayAsync(story, View, this.GetCancellationTokenOnDestroy());
                statusText.text += "\n剧情结束  |  最终信任值：" + player.Session.Variables["trust"].Integer;
            }
            catch (OperationCanceledException)
            {
                if (this != null)
                    statusText.text = "已停止 可选择任一界面重新播放";
            }
        }

        private void ReceiveEvent(StoryEvent Event)
        {
            switch (Event.EventId)
            {
                case "gate_opened":
                    statusText.text = "业务收到事件：允许通行  |  本次信任值：" + Event.Value.Integer;
                    break;
                default: throw new NotSupportedException("示例没有接入事件 " + Event.EventId);
            }
        }
    }
}

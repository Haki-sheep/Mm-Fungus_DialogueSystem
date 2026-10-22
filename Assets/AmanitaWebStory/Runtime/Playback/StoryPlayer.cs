using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AmanitaWebStory
{
    public sealed class StoryPlayer : MonoBehaviour
    {
        [SerializeField, InspectorName("剧情")]
        private StoryAsset story;
        [SerializeField, InspectorName("对话界面")]
        private DialogueView view;
        [SerializeField, InspectorName("自动播放")]
        private bool autoPlay;

        private CancellationTokenSource playbackCancellation;

        public bool IsPlaying => playbackCancellation != null;
        public StorySession Session { get; private set; }
        public event Action<StoryEvent> EventRaised;

        private void Start()
        {
            if (autoPlay)
                Play();
        }

        private void OnDisable() { Stop(); }

        public void Play() { PlayAsync(story).Forget(); }

        public UniTask PlayAsync(StoryAsset Story, CancellationToken Token = default)
        {
            return PlayAsync(Story, view, Token);
        }

        /// <summary>
        /// 同一播放器或界面只允许一次播放 停止后须等待原调用结束才能重播
        /// </summary>
        public async UniTask PlayAsync(StoryAsset Story, DialogueView View, CancellationToken Token = default)
        {
            if (!isActiveAndEnabled || IsPlaying)
                throw new InvalidOperationException($"播放器 {name} 未启用或仍在播放");
            if (Story == null || View == null)
                throw new InvalidOperationException($"播放器 {name} 需要配置剧情和对话界面");
            Token.ThrowIfCancellationRequested();
            View.Acquire(this);
            var Cancellation = CancellationTokenSource.CreateLinkedTokenSource(Token, this.GetCancellationTokenOnDestroy());
            playbackCancellation = Cancellation;
            View.Disabled += Stop;
            try
            {
                Session = new StorySession(Story);
                View.Open(Story);
                await Session.RunAsync(View, RaiseEvent, Cancellation.Token);
            }
            finally
            {
                await UniTask.SwitchToMainThread();
                try
                {
                    if (View != null)
                        View.Close();
                }
                finally
                {
                    View.Disabled -= Stop;
                    View.Release();
                    playbackCancellation = null;
                    Cancellation.Dispose();
                }
            }
        }

        public void Stop() { playbackCancellation?.Cancel(); }

        private void RaiseEvent(StoryEvent Event)
        {
            var Handler = EventRaised;
            if (Handler == null)
                throw new InvalidOperationException($"播放器 {name} 的事件 {Event.EventId} 未绑定接收逻辑");
            Handler(Event);
        }
    }
}

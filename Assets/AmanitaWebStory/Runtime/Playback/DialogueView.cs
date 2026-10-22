using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AmanitaWebStory
{
    public abstract class DialogueView : MonoBehaviour, IStoryView
    {
        private StoryPlayer owner;

        public event Action Disabled;

        internal void Acquire(StoryPlayer Player)
        {
            if (!isActiveAndEnabled)
                throw new InvalidOperationException($"界面 {name} 必须处于启用状态");
            if (owner != null)
                throw new InvalidOperationException($"界面 {name} 已由播放器 {owner.name} 使用");
            owner = Player;
        }

        internal void Release() { owner = null; }

        protected virtual void OnDisable() { Disabled?.Invoke(); }

        public abstract void Open(StoryAsset Story);
        public abstract UniTask ShowLineAsync(StoryLine Line, CancellationToken Token);
        public abstract UniTask<int> ChooseAsync(IReadOnlyList<StoryOptionData> Options, CancellationToken Token);
        public abstract UniTask PlaySoundAsync(StorySound Sound, CancellationToken Token);
        public abstract void Close();
    }
}

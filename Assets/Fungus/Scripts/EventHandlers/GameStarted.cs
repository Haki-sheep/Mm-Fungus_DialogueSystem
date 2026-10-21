using Fungus.Inspector;
using Cysharp.Threading.Tasks;
using System.Threading;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// The block will execute when the game starts playing.
    /// </summary>
    [EventHandlerInfo("其他", "游戏启动", "在游戏启动时执行所属流程")]
    [AddComponentMenu("")]
    public class GameStarted : EventHandler
    {
        private readonly AsyncOperationScope asyncScope = new AsyncOperationScope();

        [Tooltip("设置本步骤使用的等待帧数")]
        [LabelText("等待帧数")]
        [SerializeField]
        protected int waitForFrames = 1;

        protected virtual void Start()
        {
            asyncScope.Run(this, Token => GameStartAsync(Token)).Forget();
        }

        protected virtual async UniTask GameStartAsync(CancellationToken CancellationToken = default)
        {
            int frameCount = waitForFrames;
            while (frameCount > 0)
            {
                await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, CancellationToken);
                frameCount--;
            }

            ExecuteBlock();
        }
        protected virtual void OnDisable()
        {
            asyncScope.CancelAll();
        }

    }
}

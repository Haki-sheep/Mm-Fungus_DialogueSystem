using Cysharp.Threading.Tasks;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// The block will execute when the Flowchart game object is enabled.
    /// </summary>
    [EventHandlerInfo("场景", "流程图启用", "在流程图启用时执行所属流程")]
    [AddComponentMenu("")]
    public class FlowchartEnabled : EventHandler
    {
        private readonly AsyncOperationScope asyncScope = new AsyncOperationScope();

        protected virtual void OnDisable()
        {
            asyncScope.CancelAll();
        }

        protected virtual void OnEnable()
        {
            // 下一帧再执行 确保其他组件已完成启用
            asyncScope.Run(this, async Token =>
            {
                await UniTask.NextFrame(cancellationToken: Token);
                DoEvent();
            }).Forget();
        }

        protected virtual void DoEvent()
        {
            ExecuteBlock();
        }
    }
}

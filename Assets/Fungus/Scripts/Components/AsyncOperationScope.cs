using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Fungus
{
    public sealed class AsyncOperationScope
    {
        private readonly List<CancellationTokenSource> operationList = new List<CancellationTokenSource>();

        public async UniTask Run(MonoBehaviour Owner, Func<CancellationToken, UniTask> Operation,
            CancellationToken CancellationToken = default)
        {
            var Source = CancellationTokenSource.CreateLinkedTokenSource(
                Owner.GetCancellationTokenOnDestroy(), CancellationToken);
            operationList.Add(Source);
            try
            {
                Source.Token.ThrowIfCancellationRequested();
                await Operation(Source.Token);
            }
            finally
            {
                operationList.Remove(Source);
                Source.Dispose();
            }
        }

        public void CancelAll()
        {
            while (operationList.Count > 0)
            {
                int Index = operationList.Count - 1;
                var Source = operationList[Index];
                operationList.RemoveAt(Index);
                Source.Cancel();
            }
        }

        public UniTask RunAfterDelay(MonoBehaviour Owner, float Seconds, Action OnComplete)
        {
            return Run(Owner, async Token =>
            {
                await UniTask.Delay(TimeSpan.FromSeconds(Seconds), cancellationToken: Token);
                OnComplete();
            });
        }
    }
}

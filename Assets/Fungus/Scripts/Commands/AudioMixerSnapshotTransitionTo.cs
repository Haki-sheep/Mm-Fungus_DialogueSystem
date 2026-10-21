using Fungus.Inspector;
using Cysharp.Threading.Tasks;
using System.Threading;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Call TransitionTo on given Snapshot.
    /// </summary>
    [CommandInfo("声音", "切换混音快照", "对配置的目标执行切换混音快照 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    public class AudioMixerSnapshotTransitionTo : Command
    {
        [LabelText("混音快照")]
        [SerializeField]
        protected AudioMixerSnapshotData snapshot;
        [LabelText("过渡时间（秒）")]
        [SerializeField]
        protected FloatData timeToReach;

        [Tooltip("勾选后等待本步骤完成再继续 不勾选则立即执行下一步")]
        [LabelText("等完成后再执行下一步")]
        [SerializeField]
        protected bool waitUntilFinished = false;

        public override void OnEnter()
        {
            snapshot.Value.TransitionTo(timeToReach.Value);

            if (waitUntilFinished)
            {
                asyncScope.Run(this, Token => WaitForTransitionAsync(Token)).Forget();
            }
            else
            {
                Continue();
            }
        }

        protected async UniTask WaitForTransitionAsync(CancellationToken CancellationToken = default)
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(timeToReach.Value), cancellationToken: CancellationToken);
            Continue();
        }

        public override string GetSummary()
        {
            if (snapshot.Value == null)
                return "请先设置混音快照";

            var retval =  snapshot.Value.name + " 在 " + AuthoringValueLabel.Format(timeToReach.Value);

            if (waitUntilFinished)
                retval += " 等待";

            return retval;
        }

        public override Color GetButtonColor()
        {
            return new Color32(242, 209, 176, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return snapshot.audioMixerSnapshotRef == variable ||
                timeToReach.floatRef == variable ||
                base.HasReference(variable);
        }
    }
}
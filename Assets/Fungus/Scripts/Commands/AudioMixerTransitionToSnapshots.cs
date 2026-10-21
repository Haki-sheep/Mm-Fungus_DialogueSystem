using Fungus.Inspector;
using Cysharp.Threading.Tasks;
using System.Threading;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace Fungus
{
    /// <summary>
    /// Calls the mix TransitionToSnapshots.
    /// </summary>
    [CommandInfo("声音", "混合多个音频快照", "对配置的目标执行混合多个音频快照 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    public class AudioMixerTransitionToSnapshots : Command, ICollectionCompatible
    {
        [LabelText("混音器")]
        [SerializeField]
        protected AudioMixerData mixer;
        [LabelText("混音快照列表")]
        [SerializeField]
        protected AudioMixerSnapshot[] snapShots;
        [LabelText("小数数组")]
        [SerializeField]
        protected float[] floatArr;

        [Tooltip("设置本步骤使用的小数集合")]
        [LabelText("小数集合")]
        [SerializeField]
        protected CollectionData floatCollection;

        [LabelText("过渡时间（秒）")]
        [SerializeField]
        protected FloatData timeToTransition;

        [Tooltip("勾选后等待本步骤完成再继续 不勾选则立即执行下一步")]
        [LabelText("等完成后再执行下一步")]
        [SerializeField]
        protected bool waitUntilFinished = false;

        public override void OnEnter()
        {
            if (floatCollection.Value != null)
            {
                System.Array.Resize(ref floatArr, snapShots.Length);
                floatCollection.Value.CopyTo(floatArr, 0);
            }

            mixer.Value.TransitionToSnapshots(snapShots, floatArr, timeToTransition.Value);

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
            await UniTask.Delay(System.TimeSpan.FromSeconds(timeToTransition.Value), cancellationToken: CancellationToken);
            Continue();
        }

        public override string GetSummary()
        {
            if (mixer.Value == null)
                return "请先设置混音器";

            var retval =  mixer.Value.name + " " + AuthoringValueLabel.Format(snapShots.Length) + " 在 " + AuthoringValueLabel.Format(timeToTransition.Value) + "s";

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
            return mixer.audioMixerRef == variable ||
                floatCollection.collectionRef == variable ||
                timeToTransition.floatRef == variable ||
                base.HasReference(variable);
        }

        public bool IsVarCompatibleWithCollection(Variable variableInQuestion, string compatibleWith)
        {
            if (compatibleWith == "floatCollection")
                return floatCollection.Value != null && floatCollection.Value.IsElementCompatible(variableInQuestion);
            else
                return true;
        }
    }
}
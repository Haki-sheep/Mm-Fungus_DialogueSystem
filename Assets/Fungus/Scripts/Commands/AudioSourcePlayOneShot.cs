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
    /// PlayOneShot with given clip on given source
    /// </summary>
    [CommandInfo("声音", "播放一次音效", "对配置的目标执行播放一次音效 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    public class AudioSourcePlayOneShot : Command
    {
        [LabelText("音频播放器")]
        [SerializeField]
        protected AudioSourceData audioSource;
        [LabelText("音频片段")]
        [SerializeField]
        protected AudioClipData audioClip;

        [Tooltip("设置本步骤使用的音量倍率")]
        [LabelText("音量倍率")]
        [SerializeField]
        protected FloatData volumeScale = new FloatData(1);

        [Tooltip("勾选后等待本步骤完成再继续 不勾选则立即执行下一步")]
        [LabelText("等完成后再执行下一步")]
        [SerializeField]
        protected bool waitUntilFinished = false;

        public override void OnEnter()
        {
            audioSource.Value.PlayOneShot(audioClip.Value, volumeScale.Value);

            if (waitUntilFinished)
            {
                asyncScope.Run(this, Token => WaitForClipLengthAsync(Token)).Forget();
            }
            else
            {
                Continue();
            }
        }

        protected async UniTask WaitForClipLengthAsync(CancellationToken CancellationToken = default)
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(audioClip.Value.length), cancellationToken: CancellationToken);
            Continue();
        }

        public override string GetSummary()
        {
            if (audioSource.Value == null)
                return "请先设置原始对象";

            if (audioClip.Value == null)
                return "请先设置音频片段";

            var retval = audioSource.Value.name + ": " + audioClip.Value.name + " @ " + AuthoringValueLabel.Format(volumeScale.Value);

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
            return audioSource.audioSourceRef == variable ||
                audioClip.audioClipRef == variable ||
                volumeScale.floatRef == variable ||
                base.HasReference(variable);
        }
    }
}
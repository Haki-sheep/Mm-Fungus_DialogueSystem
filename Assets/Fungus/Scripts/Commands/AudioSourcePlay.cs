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
    /// Play a source, optionaly setting the clip and delay when called.
    /// </summary>
    [CommandInfo("声音", "播放音频", "对配置的目标执行播放音频 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    public class AudioSourcePlay : Command
    {
        [LabelText("音频播放器")]
        [SerializeField]
        protected AudioSourceData audioSource;

        [Tooltip("设置本步骤使用的音频片段")]
        [LabelText("音频片段")]
        [SerializeField]
        protected AudioClipData audioClip;

        [Tooltip("设置本步骤使用的开始前等待（秒）")]
        [LabelText("开始前等待（秒）")]
        [SerializeField]
        protected FloatData delay = new FloatData(0);

        [Tooltip("设置本步骤使用的修改播放器循环设置")]
        [LabelText("修改播放器循环设置")]
        [SerializeField]
        protected bool modifySourceLooping = false;

        [Tooltip("设置本步骤使用的循环播放")]
        [LabelText("循环播放")]
        [SerializeField]
        protected bool loop = false;

        [Tooltip("勾选后等待本步骤完成再继续 不勾选则立即执行下一步")]
        [LabelText("等完成后再执行下一步")]
        [SerializeField]
        protected bool waitUntilFinished = false;

        public override void OnEnter()
        {
            if (audioClip.Value != null)
            {
                audioSource.Value.clip = audioClip.Value;
            }

            if (delay.Value != 0)
            {
                audioSource.Value.PlayDelayed(delay);
            }
            else
            {
                audioSource.Value.Play();
            }

            if(modifySourceLooping)
            {
                audioSource.Value.loop = loop;
            }

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
            await UniTask.Delay(System.TimeSpan.FromSeconds(audioSource.Value.clip.length), cancellationToken: CancellationToken);
            Continue();
        }

        public override string GetSummary()
        {
            if (audioSource.Value == null)
                return "请先设置原始对象";

            var retval = audioSource.Value.name;

            if (audioClip.Value != null)
                retval += ": " + audioClip.Value.name;

            if (delay.Value != 0)
                retval += ", " + AuthoringValueLabel.Format(delay.Value) + "s";

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
                delay.floatRef == variable ||
                base.HasReference(variable);
        }
    }
}
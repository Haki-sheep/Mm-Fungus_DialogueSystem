using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Sets the global volume level for audio played with Play Music and Play Sound commands.
    /// </summary>
    [CommandInfo("声音", "设置音量", "对配置的目标执行设置音量 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    public class SetAudioVolume : Command
    {
        [Range(0,1)]
        [Tooltip("设置本步骤使用的音量")]
        [LabelText("音量")]
        [SerializeField]
        protected float volume = 1f;

        [Range(0,30)]
        [Tooltip("设置本步骤使用的淡入淡出时间（秒）")]
        [LabelText("淡入淡出时间（秒）")]
        [SerializeField]
        protected float fadeDuration = 1f;

        [Tooltip("勾选后等待本步骤完成再继续 不勾选则立即执行下一步")]
        [LabelText("等完成后再执行下一步")]
        [SerializeField]
        protected bool waitUntilFinished = true;

        #region Public members

        public override void OnEnter()
        {
            var musicManager = FungusManager.Instance.MusicManager;

            musicManager.SetAudioVolume(volume, fadeDuration, () => {
                if (waitUntilFinished)
                {
                    Continue();
                }
            });

            if (!waitUntilFinished)
            {
                Continue();
            }
        }

        public override string GetSummary()
        {
            return "设置为" + volume + " 持续 " + fadeDuration + " 秒";
        }

        public override Color GetButtonColor()
        {
            return new Color32(242, 209, 176, 255);
        }

        #endregion
    }
}
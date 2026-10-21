using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Sets the global pitch level for audio played with Play Music and Play Sound commands.
    /// </summary>
    [CommandInfo("声音", "设置音高", "对配置的目标执行设置音高 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    public class SetAudioPitch : Command
    {
        [Range(0,1)]
        [Tooltip("设置本步骤使用的音高")]
        [LabelText("音高")]
        [SerializeField]
        protected float pitch = 1;

        [Range(0,30)]
        [Tooltip("设置本步骤使用的淡入淡出时间（秒）")]
        [LabelText("淡入淡出时间（秒）")]
        [SerializeField]
        protected float fadeDuration; 

        [Tooltip("勾选后等待本步骤完成再继续 不勾选则立即执行下一步")]
        [LabelText("等完成后再执行下一步")]
        [SerializeField]
        protected bool waitUntilFinished = true;

        #region Public members

        public override void OnEnter()
        {
            System.Action onComplete = () => {
                if (waitUntilFinished)
                {
                    Continue();
                }
            };

            var musicManager = FungusManager.Instance.MusicManager;

            musicManager.SetAudioPitch(pitch, fadeDuration, onComplete);

            if (!waitUntilFinished)
            {
                Continue();
            }
        }

        public override string GetSummary()
        {
            return "设置为" + pitch + " 持续 " + fadeDuration + " 秒";
        }

        public override Color GetButtonColor()
        {
            return new Color32(242, 209, 176, 255);
        }

        #endregion
    }
}
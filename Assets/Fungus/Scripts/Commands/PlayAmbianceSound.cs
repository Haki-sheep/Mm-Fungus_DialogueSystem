using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Plays a once-off sound effect. Multiple sound effects can be played at the same time.
    /// </summary>
    [CommandInfo("声音", "播放环境音", "播放持续的环境背景音")]
    [AddComponentMenu("")]
    public class PlayAmbianceSound : Command
    {
        [Tooltip("设置本步骤使用的音效片段")]
        [LabelText("音效片段")]
        [SerializeField]
        protected AudioClip soundClip;

        [Range(0, 1)]
        [Tooltip("设置本步骤使用的音量")]
        [LabelText("音量")]
        [SerializeField]
        protected float volume = 1;
        
        [Tooltip("设置本步骤使用的循环播放")]
        [LabelText("循环播放")]
        [SerializeField]
        protected bool loop;

        protected virtual void DoWait()
        {
            Continue();
        }

        #region Public members

        public override void OnEnter()
        {
            if (soundClip == null)
            {
                Continue();
                return;
            }

            var musicManager = FungusManager.Instance.MusicManager;

            musicManager.PlayAmbianceSound(soundClip, loop, volume);

            Continue();
        }

        public override string GetSummary()
        {
            if (soundClip == null)
            {
                return "请先设置音效片段";
            }

            return soundClip.name;
        }

        public override Color GetButtonColor()
        {
            return new Color32(242, 209, 176, 255);
        }

        #endregion
    }
}

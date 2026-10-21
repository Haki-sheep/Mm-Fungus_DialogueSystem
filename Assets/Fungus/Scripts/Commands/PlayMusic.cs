using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Plays looping game music. If any game music is already playing, it is stopped. Game music will continue playing across scene loads.
    /// </summary>
    [CommandInfo("声音", "播放音乐", "播放背景音乐 可配置循环和音量变化")]
    [AddComponentMenu("")]
    public class PlayMusic : Command
    {
        [Tooltip("设置本步骤使用的音乐片段")]
        [LabelText("音乐片段")]
        [SerializeField]
        protected AudioClip musicClip;

        [Tooltip("设置本步骤使用的播放位置（秒）")]
        [LabelText("播放位置（秒）")]
        [SerializeField]
        protected float atTime;

        [Tooltip("设置本步骤使用的循环播放")]
        [LabelText("循环播放")]
        [SerializeField]
        protected bool loop = true;
    
        [Tooltip("设置本步骤使用的淡入淡出时间（秒）")]
        [LabelText("淡入淡出时间（秒）")]
        [SerializeField]
        protected float fadeDuration = 1f;

        #region Public members

        public override void OnEnter()
        {
            var musicManager = FungusManager.Instance.MusicManager;

            float startTime = Mathf.Max(0, atTime);
            musicManager.PlayMusic(musicClip, loop, fadeDuration, startTime);
                
            Continue();
        }
                    
        public override string GetSummary()
        {
            if (musicClip == null)
            {
                return "请先设置音乐片段";
            }

            return musicClip.name;
        }

        public override Color GetButtonColor()
        {
            return new Color32(242, 209, 176, 255);
        }

        #endregion
    }
}
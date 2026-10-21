using Cysharp.Threading.Tasks;
using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Plays a once-off sound effect. Multiple sound effects can be played at the same time.
    /// </summary>
    [CommandInfo("声音", "随机播放音效", "对配置的目标执行随机播放音效 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    public class PlayRandomSound : Command
    {
        [Tooltip("设置本步骤使用的音效片段")]
        [LabelText("音效片段")]
        [SerializeField]
        protected AudioClip[] soundClip;

        [Range(0, 1)]
        [Tooltip("设置本步骤使用的音量")]
        [LabelText("音量")]
        [SerializeField]
        protected float volume = 1;

        [Tooltip("勾选后等待本步骤完成再继续 不勾选则立即执行下一步")]
        [LabelText("等完成后再执行下一步")]
        [SerializeField]
        protected bool waitUntilFinished;

        protected virtual void DoWait()
        {
            Continue();
        }

        #region Public members

        public override void OnEnter()
        {
            int rand = Random.Range(0, soundClip.Length);
            if (soundClip == null)
            {
                Continue();
                return;
            }

            var musicManager = FungusManager.Instance.MusicManager;

            musicManager.PlaySound(soundClip[rand], volume);

            if (waitUntilFinished)
            {
                asyncScope.RunAfterDelay(this, soundClip[rand].length, DoWait).Forget();
            }
            else
            {
                Continue();
            }
        }

        public override string GetSummary()
        {
            if (soundClip == null)
            {
                return "请先设置音效片段";
            }
            
            string sounds = "[";
            foreach (AudioClip ac in soundClip) {
				if(ac!=null)
					sounds+=ac.name+" ,";
            }
            sounds = sounds.TrimEnd(' ', ',');
            sounds += "]";
            return "随机音效"+sounds;
        }

        public override Color GetButtonColor()
        {
            return new Color32(242, 209, 176, 255);
        }

        #endregion
    }
}

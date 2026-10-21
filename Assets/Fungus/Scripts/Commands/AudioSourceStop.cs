// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Calls Stop on given source.
    /// </summary>
    [CommandInfo("声音", "停止音频播放器", "对配置的目标执行停止音频播放器 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    public class AudioSourceStop : AudioSourceBase
    {
        public override void OnEnter()
        {
            audioSource.Value.Stop();

            Continue();
        }
    }
}
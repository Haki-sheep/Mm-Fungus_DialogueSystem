// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Calls UnPause on given source.
    /// </summary>
    [CommandInfo("声音", "继续音频播放", "对配置的目标执行继续音频播放 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    public class AudioSourceUnPause : AudioSourceBase
    {
        public override void OnEnter()
        {
            audioSource.Value.UnPause();

            Continue();
        }
    }
}
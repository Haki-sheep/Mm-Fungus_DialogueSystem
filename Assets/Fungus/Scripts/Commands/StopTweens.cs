// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Stop all active iTweens in the current scene.
    /// </summary>
    [CommandInfo("旧版补间动画", "停止对象的补间动画", "对配置的目标执行停止对象的补间动画 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    public class StopTweens : Command
    {
        #region Public members

        public override void OnEnter()
        {
            FungusTween.StopAll();
            Continue();
        }

        #endregion
    }
}
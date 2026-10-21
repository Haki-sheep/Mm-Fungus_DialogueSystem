// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Deactivates swipe panning mode.
    /// </summary>
    [CommandInfo("镜头", "关闭拖动画面", "对配置的目标执行关闭拖动画面 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    public class StopSwipe : Command 
    {
        #region Public members

        public override void OnEnter()
        {
            var cameraManager = FungusManager.Instance.CameraManager;

            cameraManager.StopSwipePan();

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(216, 228, 170, 255);
        }

        #endregion
    }
}
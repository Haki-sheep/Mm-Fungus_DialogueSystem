using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Fullscreen mode options.
    /// </summary>
    public enum FullscreenMode
    {
        /// <summary> Toggle the current mode between fullscreen and windowed. </summary>
        [UnityEngine.InspectorName("切换状态")]
        Toggle,
        /// <summary> Switch to fullscreen mode. </summary>
        [UnityEngine.InspectorName("全屏模式")]
        Fullscreen,
        /// <summary> Switch to windowed mode. </summary>
        [UnityEngine.InspectorName("窗口模式")]
        Windowed
    }

    /// <summary>
    /// Sets the application to fullscreen, windowed or toggles the current state.
    /// </summary>
    [CommandInfo("镜头", "设置全屏", "对配置的目标执行设置全屏 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    public class Fullscreen : Command 
    {
        [LabelText("全屏方式")]
        [SerializeField]
        protected FullscreenMode fullscreenMode;

        #region Public members

        public override void OnEnter()
        {
            switch (fullscreenMode)
            {
            case FullscreenMode.Toggle:
                Screen.fullScreen = !Screen.fullScreen;
                break;
            case FullscreenMode.Fullscreen:
                Screen.fullScreen = true;
                break;
            case FullscreenMode.Windowed:
                Screen.fullScreen = false;
                break;
            }

            Continue();
        }

        public override string GetSummary()
        {
            return AuthoringValueLabel.Format(fullscreenMode);
        }

        public override Color GetButtonColor()
        {
            return new Color32(216, 228, 170, 255);
        }

        #endregion
    }
}
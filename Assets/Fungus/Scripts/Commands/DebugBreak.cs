using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Writes a log message to the debug console.
    /// </summary>
    [CommandInfo("脚本与事件", "暂停调试", "对配置的目标执行暂停调试 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    public class DebugBreak : Command
    {
        [LabelText("是否启用")]
        [SerializeField]
        private new protected BooleanData enabled = new BooleanData(true);

        public override void OnEnter()
        {
            if (enabled.Value)
                Debug.Break();

            Continue();
        }

        public override string GetSummary()
        {
            return enabled.Value ? "启用" : "禁用";
        }

        public override bool HasReference(Variable variable)
        {
            return variable == enabled.booleanRef;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }
    }
}
using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Resets the state of all commands and variables in the Flowchart.
    /// </summary>
    [CommandInfo("变量", "重置流程图", "按配置重置步骤记录及剧情变量")]
    [AddComponentMenu("")]
    public class Reset : Command
    {   
        [Tooltip("设置本步骤使用的重置步骤执行记录")]
        [LabelText("重置步骤执行记录")]
        [SerializeField]
        protected bool resetCommands = true;

        [Tooltip("设置本步骤使用的重置变量")]
        [LabelText("重置变量")]
        [SerializeField]
        protected bool resetVariables = true;

        #region Public members

        public override void OnEnter()
        {
            GetFlowchart().ResetFlowchart(resetCommands, resetVariables);
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        #endregion
    }
}
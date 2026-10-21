using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections.Generic;

namespace Fungus
{
    /// <summary>
    /// Stops execution of all Blocks in a Flowchart.
    /// </summary>
    [CommandInfo("流程控制", "停止流程图", "停止目标流程图中正在执行的流程")]
    [AddComponentMenu("")]
    public class StopFlowchart : Command
    {       
        [Tooltip("设置本步骤使用的同时停止当前流程图")]
        [LabelText("同时停止当前流程图")]
        [SerializeField]
        protected bool stopParentFlowchart;

        [Tooltip("设置本步骤使用的目标流程图列表")]
        [LabelText("目标流程图列表")]
        [SerializeField]
        protected List<Flowchart> targetFlowcharts = new List<Flowchart>();

        #region Public members

        public override void OnEnter()
        {
            var flowchart = GetFlowchart();

            for (int i = 0; i < targetFlowcharts.Count; i++)
            {
                var f = targetFlowcharts[i];
                f.StopAllBlocks();
            }

            //current block and command logic doesn't require it in this order but it makes sense to
            // stop everything but yourself first
            if (stopParentFlowchart)
            {
                flowchart.StopAllBlocks();
            }

            //you might not be stopping this flowchart so keep going
            Continue();
        }

        public override bool IsReorderableArray(string propertyName)
        {
            if (propertyName == "targetFlowcharts")
            {
                return true;
            }

            return false;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        #endregion
    }
}
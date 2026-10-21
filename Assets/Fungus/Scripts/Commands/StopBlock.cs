using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Stops executing the named Block.
    /// </summary>
    [CommandInfo("流程控制", "停止指定流程", "停止指定流程及其正在等待的异步步骤")]
    public class StopBlock : Command, IBlockCaller
    {
        [Tooltip("设置本步骤使用的剧情流程图")]
        [LabelText("剧情流程图")]
        [SerializeField]
        protected Flowchart flowchart;

        [Tooltip("给这段流程起一个便于查找的名字 同一张流程图内不能重名")]
        [LabelText("流程名称")]
        [SerializeField]
        protected StringData blockName = new StringData("");

        #region Public members

        public override void OnEnter()
        {
            if (blockName.Value == "")
            {
                Continue();
            }

            if (flowchart == null)
            {
                flowchart = (Flowchart)GetFlowchart();
            }

            var block = flowchart.FindBlock(blockName.Value);
            if (block == null ||
                !block.IsExecuting())
            {
                Continue();
            }

            block.Stop();

            Continue();
        }

        public override string GetSummary()
        {
            return blockName;
        }
            
        public override Color GetButtonColor()
        {
            return new Color32(253, 253, 150, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return blockName.stringRef == variable || base.HasReference(variable);
        }

        public bool MayCallBlock(Block block)
        {
            if(flowchart != null)
                return block == flowchart.FindBlock(blockName.Value);
            return false;
        }

        #endregion
    }
}
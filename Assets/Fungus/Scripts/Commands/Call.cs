using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;
using System;

namespace Fungus
{
    /// <summary>
    /// Supported modes for calling a block.
    /// </summary>
    public enum CallMode
    {
        /// <summary> Stop executing the current block after calling. </summary>
        [UnityEngine.InspectorName("调用目标后停止当前流程")]
        Stop,
        /// <summary> Continue executing the current block after calling  </summary>
        [UnityEngine.InspectorName("同时继续当前流程")]
        Continue,
        /// <summary> Wait until the called block finishes executing, then continue executing current block. </summary>
        [UnityEngine.InspectorName("等待目标结束")]
        WaitUntilFinished,
        /// <summary> Stop executing the current block before attempting to call. This allows for circular calls within the same frame </summary>
        [UnityEngine.InspectorName("先停止当前流程再调用")]
        StopThenCall
    }

    /// <summary>
    /// Execute another block in the same Flowchart as the command, or in a different Flowchart.
    /// </summary>
    [CommandInfo("流程控制", "调用流程", "执行另一个流程 可等待它结束 同时继续 或停止当前流程")]
    [AddComponentMenu("")]
    public class Call : Command, IBlockCaller
    {
        [Tooltip("设置本步骤使用的目标流程图")]
        [LabelText("目标流程图")]
        [SerializeField]
        protected Flowchart targetFlowchart;

        [FormerlySerializedAs("targetSequence")]
        [Tooltip("选择接下来要执行的流程节点")]
        [LabelText("目标流程")]
        [SerializeField]
        protected Block targetBlock;

        [Tooltip("从目标流程中的指定标记开始 留空时使用起始步骤下标")]
        [LabelText("起始跳转标记")]
        [SerializeField]
        protected StringData startLabel = new StringData();

        [Tooltip("设置本步骤使用的起始步骤下标")]
        [FormerlySerializedAs("commandIndex")]
        [LabelText("起始步骤下标")]
        [SerializeField]
        protected int startIndex;
    
        [Tooltip("选择调用目标后当前流程如何继续 可等待目标结束 同时继续 或停止当前流程")]
        [LabelText("后续执行方式")]
        [SerializeField]
        protected CallMode callMode;

        #region Public members

        public override void OnEnter()
        {
            if (targetBlock != null)
            {
                // Check if calling your own parent block
                if (ParentBlock != null && ParentBlock.Equals(targetBlock))
                {
                    // Just ignore the callmode in this case, and jump to first command in list
                    Continue(0);
                    return;
                }

                if(targetBlock.IsExecuting())
                {
                    Debug.LogWarning(targetBlock.BlockName + " cannot be called/executed, it is already running.");
                    Continue();
                    return;
                }

                // Callback action for Wait Until Finished mode
                Action onComplete = null;
                if (callMode == CallMode.WaitUntilFinished)
                {
                    onComplete = delegate {
                        Continue();
                    };
                }

                // Find the command index to start execution at
                int index = startIndex;
                if (startLabel.Value != "")
                {
                    int labelIndex = targetBlock.GetLabelIndex(startLabel.Value);
                    if (labelIndex != -1)
                    {
                        index = labelIndex;
                    }
                }

                if (targetFlowchart == null ||
                    targetFlowchart.Equals(GetFlowchart()))
                {
                    if (callMode == CallMode.StopThenCall)
                    {
                        StopParentBlock();
                    }
                    targetBlock.StartExecution(index, onComplete);
                }
                else
                {
                    if (callMode == CallMode.StopThenCall)
                    {
                        StopParentBlock();
                    }
                    // Execute block in another Flowchart
                    targetFlowchart.ExecuteBlock(targetBlock, index, onComplete);
                }
            }

            if (callMode == CallMode.Stop)
            {
                StopParentBlock();
            }
            else if (callMode == CallMode.Continue)
            {
                Continue();
            }
        }

        public override void GetConnectedBlocks(ref List<Block> connectedBlocks)
        {
            if (targetBlock != null)
            {
                connectedBlocks.Add(targetBlock);
            }       
        }
        
        public override string GetSummary()
        {
            string summary = "";

            if (targetBlock == null)
            {
                summary = "未设置";
            }
            else
            {
                summary = targetBlock.BlockName;
            }

            summary += " : " + AuthoringValueLabel.Format(callMode);

            return summary;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return startLabel.stringRef == variable || base.HasReference(variable);
        }

        public bool MayCallBlock(Block block)
        {
            return block == targetBlock;
        }

        #endregion
    }
}

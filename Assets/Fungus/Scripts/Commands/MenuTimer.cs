using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;

namespace Fungus
{
    /// <summary>
    /// Displays a timer bar and executes a target block if the player fails to select a menu option in time.
    /// </summary>
    [CommandInfo("剧情对白", "限时选择", "开始选项倒计时 到时仍未选择就执行超时流程")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class MenuTimer : Command, IBlockCaller
    {
        [Tooltip("设置效果持续多久 单位为秒")]
        [LabelText("持续时间（秒）")]
        [SerializeField]
        protected FloatData _duration = new FloatData(1);

        [FormerlySerializedAs("targetSequence")]
        [Tooltip("选择接下来要执行的流程节点")]
        [LabelText("目标流程")]
        [SerializeField]
        protected Block targetBlock;

        #region Public members

        public override void OnEnter()
        {
            var menuDialog = MenuDialog.GetMenuDialog();

            if (menuDialog != null &&
                targetBlock != null)
            {
                menuDialog.ShowTimer(_duration.Value, targetBlock);
            }

            Continue();
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
            if (targetBlock == null)
            {
                return "请先设置目标流程";
            }

            return targetBlock.BlockName;
        }

        public override Color GetButtonColor()
        {
            return new Color32(184, 210, 235, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return _duration.floatRef == variable ||
                base.HasReference(variable);
        }

        public bool MayCallBlock(Block block)
        {
            return block == targetBlock;
        }
        
        #endregion

        #region Backwards compatibility

        [HideInInspector] [FormerlySerializedAs("duration")] public float durationOLD;

        protected virtual void OnEnable()
        {
            if (durationOLD != default(float))
            {
                _duration.Value = durationOLD;
                durationOLD = default(float);
            }
        }

        #endregion
    }
}
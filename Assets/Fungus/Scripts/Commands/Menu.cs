using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;

namespace Fungus
{
    /// <summary>
    /// Displays a button in a multiple choice menu.
    /// </summary>
    [CommandInfo("剧情对白", "添加对话选项", "添加一个可选择的选项 玩家点击后执行目标流程")]
    [AddComponentMenu("")]
    public class Menu : Command, ILocalizable, IBlockCaller
    {
        [Tooltip("设置本步骤使用的文字内容")]
        [TextArea()]
        [LabelText("文字内容")]
        [SerializeField]
        protected string text = "Option Text";

        [Tooltip("填写这段流程或对白的用途 只用于编辑时说明")]
        [LabelText("用途说明")]
        [SerializeField]
        protected string description = "";

        [FormerlySerializedAs("targetSequence")]
        [Tooltip("选择接下来要执行的流程节点")]
        [LabelText("目标流程")]
        [SerializeField]
        protected Block targetBlock;

        [Tooltip("玩家选过这个选项后 后续不再显示它")]
        [LabelText("选过后隐藏")]
        [SerializeField]
        protected bool hideIfVisited;

        [Tooltip("设置本步骤使用的允许选择")]
        [LabelText("允许选择")]
        [SerializeField]
        protected BooleanData interactable = new BooleanData(true);

        [Tooltip("设置本步骤使用的使用的选项界面")]
        [LabelText("使用的选项界面")]
        [SerializeField]
        protected MenuDialog setMenuDialog;

        [Tooltip("隐藏这个选项 但保留它在选项列表中的位置")]
        [LabelText("隐藏此选项")]
        [SerializeField]
        protected BooleanData hideThisOption = new BooleanData(false);

        #region Public members

        public MenuDialog SetMenuDialog  { get { return setMenuDialog; } set { setMenuDialog = value; } }

        public override void OnEnter()
        {
            if (setMenuDialog != null)
            {
                // Override the active menu dialog
                MenuDialog.ActiveMenuDialog = setMenuDialog;
            }

            bool hideOption = (hideIfVisited && targetBlock != null && targetBlock.GetExecutionCount() > 0) || hideThisOption.Value;

            var menuDialog = MenuDialog.GetMenuDialog();
                if (menuDialog != null)
                {
                    menuDialog.SetActive(true);

                    var flowchart = GetFlowchart();
                    string displayText = flowchart.SubstituteVariables(text);

                    menuDialog.AddOption(displayText, interactable, hideOption, targetBlock);
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

            if (text == "")
            {
                return "请先设置按钮文字";
            }

            return text + " : " + targetBlock.BlockName;
        }

        public override Color GetButtonColor()
        {
            return new Color32(184, 210, 235, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return interactable.booleanRef == variable || hideThisOption.booleanRef == variable ||
                base.HasReference(variable);
        }

        public bool MayCallBlock(Block block)
        {
            return block == targetBlock;
        }

        #endregion

        #region ILocalizable implementation

        public virtual string GetStandardText()
        {
            return text;
        }

        public virtual void SetStandardText(string standardText)
        {
            text = standardText;
        }
        
        public virtual string GetDescription()
        {
            return description;
        }
        
        public virtual string GetStringId()
        {
            // String id for Menu commands is MENU.<Localization Id>.<Command id>
            return "MENU." + GetFlowchartLocalizationId() + "." + itemId;
        }

        #endregion

        #region Editor caches
#if UNITY_EDITOR
        protected override void RefreshVariableCache()
        {
            base.RefreshVariableCache();

            var f = GetFlowchart();

            f.DetermineSubstituteVariables(text, referencedVariables);
        }
#endif
        #endregion Editor caches
    }
}
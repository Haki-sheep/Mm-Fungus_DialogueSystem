using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;

namespace Fungus
{
    /// <summary>
    /// Move execution to a specific Label command in the same block.
    /// </summary>
    [CommandInfo("流程控制", "跳到标记", "跳到当前流程内指定的标记")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class Jump : Command
    {
        [Tooltip("设置本步骤使用的目标跳转标记")]
        [LabelText("目标跳转标记")]
        [SerializeField]
        protected StringData _targetLabel = new StringData("");

        #region Public members

        public override void OnEnter()
        {
            if (_targetLabel.Value == "")
            {
                Continue();
                return;
            }

            var commandList = ParentBlock.CommandList;
            for (int i = 0; i < commandList.Count; i++)
            {
                var command = commandList[i];
                Label label = command as Label;
                if (label != null && label.Key == _targetLabel.Value)
                {
                    Continue(label.CommandIndex + 1);
                    return;
                }
            }

            // Label not found
            Debug.LogWarning("Label not found: " + _targetLabel.Value);
            Continue();
        }

        public override string GetSummary()
        {
            if (_targetLabel.Value == "")
            {
                return "请先设置跳转标记";
            }

            return _targetLabel.Value;
        }

        public override Color GetButtonColor()
        {
            return new Color32(253, 253, 150, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return _targetLabel.stringRef == variable ||
                base.HasReference(variable);
        }

        #endregion

        #region Backwards compatibility

        [HideInInspector] [FormerlySerializedAs("targetLabel")] public Label targetLabelOLD;

        protected virtual void OnEnable()
        {
            if (targetLabelOLD != null)
            {
                _targetLabel.Value = targetLabelOLD.Key;
                targetLabelOLD = null;
            }
        }

        #endregion
    }
}
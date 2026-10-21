using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Loop over a fixed integer range, similar to a common for loop.
    /// </summary>
    [CommandInfo("流程控制", "按范围循环", "按指定数值范围重复执行步骤")]
    [CommandInfo("流程控制", "计数循环", "按起始值 结束值和递增量重复执行步骤")]
    [AddComponentMenu("")]
    public class LoopRange : Condition
    {
        [Tooltip("设置本步骤使用的起始值")]
        [LabelText("起始值")]
        [SerializeField]
        protected IntegerData startingValue;

        [Tooltip("设置本步骤使用的结束值")]
        [LabelText("结束值")]
        [SerializeField]
        protected IntegerData endValue;

        [Tooltip("设置本步骤使用的循环计数")]
        [LabelText("循环计数")]
        [SerializeField]
        protected IntegerData counter;

        [Tooltip("设置本步骤使用的递增量")]
        [LabelText("递增量")]
        [SerializeField]
        protected IntegerData step = new IntegerData(1);

        #region Public members

        public override bool IsLooping { get { return true; } }

        protected override void PreEvaluate()
        {
            //if we came from the end then we are already looping, if not this is first loop so prep
            if (ParentBlock.PreviousActiveCommandIndex != endCommand.CommandIndex)
            {
                counter.Value = startingValue.Value;
            }
            else
            {
                counter.Value += (startingValue.Value <= endValue.Value ? step.Value : -step.Value);
            }
        }

        protected override bool EvaluateCondition()
        {
            return (startingValue.Value <= endValue.Value ?
                   counter.Value < endValue.Value :
                   counter.Value > endValue.Value);
        }

        protected override void OnFalse()
        {
            MoveToEnd();
        }

        public override void OnValidate()
        {
            // no infinite loops
            if (step.Value == 0)
                step.Value = 1;

            //no negative steps, we do that automatically
            step.Value = Mathf.Abs(step.Value);
        }

        public override bool HasReference(Variable variable)
        {
            return startingValue.integerRef == variable || endValue.integerRef == variable ||
                counter.integerRef == variable || step.integerRef == variable ||
                base.HasReference(variable);
        }

        #endregion
    }
}
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Marks the start of a command block to be executed when the preceding If statement is False and the test expression is true.
    /// </summary>
    [CommandInfo("流程控制", "否则如果", "前面的条件不成立时 再检查这里的条件")]
    [AddComponentMenu("")]
    public class ElseIf : VariableCondition
    {
        protected override bool IsElseIf { get { return true; } }

        #region Public members

        public override bool CloseBlock()
        {
            return true;
        }

        #endregion
    }
}
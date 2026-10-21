// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// If the test expression is true, execute the following command block.
    /// </summary>
    [CommandInfo("流程控制", "如果满足条件", "条件成立时执行下面缩进的步骤")]
    [AddComponentMenu("")]
    public class If : VariableCondition
    {
    }
}
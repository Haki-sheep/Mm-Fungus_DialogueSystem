// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Stop executing the Block that contains this command.
    /// </summary>
    [CommandInfo("流程控制", "停止当前流程", "立即结束当前流程")]
    [AddComponentMenu("")]
    public class Stop : Command
    {
        #region Public members

        public override void OnEnter()
        {
            StopParentBlock();
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        #endregion
    }
}
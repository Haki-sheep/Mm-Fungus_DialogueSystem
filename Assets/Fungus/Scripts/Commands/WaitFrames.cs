using Fungus.Inspector;
using Cysharp.Threading.Tasks;
using System.Threading;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// Waits for a number of frames before executing the next command in the block.
    /// </summary>
    [CommandInfo("流程控制", "等待指定帧数", "等待指定帧数后执行下一步")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class WaitFrames : Command
    {
        [Tooltip("设置本步骤使用的等待帧数")]
        [LabelText("等待帧数")]
        [SerializeField]
        protected IntegerData frameCount = new IntegerData(1);

        protected virtual async UniTask WaitForFramesAsync(CancellationToken CancellationToken = default)
        {
            int count = frameCount.Value;
            while (count > 0)
            {
                await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, CancellationToken);
                count--;
            }

            Continue();
        }

        #region Public members

        public override void OnEnter()
        {
            asyncScope.Run(this, Token => WaitForFramesAsync(Token)).Forget();
        }

        public override string GetSummary()
        {
            return AuthoringValueLabel.Format(frameCount.Value) + " 帧";
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return frameCount.integerRef == variable || base.HasReference(variable);
        }

        #endregion
    }
}
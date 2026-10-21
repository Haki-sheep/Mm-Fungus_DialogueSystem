using Cysharp.Threading.Tasks;
using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;

namespace Fungus
{
    /// <summary>
    /// Waits for period of time before executing the next command in the block.
    /// </summary>
    [CommandInfo("流程控制", "等待一段时间", "等待指定秒数后执行下一步 受游戏时间缩放影响")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class Wait : Command
    {
        [Tooltip("设置效果持续多久 单位为秒")]
        [LabelText("持续时间（秒）")]
        [SerializeField]
        protected FloatData _duration = new FloatData(1);

        protected virtual void OnWaitComplete()
        {
            Continue();
        }

        #region Public members

        public override void OnEnter()
        {
            asyncScope.RunAfterDelay(this, _duration.Value, OnWaitComplete).Forget();
        }

        public override string GetSummary()
        {
            return AuthoringValueLabel.Format(_duration.Value) + " 秒";
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return _duration.floatRef == variable || base.HasReference(variable);
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
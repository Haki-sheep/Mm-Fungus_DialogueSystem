using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Type of log message. Maps directly to Unity's log types.
    /// </summary>
    public enum DebugLogType
    {
        /// <summary> Informative log message. </summary>
        [UnityEngine.InspectorName("提示")]
        Info,
        /// <summary> Warning log message. </summary>
        [UnityEngine.InspectorName("警告")]
        Warning,
        /// <summary> Error log message. </summary>
        [UnityEngine.InspectorName("错误")]
        Error
    }

    /// <summary>
    /// Writes a log message to the debug console.
    /// </summary>
    [CommandInfo("脚本与事件", "输出日志", "对配置的目标执行输出日志 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    public class DebugLog : Command 
    {
        [Tooltip("设置本步骤使用的日志级别")]
        [LabelText("日志级别")]
        [SerializeField]
        protected DebugLogType logType;

        [Tooltip("设置本步骤使用的日志内容")]
        [LabelText("日志内容")]
        [SerializeField]
        protected StringDataMulti logMessage;

        #region Public members

        public override void OnEnter ()
        {
            var flowchart = GetFlowchart();
            string message = flowchart.SubstituteVariables(logMessage.Value);

            switch (logType)
            {
            case DebugLogType.Info:
                Debug.Log(message);
                break;
            case DebugLogType.Warning:
                Debug.LogWarning(message);
                break;
            case DebugLogType.Error:
                Debug.LogError(message);
                break;
            }

            Continue();
        }

        public override string GetSummary()
        {
            return logMessage.GetDescription();
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return logMessage.stringRef == variable || base.HasReference(variable);
        }

        #endregion

        #region Editor caches
#if UNITY_EDITOR
        protected override void RefreshVariableCache()
        {
            base.RefreshVariableCache();

            var f = GetFlowchart();

            f.DetermineSubstituteVariables(logMessage.Value, referencedVariables);
        }
#endif
        #endregion Editor caches
    }
}
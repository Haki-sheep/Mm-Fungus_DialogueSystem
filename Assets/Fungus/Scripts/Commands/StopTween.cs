using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;

namespace Fungus
{
    /// <summary>
    /// Stops an active iTween by name.
    /// </summary>
    [CommandInfo("旧版补间动画", "停止指定补间动画", "对配置的目标执行停止指定补间动画 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class StopTween : Command
    {
        [Tooltip("设置本步骤使用的动画名称")]
        [LabelText("动画名称")]
        [SerializeField]
        protected StringData _tweenName;

        #region Public members

        public override void OnEnter()
        {
            FungusTween.StopAll(_tweenName.Value);
            Continue();
        }

        public override bool HasReference(Variable variable)
        {
            return _tweenName.stringRef == variable || base.HasReference(variable);
        }

        #endregion

        #region Backwards compatibility

        [HideInInspector] [FormerlySerializedAs("tweenName")] public string tweenNameOLD = "";

        protected virtual void OnEnable()
        {
            if (tweenNameOLD != "")
            {
                _tweenName.Value = tweenNameOLD;
                tweenNameOLD = "";
            }
        }

        #endregion
    }
}
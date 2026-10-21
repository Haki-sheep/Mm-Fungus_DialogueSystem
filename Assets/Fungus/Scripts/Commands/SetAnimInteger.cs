using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;

namespace Fungus
{
    /// <summary>
    /// Sets an integer parameter on an Animator component to control a Unity animation.
    /// </summary>
    [CommandInfo("动画", "设置动画整数参数", "对配置的目标执行设置动画整数参数 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class SetAnimInteger : Command
    {
        [Tooltip("设置本步骤使用的动画控制器")]
        [LabelText("动画控制器")]
        [SerializeField]
        protected AnimatorData _animator;

        [Tooltip("设置本步骤使用的参数名称")]
        [LabelText("参数名称")]
        [SerializeField]
        protected StringData _parameterName;

        [Tooltip("设置本步骤使用的数值")]
        [LabelText("数值")]
        [SerializeField]
        protected IntegerData value;

        #region Public members

        public override void OnEnter()
        {
            if (_animator.Value != null)
            {
                _animator.Value.SetInteger(_parameterName.Value, value.Value);
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (_animator.Value == null)
            {
                return "请先设置动画控制器";
            }

            return _animator.Value.name + " (" + _parameterName.Value + ")";
        }

        public override Color GetButtonColor()
        {
            return new Color32(170, 204, 169, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return _animator.animatorRef == variable || _parameterName.stringRef == variable || value.integerRef == variable || 
                base.HasReference(variable);
        }

        #endregion

        #region Backwards compatibility

        [HideInInspector] [FormerlySerializedAs("animator")] public Animator animatorOLD;
        [HideInInspector] [FormerlySerializedAs("parameterName")] public string parameterNameOLD = "";

        protected virtual void OnEnable()
        {
            if (animatorOLD != null)
            {
                _animator.Value = animatorOLD;
                animatorOLD = null;
            }

            if (parameterNameOLD != "")
            {
                _parameterName.Value = parameterNameOLD;
                parameterNameOLD = "";
            }
        }

        #endregion
    }
}
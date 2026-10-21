using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Set or get a named float on a mixer
    /// </summary>
    [CommandInfo("声音", "设置混音参数", "对配置的目标执行设置混音参数 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    public class AudioMixerFloat : Command
    {
        [LabelText("读取或写入")]
        [SerializeField]
        protected BaseVariableProperty.GetSet getOrSet;
        [LabelText("混音器")]
        [SerializeField]
        protected AudioMixerData mixer;
        [LabelText("参数名称")]
        [SerializeField]
        protected StringData paramName;
        [LabelText("数值")]
        [SerializeField]
        protected FloatData val;

        public override void OnEnter()
        {
            switch (getOrSet)
            {
            case BaseVariableProperty.GetSet.Get:
                mixer.Value.GetFloat(paramName.Value, out var f);
                val.Value = f;
                break;
            case BaseVariableProperty.GetSet.Set:
                mixer.Value.SetFloat(paramName.Value, val.Value);
                break;
            default:
                break;
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (mixer.Value == null)
                return "请先设置混音器";
            
            switch (getOrSet)
            {
            case BaseVariableProperty.GetSet.Get:
                if (val.floatRef == null)
                    return "请先设置小数变量引用";
                else
                    return val.floatRef.Key + "=" + mixer.Value.name + "." + paramName.Value;
            case BaseVariableProperty.GetSet.Set:
                return mixer.Value.name + "." + paramName.Value + "=" + AuthoringValueLabel.Format(val.Value);
            default:
                break;
            }
            return "配置错误：未知错误";
        }

        public override Color GetButtonColor()
        {
            return new Color32(242, 209, 176, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return mixer.audioMixerRef == variable ||
                paramName.stringRef == variable ||
                val.floatRef == variable ||
                base.HasReference(variable);
        }
    }
}
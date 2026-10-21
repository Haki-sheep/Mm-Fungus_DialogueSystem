using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Stores the result of a ToString on given variable in a string.
    /// </summary>
    [CommandInfo("变量", "转为文字", "对配置的目标执行转为文字 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    public class ToString : Command
    {
        [Tooltip("设置本步骤使用的使用的变量")]
        [VariableProperty()]
        [LabelText("使用的变量")]
        [SerializeField]
        protected Variable variable;

        [Tooltip("设置本步骤使用的输出值")]
        [VariableProperty(typeof(StringVariable))]
        [LabelText("输出值")]
        [SerializeField]
        protected StringVariable outValue;

        //[Tooltip("设置本步骤使用的文字格式")]
        //[LabelText("文字格式")]
        [SerializeField]
        [LabelText("文字格式")]
        protected StringData format;

        public override void OnEnter()
        {
            if (variable != null && outValue != null)
            {
                outValue.Value = variable.ToString();
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (variable == null)
            {
                return "请先设置变量";
            }

            if (outValue == null)
            {
                return "请先设置输出变量";
            }

            return outValue.Key + " = " + variable.Key + " 转为文字";
        }

        public override bool HasReference(Variable variable)
        {
            return (variable == this.variable) || outValue == variable;
        }

        public override Color GetButtonColor()
        {
            return new Color32(253, 253, 150, 255);
        }
    }
}
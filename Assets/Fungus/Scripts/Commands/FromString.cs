using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Attempts to parse a string into a given fungus variable type, such as integer or float
    /// </summary>
    [CommandInfo("变量", "解析文字为数值", "对配置的目标执行解析文字为数值 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    public class FromString : Command
    {
        [Tooltip("设置本步骤使用的原始文字")]
        [VariableProperty(typeof(StringVariable))]
        [LabelText("原始文字")]
        [SerializeField]
        protected StringVariable sourceString;

        [Tooltip("设置本步骤使用的输出值")]
        [VariableProperty(typeof(IntegerVariable), typeof(FloatVariable))]
        [LabelText("输出值")]
        [SerializeField]
        protected Variable outValue;

        public override void OnEnter()
        {
            if (sourceString != null && outValue != null)
            {
                double asDouble = System.Convert.ToDouble(sourceString.Value, System.Globalization.CultureInfo.CurrentCulture);

                IntegerVariable intOutVar = outValue as IntegerVariable;
                if (intOutVar != null)
                {
                    intOutVar.Value = (int)asDouble;
                }
                else
                {
                    FloatVariable floatOutVar = outValue as FloatVariable;
                    if (floatOutVar != null)
                    {
                        floatOutVar.Value = (float)asDouble;
                    }
                }
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (sourceString == null)
            {
                return "请先设置原始文字";
            }

            if (outValue == null)
            {
                return "请先设置类型和接收变量";
            }

            return outValue.Key + " 解析 " + sourceString.Key;
        }

        public override bool HasReference(Variable variable)
        {
            return (variable == sourceString) || (variable == outValue);
        }

        public override Color GetButtonColor()
        {
            return new Color32(253, 253, 150, 255);
        }
    }
}
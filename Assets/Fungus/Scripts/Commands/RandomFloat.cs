using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Sets an float variable to a random value in the defined range.
    /// </summary>
    [CommandInfo("变量", "生成随机小数", "对配置的目标执行生成随机小数 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    public class RandomFloat : Command 
    {
        [Tooltip("设置本步骤使用的使用的变量")]
        [VariableProperty(typeof(FloatVariable))]
        [LabelText("使用的变量")]
        [SerializeField]
        protected FloatVariable variable;

        [Tooltip("设置本步骤使用的最小值")]
        [LabelText("最小值")]
        [SerializeField]
        protected FloatData minValue;

        [Tooltip("设置本步骤使用的最大值")]
        [LabelText("最大值")]
        [SerializeField]
        protected FloatData maxValue;

        #region Public members

        public override void OnEnter()
        {
            if (variable != null)
            {
                variable.Value = Random.Range(minValue.Value, maxValue.Value);
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (variable == null)
            {
                return "请先设置变量";
            }

            return variable.Key;
        }

        public override bool HasReference(Variable variable)
        {
            return (variable == this.variable) || minValue.floatRef == variable || maxValue.floatRef == variable;
        }

        public override Color GetButtonColor()
        {
            return new Color32(253, 253, 150, 255);
        }

        #endregion
    }
}
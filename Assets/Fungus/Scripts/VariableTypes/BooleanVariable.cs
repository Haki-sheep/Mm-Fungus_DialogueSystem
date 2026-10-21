using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Boolean variable type.
    /// </summary>
    [VariableInfo("其他", "Boolean", DisplayName = "布尔值")]
    [AddComponentMenu("")]
    [System.Serializable]
    public class BooleanVariable : VariableBase<bool>
    {
        public override bool IsArithmeticSupported(SetOperator setOperator)
        {
            return setOperator == SetOperator.Negate || base.IsArithmeticSupported(setOperator);
        }

        public override void Apply(SetOperator op, bool value)
        {
            switch (op)
            {
            case SetOperator.Negate:
                Value = !value;
                break;
            default:
                base.Apply(op, value);
                break;
            }
        }
    }

    /// <summary>
    /// Container for a Boolean variable reference or constant value.
    /// </summary>
    [System.Serializable]
    public struct BooleanData
    {
        [SerializeField]
        [VariableProperty("直接填值", typeof(BooleanVariable))]
        [LabelText("布尔值变量")]
        public BooleanVariable booleanRef;

        [LabelText("布尔值")]
        [SerializeField]
        public bool booleanVal;

        public BooleanData(bool v)
        {
            booleanVal = v;
            booleanRef = null;
        }
        
        public static implicit operator bool(BooleanData booleanData)
        {
            return booleanData.Value;
        }

        public bool Value
        {
            get { return (booleanRef == null) ? booleanVal : booleanRef.Value; }
            set { if (booleanRef == null) { booleanVal = value; } else { booleanRef.Value = value; } }
        }

        public string GetDescription()
        {
            if (booleanRef == null)
            {
                return booleanVal.ToString();
            }
            else
            {
                return booleanRef.Key;
            }
        }
    }
}
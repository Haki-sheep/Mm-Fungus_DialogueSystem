using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// Vector3 variable type.
    /// </summary>
    [VariableInfo("其他类型", "Vector3", DisplayName = "三维向量")]
    [AddComponentMenu("")]
    [System.Serializable]
    public class Vector3Variable : VariableBase<Vector3>
    {
        public override bool IsArithmeticSupported(SetOperator setOperator)
        {
            return true;
        }

        public override void Apply(SetOperator setOperator, Vector3 value)
        {
            Vector3 local = Value;

            switch (setOperator)
            {
            case SetOperator.Negate:
                Value = Value * -1;
                break;
            case SetOperator.Add:
                Value += value;
                break;
            case SetOperator.Subtract:
                Value -= value;
                break;
            case SetOperator.Multiply:
                local.Scale(value);
                Value = local;
                break;
            case SetOperator.Divide:
                local.Scale(new Vector3(1.0f / value.x, 1.0f / value.y, 1.0f / value.z));
                Value = local;
                break;
            default:
                base.Apply(setOperator, value);
                break;
            }
        }
    }

    /// <summary>
    /// Container for a Vector3 variable reference or constant value.
    /// </summary>
    [System.Serializable]
    public struct Vector3Data
    {
        [SerializeField]
        [VariableProperty("直接填值", typeof(Vector3Variable))]
        [LabelText("三维向量变量")]
        public Vector3Variable vector3Ref;
        
        [LabelText("三维向量值")]
        [SerializeField]
        public Vector3 vector3Val;

        public Vector3Data(Vector3 v)
        {
            vector3Val = v;
            vector3Ref = null;
        }
        
        public static implicit operator Vector3(Vector3Data vector3Data)
        {
            return vector3Data.Value;
        }

        public Vector3 Value
        {
            get { return (vector3Ref == null) ? vector3Val : vector3Ref.Value; }
            set { if (vector3Ref == null) { vector3Val = value; } else { vector3Ref.Value = value; } }
        }

        public string GetDescription()
        {
            if (vector3Ref == null)
            {
                return vector3Val.ToString();
            }
            else
            {
                return vector3Ref.Key;
            }
        }
    }
}
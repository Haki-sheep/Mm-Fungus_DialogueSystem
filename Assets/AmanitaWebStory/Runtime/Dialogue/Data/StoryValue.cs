using System;
using System.Globalization;
using UnityEngine;

namespace AmanitaWebStory
{
    public enum eStoryDataType { Integer, Float, Boolean, String, None }
    public enum eStoryAssignment { Assign, Add, Subtract }
    public enum eStoryComparison { Equal, NotEqual, Greater, GreaterOrEqual, Less, LessOrEqual }

    [Serializable]
    public struct StoryValue
    {
        [SerializeField]
        private eStoryDataType type;
        [SerializeField]
        private int integer;
        [SerializeField]
        private float number;
        [SerializeField]
        private bool boolean;
        [SerializeField]
        private string text;

        public eStoryDataType Type => type;
        public int Integer => type == eStoryDataType.Integer ? integer : throw WrongType();
        public float Float => type == eStoryDataType.Float ? number : throw WrongType();
        public bool Boolean => type == eStoryDataType.Boolean ? boolean : throw WrongType();
        public string String => type == eStoryDataType.String ? text : throw WrongType();

        public static StoryValue Parse(eStoryDataType Type, string Text)
        {
            var Value = new StoryValue { type = Type };
            switch (Type)
            {
                case eStoryDataType.Integer: Value.integer = int.Parse(Text, CultureInfo.InvariantCulture); break;
                case eStoryDataType.Float: Value.number = float.Parse(Text, CultureInfo.InvariantCulture); break;
                case eStoryDataType.Boolean: Value.boolean = bool.Parse(Text); break;
                case eStoryDataType.String: Value.text = Text; break;
                case eStoryDataType.None: break;
                default: throw new ArgumentOutOfRangeException(nameof(Type));
            }
            return Value;
        }

        public StoryValue Apply(eStoryAssignment Operation, StoryValue Operand)
        {
            RequireSameType(Operand);
            if (Operation == eStoryAssignment.Assign)
                return Operand;
            if (Operation != eStoryAssignment.Add && Operation != eStoryAssignment.Subtract)
                throw new ArgumentOutOfRangeException(nameof(Operation));
            var Result = this;
            switch (type)
            {
                case eStoryDataType.Integer:
                    Result.integer = checked(Operation == eStoryAssignment.Add ? integer + Operand.integer : integer - Operand.integer);
                    break;
                case eStoryDataType.Float:
                    Result.number = Operation == eStoryAssignment.Add ? number + Operand.number : number - Operand.number;
                    break;
                default: throw new InvalidOperationException($"{type} 不支持 {Operation}");
            }
            return Result;
        }

        public bool Compare(eStoryComparison Comparison, StoryValue Operand)
        {
            RequireSameType(Operand);
            int Order;
            switch (type)
            {
                case eStoryDataType.Integer: Order = integer.CompareTo(Operand.integer); break;
                case eStoryDataType.Float: Order = number.CompareTo(Operand.number); break;
                case eStoryDataType.Boolean: Order = boolean.CompareTo(Operand.boolean); break;
                case eStoryDataType.String: Order = string.CompareOrdinal(text, Operand.text); break;
                default: throw new InvalidOperationException("无参数值不能参与比较");
            }
            if ((type == eStoryDataType.Boolean || type == eStoryDataType.String) &&
                Comparison != eStoryComparison.Equal && Comparison != eStoryComparison.NotEqual)
                throw new InvalidOperationException($"{type} 不支持 {Comparison}");
            switch (Comparison)
            {
                case eStoryComparison.Equal: return Order == 0;
                case eStoryComparison.NotEqual: return Order != 0;
                case eStoryComparison.Greater: return Order > 0;
                case eStoryComparison.GreaterOrEqual: return Order >= 0;
                case eStoryComparison.Less: return Order < 0;
                case eStoryComparison.LessOrEqual: return Order <= 0;
                default: throw new ArgumentOutOfRangeException(nameof(Comparison));
            }
        }

        public override string ToString()
        {
            switch (type)
            {
                case eStoryDataType.Integer: return integer.ToString(CultureInfo.InvariantCulture);
                case eStoryDataType.Float: return number.ToString(CultureInfo.InvariantCulture);
                case eStoryDataType.Boolean: return boolean ? "true" : "false";
                case eStoryDataType.String: return text;
                case eStoryDataType.None: return "";
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private void RequireSameType(StoryValue Other)
        {
            if (type != Other.type)
                throw new InvalidOperationException($"剧情变量类型不一致 {type} 与 {Other.type}");
        }

        private InvalidOperationException WrongType() => new InvalidOperationException($"剧情值的实际类型为 {type}");
    }
}

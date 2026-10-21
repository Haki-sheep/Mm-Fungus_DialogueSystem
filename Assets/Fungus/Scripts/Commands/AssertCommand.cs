using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Assertions;

namespace Fungus
{
    /// <summary>
    /// Assert on 2 Fungus variable values.
    /// </summary>
    [CommandInfo("脚本与事件", "检查断言", "检查两个变量是否符合相等或不相等的约定 不符合时报告错误")]
    [AddComponentMenu("")]
    public class AssertCommand : Command
    {
        [LabelText("消息内容")]
        [SerializeField]
        protected StringData message;

        [LabelText("输入变量 A")]
        [SerializeField]
        [VariableProperty(AllVariableTypes.VariableAny.Any)]
        protected Variable a;

        [LabelText("输入变量 B")]
        [SerializeField]
        [VariableProperty(AllVariableTypes.VariableAny.Any)]
        protected Variable b;

        public enum Method
        {
            [UnityEngine.InspectorName("相等")]
            AreEqual,
            [UnityEngine.InspectorName("不相等")]
            AreNotEqual,
        }

        [LabelText("调用的方法")]
        [SerializeField]
        protected Method method;

        public override void OnEnter()
        {
            switch (method)
            {
                case Method.AreEqual:
                    Assert.AreEqual(a.GetValue(), b.GetValue());
                    break;

                case Method.AreNotEqual:
                    Assert.AreNotEqual(a.GetValue(), b.GetValue());
                    break;

                default:
                break;
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (a == null)
                return "请先设置输入变量 A";
            if (b == null)
                return "请先设置输入变量 B";

            return a.Key + " " + AuthoringValueLabel.Format(method) + " " + b.Key;
        }

        public override bool HasReference(Variable variable)
        {
            return variable == message.stringRef ||
                variable == a || variable == b ||
                base.HasReference(variable);
        }
    }
}

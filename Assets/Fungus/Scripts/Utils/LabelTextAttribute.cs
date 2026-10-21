using System;
using UnityEngine;

namespace Fungus.Inspector
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class LabelTextAttribute : PropertyAttribute
    {
        public string Text { get; }

        public LabelTextAttribute(string Text)
        {
            this.Text = Text;
            order = int.MinValue;
        }
    }
}

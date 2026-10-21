using System;
using System.Reflection;
using UnityEngine;

namespace Fungus
{
    public static class AuthoringValueLabel
    {
        public static string Format(object Value)
        {
            if (Value is Enum)
            {
                var Field = Value.GetType().GetField(Value.ToString());
                return Field?.GetCustomAttribute<InspectorNameAttribute>()?.displayName ?? Value.ToString();
            }
            return Value.ToString();
        }
    }
}

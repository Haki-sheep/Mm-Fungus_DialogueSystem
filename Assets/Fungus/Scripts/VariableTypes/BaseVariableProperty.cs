using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    [AddComponentMenu("")]
    public abstract class BaseVariableProperty : Command
    {
        public enum GetSet
        {
            [UnityEngine.InspectorName("读取")]
            Get,
            [UnityEngine.InspectorName("写入")]
            Set,
        }

        [LabelText("读取或写入")]
        public GetSet getOrSet = GetSet.Get;
    }
}
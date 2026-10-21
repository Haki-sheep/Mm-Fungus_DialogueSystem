using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// Material variable type.
    /// </summary>
    [VariableInfo("其他类型", "Material", DisplayName = "材质")]
    [AddComponentMenu("")]
    [System.Serializable]
    public class MaterialVariable : VariableBase<Material>
    {
    }

    /// <summary>
    /// Container for a Material variable reference or constant value.
    /// </summary>
    [System.Serializable]
    public struct MaterialData
    {
        [SerializeField]
        [VariableProperty("直接填值", typeof(MaterialVariable))]
        [LabelText("材质变量")]
        public MaterialVariable materialRef;
        
        [LabelText("材质值")]
        [SerializeField]
        public Material materialVal;

        public MaterialData(Material v)
        {
            materialVal = v;
            materialRef = null;
        }
        
        public static implicit operator Material(MaterialData materialData)
        {
            return materialData.Value;
        }

        public Material Value
        {
            get { return (materialRef == null) ? materialVal : materialRef.Value; }
            set { if (materialRef == null) { materialVal = value; } else { materialRef.Value = value; } }
        }

        public string GetDescription()
        {
            if (materialRef == null)
            {
                return materialVal != null ? materialVal.ToString() : "Null";
            }
            else
            {
                return materialRef.Key;
            }
        }
    }
}
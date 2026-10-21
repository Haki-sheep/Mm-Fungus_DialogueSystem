using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// Texture variable type.
    /// </summary>
    [VariableInfo("其他类型", "Texture", DisplayName = "贴图")]
    [AddComponentMenu("")]
    [System.Serializable]
    public class TextureVariable : VariableBase<Texture>
    {
    }

    /// <summary>
    /// Container for a Texture variable reference or constant value.
    /// </summary>
    [System.Serializable]
    public struct TextureData
    {
        [SerializeField]
        [VariableProperty("直接填值", typeof(TextureVariable))]
        [LabelText("贴图变量")]
        public TextureVariable textureRef;
        
        [LabelText("贴图值")]
        [SerializeField]
        public Texture textureVal;

        public TextureData(Texture v)
        {
            textureVal = v;
            textureRef = null;
        }
        
        public static implicit operator Texture(TextureData textureData)
        {
            return textureData.Value;
        }

        public Texture Value
        {
            get { return (textureRef == null) ? textureVal : textureRef.Value; }
            set { if (textureRef == null) { textureVal = value; } else { textureRef.Value = value; } }
        }

        public string GetDescription()
        {
            if (textureRef == null)
            {
                return textureVal != null ? textureVal.ToString() : "Null";
            }
            else
            {
                return textureRef.Key;
            }
        }
    }
}
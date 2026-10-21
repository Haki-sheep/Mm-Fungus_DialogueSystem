using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Animator variable type.
    /// </summary>
    [VariableInfo("其他类型", "Animator", DisplayName = "动画控制器")]
    [AddComponentMenu("")]
    [System.Serializable]
    public class AnimatorVariable : VariableBase<Animator>
    {
    }

    /// <summary>
    /// Container for an Animator variable reference or constant value.
    /// </summary>
    [System.Serializable]
    public struct AnimatorData
    {
        [SerializeField]
        [VariableProperty("直接填值", typeof(AnimatorVariable))]
        [LabelText("动画控制器变量")]
        public AnimatorVariable animatorRef;
        
        [LabelText("动画控制器值")]
        [SerializeField]
        public Animator animatorVal;

        public static implicit operator Animator(AnimatorData animatorData)
        {
            return animatorData.Value;
        }

        public AnimatorData(Animator v)
        {
            animatorVal = v;
            animatorRef = null;
        }
            
        public Animator Value
        {
            get { return (animatorRef == null) ? animatorVal : animatorRef.Value; }
            set { if (animatorRef == null) { animatorVal = value; } else { animatorRef.Value = value; } }
        }

        public string GetDescription()
        {
            if (animatorRef == null)
            {
                return animatorVal != null ? animatorVal.ToString() : "Null";
            }
            else
            {
                return animatorRef.Key;
            }
        }
    }
}
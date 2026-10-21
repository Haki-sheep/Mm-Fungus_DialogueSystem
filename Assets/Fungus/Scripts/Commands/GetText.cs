using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

namespace Fungus
{
    /// <summary>
    /// Gets the text property from a UI Text object and stores it in a string variable.
    /// </summary>
    [CommandInfo("界面", "读取界面文字", "对配置的目标执行读取界面文字 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    public class GetText : Command 
    {
        [Tooltip("设置本步骤使用的目标文字对象")]
        [LabelText("目标文字对象")]
        [SerializeField]
        protected GameObject targetTextObject;
        
        [Tooltip("设置本步骤使用的接收文字的变量")]
        [VariableProperty(typeof(StringVariable))]
        [LabelText("接收文字的变量")]
        [SerializeField]
        protected StringVariable stringVariable;

        #region Public members

        public override void OnEnter()
        {
            if (stringVariable == null)
            {
                Continue();
                return;
            }

            TextAdapter textAdapter = new TextAdapter();
            textAdapter.InitFromGameObject(targetTextObject);

            if (textAdapter.HasTextObject())
            {
                stringVariable.Value = textAdapter.Text;
            }

            Continue();
        }
        
        public override string GetSummary()
        {
            if (targetTextObject == null)
            {
                return "请先设置文字对象";
            }
            
            if (stringVariable == null)
            {
                return "请先设置变量";
            }
            
            return targetTextObject.name + " : " + stringVariable.name;
        }
        
        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return stringVariable == variable || 
                base.HasReference(variable);
        }

        #endregion

        #region Backwards compatibility

        // Backwards compatibility with Fungus v2.1.2
        [HideInInspector]
        [FormerlySerializedAs("textObject")]
        public Text _textObjectObsolete;
        protected virtual void OnEnable()
        {
            if (_textObjectObsolete != null)
            {
                targetTextObject = _textObjectObsolete.gameObject;
            }
        }

        #endregion
    }
}
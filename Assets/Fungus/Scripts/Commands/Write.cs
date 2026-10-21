using Fungus.Inspector;
using Cysharp.Threading.Tasks;
using System.Threading;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Text coloring mode for Write command.
    /// </summary>
    public enum TextColor
    {
        /// <summary> Don't change the text color. </summary>
        [UnityEngine.InspectorName("使用默认值")]
        Default,
        /// <summary> Set the text alpha to 1. </summary>
        [UnityEngine.InspectorName("设为完全可见")]
        SetVisible,
        /// <summary> Set the text alpha to a value. </summary>
        [UnityEngine.InspectorName("设置不透明度")]
        SetAlpha,
        /// <summary> Set the text color to a value. </summary>
        [UnityEngine.InspectorName("设置颜色")]
        SetColor
    }

    /// <summary>
    /// Writes content to a UI Text or Text Mesh object.
    /// </summary>
    [CommandInfo("界面", "逐字写入界面文字", "向界面文字对象逐字写入内容 可配置是否等待完成")]
    [AddComponentMenu("")]
    public class Write : Command, ILocalizable
    {
        [Tooltip("设置本步骤使用的文字对象")]
        [LabelText("文字对象")]
        [SerializeField]
        protected GameObject textObject;

        [Tooltip("设置本步骤使用的文字内容")]
        [LabelText("文字内容")]
        [SerializeField]
        protected StringDataMulti text;

        [Tooltip("填写这段流程或对白的用途 只用于编辑时说明")]
        [LabelText("用途说明")]
        [SerializeField]
        protected string description;

        [Tooltip("设置本步骤使用的清除已有文字")]
        [LabelText("清除已有文字")]
        [SerializeField]
        protected bool clearText = true;

        [Tooltip("勾选后等待本步骤完成再继续 不勾选则立即执行下一步")]
        [LabelText("等完成后再执行下一步")]
        [SerializeField]
        protected bool waitUntilFinished = true;

        [Tooltip("设置本步骤使用的文字颜色设置")]
        [LabelText("文字颜色设置")]
        [SerializeField]
        protected TextColor textColor = TextColor.Default;

        [Tooltip("设置本步骤使用的目标不透明度")]
        [LabelText("目标不透明度")]
        [SerializeField]
        protected FloatData setAlpha = new FloatData(1f);

        [Tooltip("设置本步骤使用的目标颜色")]
        [LabelText("目标颜色")]
        [SerializeField]
        protected ColorData setColor = new ColorData(Color.white);

        protected Writer GetWriter()
        {
            var writer = textObject.GetComponent<Writer>();
            if (writer == null)
            {
                writer = textObject.AddComponent<Writer>();
            }
            
            return writer;
        }

        #region Public members

        public override void OnEnter()
        {
            if (textObject == null)
            {
                Continue();
                return;
            }
        
            var writer = GetWriter();
            if (writer == null)
            {
                Continue();
                return;
            }

            switch (textColor)
            {
            case TextColor.SetAlpha:
                writer.SetTextAlpha(setAlpha);
                break;
            case TextColor.SetColor:
                writer.SetTextColor(setColor);
                break;
            case TextColor.SetVisible:
                writer.SetTextAlpha(1f);
                break;
            }

            var flowchart = GetFlowchart();
            string newText = flowchart.SubstituteVariables(text.Value);

            if (!waitUntilFinished)
            {
                asyncScope.Run(this, Token => writer.WriteAsync(newText, clearText, false, true, false, null, null, Token)).Forget();
                Continue();
            }
            else
            {
                asyncScope.Run(this, Token => writer.WriteAsync(newText, clearText, false, true, false, null,
                             () => { Continue (); }
                , Token)).Forget();
            }
        }

        public override string GetSummary()
        {
            if (textObject != null)
            {
                return textObject.name + " : " + text.Value;
            }

            return "请先设置文字对象";
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override void OnStopExecuting()
        {
            GetWriter().Stop();
        }

        #endregion

        #region ILocalizable implementation

        public virtual string GetStandardText()
        {
            return text;
        }

        public virtual void SetStandardText(string standardText)
        {
            text.Value = standardText;
        }

        public virtual string GetDescription()
        {
            return description;
        }
        
        public virtual string GetStringId()
        {
            // String id for Write commands is WRITE.<Localization Id>.<Command id>
            return "WRITE." + GetFlowchartLocalizationId() + "." + itemId;
        }

        public override bool HasReference(Variable variable)
        {
            return text.stringRef == variable || setAlpha.floatRef == variable || setColor.colorRef == variable || base.HasReference(variable);
        }

        #endregion
    }
}
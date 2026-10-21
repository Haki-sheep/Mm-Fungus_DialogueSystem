using Fungus.Inspector;
using Cysharp.Threading.Tasks;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Writes text in a dialog box.
    /// </summary>
    [CommandInfo("剧情对白", "显示对白", "显示角色对白 支持立绘 配音 文字标签及变量插值")]
    [AddComponentMenu("")]
    public class Say : Command, ILocalizable
    {
        // Removed this tooltip as users's reported it obscures the text box
        [TextArea(5,10)]
        [LabelText("对白内容")]
        [SerializeField]
        protected string storyText = "";

        [Tooltip("填写这段流程或对白的用途 只用于编辑时说明")]
        [LabelText("用途说明")]
        [SerializeField]
        protected string description = "";

        [Tooltip("选择本句发言的角色 决定姓名颜色和角色音效")]
        [LabelText("说话角色")]
        [SerializeField]
        protected Character character;

        [Tooltip("设置本步骤使用的角色表情")]
        [LabelText("角色表情")]
        [SerializeField]
        protected Sprite portrait;

        [Tooltip("设置本步骤使用的配音片段")]
        [LabelText("配音片段")]
        [SerializeField]
        protected AudioClip voiceOverClip;

        [Tooltip("设置本步骤使用的始终显示")]
        [LabelText("始终显示")]
        [SerializeField]
        protected bool showAlways = true;

        [Tooltip("设置本步骤使用的显示数量")]
        [LabelText("显示数量")]
        [SerializeField]
        protected int showCount = 1;

        [Tooltip("设置本步骤使用的接着上一句继续写")]
        [LabelText("接着上一句继续写")]
        [SerializeField]
        protected bool extendPrevious = false;

        [Tooltip("对白结束后将对白框淡出隐藏")]
        [LabelText("结束后隐藏对白框")]
        [SerializeField]
        protected bool fadeWhenDone = true;

        [Tooltip("设置本步骤使用的等待点击")]
        [LabelText("等待点击")]
        [SerializeField]
        protected bool waitForClick = true;

        [Tooltip("开始这句对白时停止上一句仍在播放的配音")]
        [LabelText("停止上一段配音")]
        [SerializeField]
        protected bool stopVoiceover = true;

        [Tooltip("配音播放完毕后才允许本句结束")]
        [LabelText("等待配音结束")]
        [SerializeField]
        protected bool waitForVO = false;

        //add wait for vo that overrides stopvo

        [Tooltip("设置本步骤使用的使用的对白界面")]
        [LabelText("使用的对白界面")]
        [SerializeField]
        protected SayDialog setSayDialog;

        protected int executionCount;

        #region Public members

        /// <summary>
        /// Character that is speaking.
        /// </summary>
        public virtual Character _Character { get { return character; } }

        /// <summary>
        /// Portrait that represents speaking character.
        /// </summary>
        public virtual Sprite Portrait { get { return portrait; } set { portrait = value; } }

        /// <summary>
        /// Type this text in the previous dialog box.
        /// </summary>
        public virtual bool ExtendPrevious { get { return extendPrevious; } }

        public override void OnEnter()
        {
            if (!showAlways && executionCount >= showCount)
            {
                Continue();
                return;
            }

            executionCount++;

            // Override the active say dialog if needed
            if (character != null && character.SetSayDialog != null)
            {
                SayDialog.ActiveSayDialog = character.SetSayDialog;
            }

            if (setSayDialog != null)
            {
                SayDialog.ActiveSayDialog = setSayDialog;
            }

            var sayDialog = SayDialog.GetSayDialog();
            if (sayDialog == null)
            {
                Continue();
                return;
            }
    
            var flowchart = GetFlowchart();

            sayDialog.SetActive(true);

            sayDialog.SetCharacter(character);
            sayDialog.SetCharacterImage(portrait);

            string displayText = storyText;

            var activeCustomTags = CustomTag.activeCustomTags;
            for (int i = 0; i < activeCustomTags.Count; i++)
            {
                var ct = activeCustomTags[i];
                displayText = displayText.Replace(ct.TagStartSymbol, ct.ReplaceTagStartWith);
                if (ct.TagEndSymbol != "" && ct.ReplaceTagEndWith != "")
                {
                    displayText = displayText.Replace(ct.TagEndSymbol, ct.ReplaceTagEndWith);
                }
            }

            string subbedText = flowchart.SubstituteVariables(displayText);

            asyncScope.Run(this, Token => sayDialog.DoSayAsync(subbedText, !extendPrevious, waitForClick,
                fadeWhenDone, stopVoiceover, waitForVO, voiceOverClip, CreateContinueCallback(), Token)).Forget();
        }

        public override string GetSummary()
        {
            string namePrefix = "";
            if (character != null) 
            {
                namePrefix = character.NameText + ": ";
            }
            if (extendPrevious)
            {
                namePrefix = "接着上一句" + ": ";
            }
            return namePrefix + "\"" + storyText + "\"";
        }

        public override Color GetButtonColor()
        {
            return new Color32(184, 210, 235, 255);
        }

        public override void OnReset()
        {
            executionCount = 0;
        }

        public override void OnStopExecuting()
        {
            var sayDialog = SayDialog.GetSayDialog();
            if (sayDialog == null)
            {
                return;
            }

            sayDialog.Stop();
        }

        #endregion

        #region ILocalizable implementation

        public virtual string GetStandardText()
        {
            return storyText;
        }

        public virtual void SetStandardText(string standardText)
        {
            storyText = standardText;
        }

        public virtual string GetDescription()
        {
            return description;
        }
        
        public virtual string GetStringId()
        {
            // String id for Say commands is SAY.<Localization Id>.<Command id>.[Character Name]
            string stringId = "SAY." + GetFlowchartLocalizationId() + "." + itemId + ".";
            if (character != null)
            {
                stringId += character.NameText;
            }

            return stringId;
        }

        #endregion
    }
}

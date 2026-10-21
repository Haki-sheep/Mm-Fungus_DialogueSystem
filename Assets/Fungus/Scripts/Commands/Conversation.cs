using Fungus.Inspector;
using Cysharp.Threading.Tasks;
using System.Threading;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// Do multiple say and portrait commands in a single block of text. Format is: [character] [portrait] [stage position] [hide] [<<< | >>>] [clear | noclear] [wait | nowait] [fade | nofade] [: Story text].
    /// </summary>
    [CommandInfo("剧情对白", "播放连续对白", "按照连续对白脚本播放多句台词及立绘变化")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class Conversation : Command
    {
        [LabelText("连续对白脚本")]
        [SerializeField]
        protected StringDataMulti conversationText;

        protected ConversationManager conversationManager = new ConversationManager();

        [LabelText("清除上一段文字")]
        [SerializeField]
        protected BooleanData clearPrevious = new BooleanData(true);
        [LabelText("等待玩家继续")]
        [SerializeField]
        protected BooleanData waitForInput = new BooleanData(true);
        [Tooltip("设置本步骤使用的额外停顿（秒）")]
        [LabelText("额外停顿（秒）")]
        [SerializeField]
        protected FloatData waitForSeconds = new FloatData(0);
        [LabelText("结束后隐藏对白框")]
        [SerializeField]
        protected BooleanData fadeWhenDone = new BooleanData(true);

        protected virtual void Start()
        {
            conversationManager.PopulateCharacterCache();
        }

        protected virtual async UniTask DoConversationAsync(CancellationToken CancellationToken = default)
        {
            var flowchart = GetFlowchart();
            string subbedText = flowchart.SubstituteVariables(conversationText.Value);

            conversationManager.ClearPrev = clearPrevious;
            conversationManager.WaitForInput = waitForInput;
            conversationManager.FadeDone = fadeWhenDone;
            conversationManager.WaitForSeconds = waitForSeconds;

            await conversationManager.DoConversationAsync(subbedText, CancellationToken);

            Continue();
        }

        #region Public members

        public override void OnEnter()
        {
            asyncScope.Run(this, Token => DoConversationAsync(Token)).Forget();
        }

        public override string GetSummary()
        {
            return conversationText.Value;
        }

        public override Color GetButtonColor()
        {
            return new Color32(184, 210, 235, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return clearPrevious.booleanRef == variable || waitForInput.booleanRef == variable || 
                waitForSeconds.floatRef == variable || fadeWhenDone.booleanRef == variable ||
                base.HasReference(variable);
        }

        #endregion


        #region Editor caches
#if UNITY_EDITOR
        protected override void RefreshVariableCache()
        {
            base.RefreshVariableCache();

            var f = GetFlowchart();

            if(!string.IsNullOrEmpty(conversationText.Value))
                f.DetermineSubstituteVariables(conversationText, referencedVariables);
        }
#endif
        #endregion Editor caches
    }
}
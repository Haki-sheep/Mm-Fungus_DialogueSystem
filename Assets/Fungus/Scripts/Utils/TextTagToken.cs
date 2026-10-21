// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;

namespace Fungus
{
    /// <summary>
    /// Supported token types for use in Say / Write text.
    /// </summary>
    public enum TokenType
    {
        /// <summary> Invalid token type. </summary>
        [UnityEngine.InspectorName("无效")]
        Invalid,
        /// <summary> A string of words. </summary>
        [UnityEngine.InspectorName("文字")]
        Words,
        /// <summary> b </summary>
        [UnityEngine.InspectorName("开始加粗")]
        BoldStart,              // 
        /// <summary> /b </summary>
        [UnityEngine.InspectorName("结束加粗")]
        BoldEnd,
        /// <summary> i </summary>
        [UnityEngine.InspectorName("开始斜体")]
        ItalicStart,
        /// <summary> /i </summary>
        [UnityEngine.InspectorName("结束斜体")]
        ItalicEnd,
        /// <summary> color=red </summary>
        [UnityEngine.InspectorName("开始文字颜色")]
        ColorStart,
        /// <summary> /color </summary>
        [UnityEngine.InspectorName("结束文字颜色")]
        ColorEnd,
        /// <summary> size=20 </summary>
        [UnityEngine.InspectorName("开始文字大小")]
        SizeStart,
        /// <summary> /size </summary>
        [UnityEngine.InspectorName("结束文字大小")]
        SizeEnd,
        /// <summary> w, w=0.5 </summary>
        [UnityEngine.InspectorName("等待")]
        Wait,
        /// <summary> wi </summary>
        [UnityEngine.InspectorName("等待输入并保留文字")]
        WaitForInputNoClear,
        /// <summary> wc </summary>
        [UnityEngine.InspectorName("等待输入后清空")]
        WaitForInputAndClear,
        /// <summary> wp, wp=0.5 </summary>
        [UnityEngine.InspectorName("开始标点停顿")]
        WaitOnPunctuationStart,
        /// <summary> /wp </summary>
        [UnityEngine.InspectorName("结束标点停顿")]
        WaitOnPunctuationEnd,
        /// <summary> c </summary>
        [UnityEngine.InspectorName("清空")]
        Clear,
        /// <summary> s, s=60 </summary>
        [UnityEngine.InspectorName("开始打字速度")]
        SpeedStart,
        /// <summary> /s </summary>
        [UnityEngine.InspectorName("结束打字速度")]
        SpeedEnd,
        /// <summary> x </summary>
        [UnityEngine.InspectorName("离开时")]
        Exit,
        /// <summary> m=MessageName </summary>
        [UnityEngine.InspectorName("消息")]
        Message,
        /// <summary> vpunch=0.5 </summary>
        [UnityEngine.InspectorName("竖直弹动")]
        VerticalPunch,
        /// <summary> hpunch=0.5 </summary>
        [UnityEngine.InspectorName("水平弹动")]
        HorizontalPunch,
        /// <summary> punch=0.5 </summary>
        [UnityEngine.InspectorName("弹动")]
        Punch,
        /// <summary> flash=0.5 </summary>
        [UnityEngine.InspectorName("闪烁")]
        Flash,
        /// <summary> audio=Sound </summary>
        [UnityEngine.InspectorName("声音")]
        Audio,
        /// <summary> audioloop=Sound </summary>
        [UnityEngine.InspectorName("循环音频")]
        AudioLoop,
        /// <summary> audiopause=Sound </summary>
        [UnityEngine.InspectorName("暂停音频")]
        AudioPause,
        /// <summary> audiostop=Sound </summary>
        [UnityEngine.InspectorName("停止音频")]
        AudioStop,
        /// <summary> wvo </summary>
        [UnityEngine.InspectorName("等待配音")]
        WaitForVoiceOver,
        /// <summary> link start </summary>
        [UnityEngine.InspectorName("开始链接")]
        LinkStart,
        /// <summary> link end </summary>
        [UnityEngine.InspectorName("结束链接")]
        LinkEnd,
    }

    /// <summary>
    /// Represents a token of story text. The text is broken into a list of tokens.
    /// </summary>
    public class TextTagToken
    {
        #region Public members

        /// <summary>
        /// The type of the token.
        /// </summary>
        public TokenType type = TokenType.Invalid;

        /// <summary>
        /// List of comma separated parameters.
        /// </summary>
        public List<string> paramList;

        #endregion
    }
}
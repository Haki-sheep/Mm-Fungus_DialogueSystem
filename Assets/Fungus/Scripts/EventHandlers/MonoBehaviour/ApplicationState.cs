using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// The block will execute when the desired OnApplication message for the monobehaviour is received.
    /// </summary>
    [EventHandlerInfo("组件回调", "应用状态回调", "在应用状态回调时执行所属流程")]
    [AddComponentMenu("")]
    public class ApplicationState : EventHandler
    {

        [System.Flags]
        public enum ApplicationMessageFlags
        {
            [UnityEngine.InspectorName("窗口获得焦点")]
            OnApplicationGetFocus  = 1 << 0,
            [UnityEngine.InspectorName("窗口失去焦点")]
            OnApplicationLoseFocus = 1 << 1,
            [UnityEngine.InspectorName("应用暂停")]
            OnApplicationPause    = 1 << 2,
            [UnityEngine.InspectorName("应用恢复")]
            OnApplicationResume = 1 << 3,
            [UnityEngine.InspectorName("应用退出")]
            OnApplicationQuit = 1 << 4,
        }

        [Tooltip("Which of the Application messages to trigger on.")]
        [SerializeField]
        [EnumFlag]
        [LabelText("触发时机")]
        protected ApplicationMessageFlags FireOn = ApplicationMessageFlags.OnApplicationQuit;

        private void OnApplicationFocus(bool focus)
        {
            if (
                (focus && ((FireOn & ApplicationMessageFlags.OnApplicationGetFocus) != 0))
                ||
                (!focus && ((FireOn & ApplicationMessageFlags.OnApplicationLoseFocus) != 0))
                )
            {
                ExecuteBlock();
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if ( 
                (pause && (( FireOn & ApplicationMessageFlags.OnApplicationPause) != 0))
                ||
                (!pause && ((FireOn & ApplicationMessageFlags.OnApplicationResume) != 0))
                )
            {
                ExecuteBlock();
            }
        }

        private void OnApplicationQuit()
        {
            if((FireOn & ApplicationMessageFlags.OnApplicationQuit) != 0)
            {
                ExecuteBlock();
            }
        }
    }
}

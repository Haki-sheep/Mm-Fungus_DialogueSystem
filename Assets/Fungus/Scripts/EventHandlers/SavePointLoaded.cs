using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections.Generic;

namespace Fungus
{
    [EventHandlerInfo("场景", "读取存档点", "在读取存档点时执行所属流程")]
    public class SavePointLoaded : EventHandler 
    {
        [Tooltip("设置本步骤使用的存档点标识列表")]
        [LabelText("存档点标识列表")]
        [SerializeField]
        protected List<string> savePointKeys = new List<string>();

        protected void OnSavePointLoaded(string _savePointKey)
        {
            for (int i = 0; i < savePointKeys.Count; i++)
            {
                var key = savePointKeys[i];
                if (string.Compare(key, _savePointKey, true) == 0)
                {
                    ExecuteBlock();
                    break;
                }
            }
        }

        #region Public methods

        public static void NotifyEventHandlers(string _savePointKey)
        {
            // Fire any matching SavePointLoaded event handler with matching save key.
        #if UNITY_6000
            var eventHandlers = Object.FindObjectsByType<SavePointLoaded>(FindObjectsSortMode.None);
        #else
            var eventHandlers = Object.FindObjectsOfType<SavePointLoaded>();
        #endif
            for (int i = 0; i < eventHandlers.Length; i++)
            {
                var eventHandler = eventHandlers[i];
                eventHandler.OnSavePointLoaded(_savePointKey);
            }
        }

        #endregion
    }
}
using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using UnityEngine.Serialization;

namespace Fungus
{
    /// <summary>
    /// Set the active language for the scene. A Localization object with a localization file must be present in the scene.
    /// </summary>
    [CommandInfo("剧情对白", "切换语言", "切换当前对白使用的语言代码")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class SetLanguage : Command
    {
        [Tooltip("设置本步骤使用的语言代码")]
        [LabelText("语言代码")]
        [SerializeField]
        protected StringData _languageCode = new StringData(); 

        #region Public members

        public static string mostRecentLanguage = "";

        public override void OnEnter()
        {
        #if UNITY_6000
            Localization localization = GameObject.FindFirstObjectByType<Localization>();
        #else
            Localization localization = GameObject.FindObjectOfType<Localization>();
        #endif
            if (localization != null)
            {
                localization.SetActiveLanguage(_languageCode.Value, true);

                // Cache the most recently set language code so we can continue to 
                // use the same language in subsequent scenes.
                mostRecentLanguage = _languageCode.Value;
            }

            Continue();
        }

        public override string GetSummary()
        {
            return _languageCode.Value;
        }

        public override Color GetButtonColor()
        {
            return new Color32(184, 210, 235, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return _languageCode.stringRef == variable || base.HasReference(variable);
        }

        #endregion

        #region Backwards compatibility

        [HideInInspector] [FormerlySerializedAs("languageCode")] public string languageCodeOLD = "";

        protected virtual void OnEnable()
        {
            if (languageCodeOLD != "")
            {
                _languageCode.Value = languageCodeOLD;
                languageCodeOLD = "";
            }
        }

        #endregion
    }
}
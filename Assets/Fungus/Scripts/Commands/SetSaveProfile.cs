using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// Sets the active profile that the Save Variable and Load Variable commands will use. This is useful to crete multiple player save games. Once set, the profile applies across all Flowcharts and will also persist across scene loads.
    /// </summary>
    [CommandInfo("变量", "选择存档配置", "设置保存键使用的配置前缀 区分不同存档配置")]
    [AddComponentMenu("")]
    public class SetSaveProfile : Command
    {
        [Tooltip("设置存档键的配置前缀 用于区分不同存档配置")]
        [LabelText("存档配置名称")]
        [SerializeField]
        protected string saveProfileName = "";

		/// <summary>
		/// Shared save profile name used by SaveVariable and LoadVariable.
		/// </summary>
		private static string saveProfile = "";

        #region Public members

		public static String SaveProfile { get { return saveProfile; } }

        public override void OnEnter()
        {
            saveProfile = saveProfileName;

            Continue();
        }
        
        public override string GetSummary()
        {
            return saveProfileName;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        #endregion
    }    
}
using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Deletes a saved value from permanent storage.
    /// </summary>
    [CommandInfo("变量", "删除变量存档", "对配置的目标执行删除变量存档 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    public class DeleteSaveKey : Command
    {
        [Tooltip("保存与读取必须使用相同的键名 不同剧情请避免重名")]
        [LabelText("存取键名")]
        [SerializeField]
        protected string key = "";

        #region Public members

        public override void OnEnter()
        {
            if (key == "")
            {
                Continue();
                return;
            }
            
            var flowchart = GetFlowchart();
            
            // Prepend the current save profile (if any)
            string prefsKey = SetSaveProfile.SaveProfile + "_" + flowchart.SubstituteVariables(key);
            
            PlayerPrefs.DeleteKey(prefsKey);

            Continue();
        }
        
        public override string GetSummary()
        {
            if (key.Length == 0)
            {
                return "请先设置存取键名";
            }

            return key;
        }
        
        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        #endregion

        #region Editor caches
#if UNITY_EDITOR
        protected override void RefreshVariableCache()
        {
            base.RefreshVariableCache();

            var f = GetFlowchart();

            f.DetermineSubstituteVariables(key, referencedVariables);
        }
#endif
        #endregion Editor caches
    }
}
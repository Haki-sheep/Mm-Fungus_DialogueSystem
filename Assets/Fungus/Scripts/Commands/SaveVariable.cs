using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Save an Boolean, Integer, Float or String variable to persistent storage using a string key.
    /// The value can be loaded again later using the Load Variable command. You can also 
    /// use the Set Save Profile command to manage separate save profiles for multiple players.
    /// </summary>
    [CommandInfo("变量", "保存变量", "将一个变量写入 PlayerPrefs 使用指定键名保存" +
                 "The value can be loaded again later using the Load Variable command. You can also " +
                 "use the Set Save Profile command to manage separate save profiles for multiple players.")]
    [AddComponentMenu("")]
    public class SaveVariable : Command
    {
        [Tooltip("保存与读取必须使用相同的键名 不同剧情请避免重名")]
        [LabelText("存取键名")]
        [SerializeField]
        protected string key = "";
        
        [Tooltip("设置本步骤使用的使用的变量")]
        [VariableProperty(typeof(BooleanVariable),
                          typeof(IntegerVariable), 
                          typeof(FloatVariable), 
                          typeof(StringVariable))]
        [LabelText("使用的变量")]
        [SerializeField]
        protected Variable variable;

        #region Public members

        public override void OnEnter()
        {
            if (key == "" ||
                variable == null)
            {
                Continue();
                return;
            }
            
            var flowchart = GetFlowchart();
            
            // Prepend the current save profile (if any)
            string prefsKey = SetSaveProfile.SaveProfile + "_" + flowchart.SubstituteVariables(key);
            
            System.Type variableType = variable.GetType();

            if (variableType == typeof(BooleanVariable))
            {
                BooleanVariable booleanVariable = variable as BooleanVariable;
                if (booleanVariable != null)
                {
                    // PlayerPrefs does not have bool accessors, so just use int
                    PlayerPrefs.SetInt(prefsKey, booleanVariable.Value ? 1 : 0);
                }
            }
            else if (variableType == typeof(IntegerVariable))
            {
                IntegerVariable integerVariable = variable as IntegerVariable;
                if (integerVariable != null)
                {
                    PlayerPrefs.SetInt(prefsKey, integerVariable.Value);
                }
            }
            else if (variableType == typeof(FloatVariable))
            {
                FloatVariable floatVariable = variable as FloatVariable;
                if (floatVariable != null)
                {
                    PlayerPrefs.SetFloat(prefsKey, floatVariable.Value);
                }
            }
            else if (variableType == typeof(StringVariable))
            {
                StringVariable stringVariable = variable as StringVariable;
                if (stringVariable != null)
                {
                    PlayerPrefs.SetString(prefsKey, stringVariable.Value);
                }
            }
            
            Continue();
        }
        
        public override string GetSummary()
        {
            if (key.Length == 0)
            {
                return "请先设置存取键名";
            }
            
            if (variable == null)
            {
                return "请先设置变量";
            }
            
            return variable.Key + " 保存到键名 '" + key + "'";
        }
        
        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool HasReference(Variable in_variable)
        {
            return this.variable == in_variable || base.HasReference(in_variable);
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
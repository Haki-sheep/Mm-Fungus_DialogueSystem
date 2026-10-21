using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using Fungus;

namespace Fungus
{
    /// <summary>
    /// Reads in a text file and stores the contents in a string variable.
    /// </summary>
    [CommandInfo("变量", "读取文本文件", "读取文本文件内容 并写入目标变量")]
    public class ReadTextFile : Command
    {
        [Tooltip("设置本步骤使用的文本文件")]
        [LabelText("文本文件")]
        [SerializeField]
        protected TextAsset textFile;

        [Tooltip("设置本步骤使用的接收文字的变量")]
        [VariableProperty(typeof(StringVariable))]
        [LabelText("接收文字的变量")]
        [SerializeField]
        protected StringVariable stringVariable;

        #region Public members

        public override void OnEnter() 
        {
            if (textFile == null || 
                stringVariable == null) 
            {
                Continue();
                return;
            }

            stringVariable.Value = textFile.text;

            Continue();
        }

        public override string GetSummary()
        {
            if (stringVariable == null)
            {
                return "请先设置变量";
            }

            if (textFile == null)
            {
                return "请先设置文本文件";
            }

            return stringVariable.Key;
        }
        
        public override bool HasReference(Variable variable)
        {
            return (variable == stringVariable);
        }
        
        public override Color GetButtonColor()
        {
            return new Color32(253, 253, 150, 255);
        }

        #endregion
    }
}
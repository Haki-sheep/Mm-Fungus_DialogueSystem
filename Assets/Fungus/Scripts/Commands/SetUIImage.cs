using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Fungus
{
    /// <summary>
    /// Changes the Image property on a UI element.
    /// </summary>
    [CommandInfo("界面", "替换界面图片", "对配置的目标执行替换界面图片 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    public class SetUIImage : Command 
    {
        [Tooltip("设置本步骤使用的界面图片列表")]
        [LabelText("界面图片列表")]
        [SerializeField]
        protected List<Image> images = new List<Image>();

        [Tooltip("设置本步骤使用的精灵图片")]
        [LabelText("精灵图片")]
        [SerializeField]
        protected Sprite sprite;

        #region Public members

        public override void OnEnter()
        {
            for (int i = 0; i < images.Count; i++)
            {
                var image = images[i];
                image.sprite = sprite;
            }

            Continue();
        }
        
        public override string GetSummary()
        {
            string summary = "";
            for (int i = 0; i < images.Count; i++)
            {
                var targetImage = images[i];
                if (targetImage == null)
                {
                    continue;
                }
                if (summary.Length > 0)
                {
                    summary += ", ";
                }
                summary += targetImage.name;
            }

            if (summary.Length == 0)
            {
                return "请先设置精灵图片";
            }

            return summary + " = " + sprite;
        }
        
        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool IsReorderableArray(string propertyName)
        {
            if (propertyName == "images")
            {
                return true;
            }

            return false;
        }

        public override void OnCommandAdded(Block parentBlock)
        {
            // Add a default empty entry
            images.Add(null);
        }

        #endregion
    }
}

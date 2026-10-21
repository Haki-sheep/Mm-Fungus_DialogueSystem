using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using System.Collections.Generic;

namespace Fungus
{
    /// <summary>
    /// Controls the render order of sprites by setting the Order In Layer property of a list of sprites.
    /// </summary>
    [CommandInfo("精灵图片", "设置精灵显示顺序", "对配置的目标执行设置精灵显示顺序 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    public class SetSpriteOrder : Command 
    {
        [Tooltip("设置本步骤使用的目标精灵列表")]
        [LabelText("目标精灵列表")]
        [SerializeField]
        protected List<SpriteRenderer> targetSprites = new List<SpriteRenderer>();

        [Tooltip("设置本步骤使用的层内顺序")]
        [LabelText("层内顺序")]
        [SerializeField]
        protected IntegerData orderInLayer;

        #region Public members

        public override void OnEnter()
        {
            for (int i = 0; i < targetSprites.Count; i++)
            {
                var spriteRenderer = targetSprites[i];
                spriteRenderer.sortingOrder = orderInLayer;
            }

            Continue();
        }
        
        public override string GetSummary()
        {
            string summary = "";
            for (int i = 0; i < targetSprites.Count; i++)
            {
                var spriteRenderer = targetSprites[i];
                if (spriteRenderer == null)
                {
                    continue;
                }
                if (summary.Length > 0)
                {
                    summary += ", ";
                }
                summary += spriteRenderer.name;
            }

            if (summary.Length == 0)
            {
                return "请先设置指针图片";
            }

            return summary + " = " + orderInLayer.Value;
        }
        
        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool IsReorderableArray(string propertyName)
        {
            if (propertyName == "targetSprites")
            {
                return true;
            }

            return false;
        }

        public override void OnCommandAdded(Block parentBlock)
        {
            // Add a default empty entry
            targetSprites.Add(null);
        }

        public override bool HasReference(Variable variable)
        {
            return orderInLayer.integerRef == variable || base.HasReference(variable);
        }

        #endregion
    }
}

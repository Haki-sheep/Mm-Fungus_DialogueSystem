using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

// Snippet added by ducksonthewater, 2019-01-03 - www.ducks-on-the-water.com

using UnityEngine;
using System.Collections.Generic;

namespace Fungus
{
    /// <summary>
    /// Changes the sprite on a SpriteRenderer.
    /// </summary>
    [CommandInfo("精灵图片", "替换精灵图片", "对配置的目标执行替换精灵图片 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    public class SetSprite : Command 
    {
        [Tooltip("设置本步骤使用的精灵渲染器列表")]
        [LabelText("精灵渲染器列表")]
        [SerializeField]
        protected List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();

        [Tooltip("设置本步骤使用的精灵图片")]
        [LabelText("精灵图片")]
        [SerializeField]
        protected Sprite sprite;

        #region Public members

        public override void OnEnter()
        {
            for (int i = 0; i < spriteRenderers.Count; i++)
            {
                var spriteRenderer = spriteRenderers[i];
                spriteRenderer.sprite = sprite;
            }

            Continue();
        }
        
        public override string GetSummary()
        {
            string summary = "";
            for (int i = 0; i < spriteRenderers.Count; i++)
            {
                var spriteRenderer = spriteRenderers[i];
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
            if (propertyName == "spriteRenderers")
            {
                return true;
            }

            return false;
        }

        public override void OnCommandAdded(Block parentBlock)
        {
            // Add a default empty entry
            spriteRenderers.Add(null);
        }

        #endregion
    }
}

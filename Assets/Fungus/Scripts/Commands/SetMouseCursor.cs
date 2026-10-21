using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Sets the mouse cursor sprite.
    /// </summary>
    [CommandInfo("精灵图片", "设置鼠标指针", "对配置的目标执行设置鼠标指针 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    public class SetMouseCursor : Command 
    {
        [Tooltip("设置本步骤使用的鼠标指针贴图")]
        [LabelText("鼠标指针贴图")]
        [SerializeField]
        protected Texture2D cursorTexture;

        [Tooltip("设置本步骤使用的指针点击热点")]
        [LabelText("指针点击热点")]
        [SerializeField]
        protected Vector2 hotSpot;

        // Cached static cursor settings
        protected static Texture2D activeCursorTexture;
        protected static Vector2 activeHotspot;

        #region Public members

        public static void ResetMouseCursor()
        {
            // Change mouse cursor back to most recent settings
            Cursor.SetCursor(activeCursorTexture, activeHotspot, CursorMode.Auto);
        }

        public override void OnEnter()
        {
            Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);

            activeCursorTexture = cursorTexture;
            activeHotspot = hotSpot;

            Continue();
        }

        public override string GetSummary()
        {
            if (cursorTexture == null)
            {
                return "请先设置指针图片";
            }

            return cursorTexture.name;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        #endregion
    }
}
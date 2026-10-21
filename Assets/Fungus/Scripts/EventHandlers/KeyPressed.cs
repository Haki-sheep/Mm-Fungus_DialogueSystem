using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Key press modes supported by Key Pressed event handler.
    /// </summary>
    public enum KeyPressType
    {
        /// <summary> Execute once when the key is pressed down. </summary>
        [UnityEngine.InspectorName("刚按下时")]
        KeyDown,
        /// <summary> Execute once when the key is released </summary>
        [UnityEngine.InspectorName("刚松开时")]
        KeyUp,
        /// <summary> Execute once per frame when key is held down. </summary>
        [UnityEngine.InspectorName("按住期间")]
        KeyRepeat
    }

    /// <summary>
    /// The block will execute when a key press event occurs.
    /// </summary>
    [EventHandlerInfo("玩家输入", "按键触发", "在按键触发时执行所属流程")]
    [AddComponentMenu("")]
    public class KeyPressed : EventHandler
    {
        [Tooltip("设置本步骤使用的按键触发时机")]
        [LabelText("按键触发时机")]
        [SerializeField]
        protected KeyPressType keyPressType;

        [Tooltip("设置本步骤使用的按键")]
        [LabelText("按键")]
        [SerializeField]
        protected KeyCode keyCode;

        protected virtual void Update()
        {
            switch (keyPressType)
            {
            case KeyPressType.KeyDown:
                if (Input.GetKeyDown(keyCode))
                {
                    ExecuteBlock();
                }
                break;
            case KeyPressType.KeyUp:
                if (Input.GetKeyUp(keyCode))
                {
                    ExecuteBlock();
                }
                break;
            case KeyPressType.KeyRepeat:
                if (Input.GetKey(keyCode))
                {
                    ExecuteBlock();
                }
                break;
            }
        }
    }
}
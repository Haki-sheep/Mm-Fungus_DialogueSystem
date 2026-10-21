using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// The block will execute when the desired OnMouse* message for the monobehaviour is received
    /// </summary>
    [EventHandlerInfo("组件回调", "鼠标事件", "在鼠标事件时执行所属流程")]
    [AddComponentMenu("")]
    public class Mouse : EventHandler
    {

        [System.Flags]
        public enum MouseMessageFlags
        {
            [UnityEngine.InspectorName("鼠标按下")]
            OnMouseDown     = 1 << 0,
            [UnityEngine.InspectorName("鼠标拖动")]
            OnMouseDrag     = 1 << 1,
            [UnityEngine.InspectorName("鼠标移入")]
            OnMouseEnter    = 1 << 2,
            [UnityEngine.InspectorName("鼠标移出")]
            OnMouseExit     = 1 << 3,
            [UnityEngine.InspectorName("鼠标悬停")]
            OnMouseOver     = 1 << 4,
            [UnityEngine.InspectorName("鼠标松开")]
            OnMouseUp       = 1 << 5,
            [UnityEngine.InspectorName("在同一对象上按下并松开")]
            OnMouseUpAsButton = 1 << 6,
        }

        [Tooltip("Which of the Mouse messages to trigger on.")]
        [SerializeField]
        [EnumFlag]
        [LabelText("触发时机")]
        protected MouseMessageFlags FireOn = MouseMessageFlags.OnMouseUpAsButton;

        private void OnMouseDown()
        {
            HandleTriggering(MouseMessageFlags.OnMouseDown);
        }

        private void OnMouseDrag()
        {
            HandleTriggering(MouseMessageFlags.OnMouseDrag);
        }

        private void OnMouseEnter()
        {
            HandleTriggering(MouseMessageFlags.OnMouseEnter);
        }

        private void OnMouseExit()
        {
            HandleTriggering(MouseMessageFlags.OnMouseExit);
        }

        private void OnMouseOver()
        {
            HandleTriggering(MouseMessageFlags.OnMouseOver);
        }

        private void OnMouseUp()
        {
            HandleTriggering(MouseMessageFlags.OnMouseUp);
        }

        private void OnMouseUpAsButton()
        {
            HandleTriggering(MouseMessageFlags.OnMouseUpAsButton);
        }

        private void HandleTriggering(MouseMessageFlags from)
        {
            if((from & FireOn) != 0)
            {
                ExecuteBlock();
            }
        }
    }
}

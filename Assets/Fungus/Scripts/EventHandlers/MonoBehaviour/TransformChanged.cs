using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// The block will execute when the desired OnTransform related message for the monobehaviour is received.
    /// </summary>
    [EventHandlerInfo("组件回调", "变换层级改变", "在变换层级改变时执行所属流程")]
    [AddComponentMenu("")]
    public class TransformChanged : EventHandler
    {

        [System.Flags]
        public enum TransformMessageFlags
        {
            [UnityEngine.InspectorName("子对象列表改变")]
            OnTransformChildrenChanged = 1 << 0,
            [UnityEngine.InspectorName("父对象改变")]
            OnTransformParentChanged = 1 << 1,
        }

        [Tooltip("Which of the OnTransformChanged messages to trigger on.")]
        [SerializeField]
        [EnumFlag]
        [LabelText("触发时机")]
        protected TransformMessageFlags FireOn = TransformMessageFlags.OnTransformChildrenChanged | TransformMessageFlags.OnTransformParentChanged;

        private void OnTransformChildrenChanged()
        {
            if ((FireOn & TransformMessageFlags.OnTransformChildrenChanged) != 0)
            {
                ExecuteBlock();
            }
        }

        private void OnTransformParentChanged()
        {
            if ((FireOn & TransformMessageFlags.OnTransformParentChanged) != 0)
            {
                ExecuteBlock();
            }
        }
    }
}

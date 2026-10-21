using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// The block will execute when the desired OnAnimator* message for the monobehaviour is received.
    /// </summary>
    [EventHandlerInfo("组件回调", "动画回调", "在动画回调时执行所属流程")]
    [AddComponentMenu("")]
    public class AnimatorState : EventHandler
    {

        [System.Flags]
        public enum AnimatorMessageFlags
        {
            [UnityEngine.InspectorName("动画反向运动学更新")]
            OnAnimatorIK = 1 << 0,
            [UnityEngine.InspectorName("动画位移更新")]
            OnAnimatorMove = 1 << 1,
        }

        [Tooltip("设置本步骤使用的反向运动学层")]
        [SerializeField]
        [EnumFlag]
        [LabelText("触发时机")]
        protected AnimatorMessageFlags FireOn = AnimatorMessageFlags.OnAnimatorMove;

        [Tooltip("设置本步骤使用的反向运动学层")]
        [LabelText("反向运动学层")]
        [SerializeField]
        protected int IKLayer = 1;
        
        private void OnAnimatorIK(int layer)
        {
            if ((FireOn & AnimatorMessageFlags.OnAnimatorIK) != 0 && 
                (IKLayer == layer || IKLayer < 0) )
            {
                ExecuteBlock();
            }
        }

        private void OnAnimatorMove()
        {
            if ((FireOn & AnimatorMessageFlags.OnAnimatorMove) != 0)
            {
                ExecuteBlock();
            }
        }
    }
}

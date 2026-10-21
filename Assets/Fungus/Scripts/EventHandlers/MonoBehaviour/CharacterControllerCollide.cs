using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// The block will execute when tag filtered OnControllerColliderHit is received.
    /// </summary>
    [EventHandlerInfo("组件回调", "角色控制器碰撞", "在角色控制器碰撞时执行所属流程")]
    [AddComponentMenu("")]
    public class CharacterControllerCollide : TagFilteredEventHandler
    {
        [Tooltip("设置本步骤使用的接收碰撞信息")]
        [VariableProperty(typeof(ControllerColliderHitVariable))]
        [LabelText("接收碰撞信息")]
        [SerializeField]
        protected ControllerColliderHitVariable colHitVar;

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (DoesPassFilter(hit.gameObject.tag))
            {
                if (colHitVar != null)
                {
                    colHitVar.Value = hit;
                }

                ExecuteBlock();
            }
        }
    }
}
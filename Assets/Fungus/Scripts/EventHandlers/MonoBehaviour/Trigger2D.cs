using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// The block will execute when a 2d physics trigger matching some basic conditions is met.
    /// </summary>
    [EventHandlerInfo("组件回调", "二维触发器", "在二维触发器时执行所属流程")]
    [AddComponentMenu("")]
    public class Trigger2D : BasePhysicsEventHandler
    {
        [Tooltip("设置本步骤使用的接收碰撞体")]
        [VariableProperty(typeof(Collider2DVariable))]
        [LabelText("接收碰撞体")]
        [SerializeField]
        protected Collider2DVariable colliderVar;

        private void OnTriggerEnter2D(Collider2D col)
        {
            ProcessCollider(PhysicsMessageType.Enter, col);
        }

        private void OnTriggerStay2D(Collider2D col)
        {
            ProcessCollider(PhysicsMessageType.Stay, col);
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            ProcessCollider(PhysicsMessageType.Exit, col);
        }

        protected void ProcessCollider(PhysicsMessageType from, Collider2D other)
        {
            if ((from & FireOn) != 0 && DoesPassFilter(other.tag))
            {
                if (colliderVar != null)
                {
                    colliderVar.Value = other;
                }

                ExecuteBlock();
            }
        }
    }
}
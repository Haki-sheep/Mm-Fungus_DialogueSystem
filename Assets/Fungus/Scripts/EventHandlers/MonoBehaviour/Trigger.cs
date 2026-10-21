using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.UI;

namespace Fungus
{
    /// <summary>
    /// The block will execute when a 3d physics trigger matching some basic conditions is met. 
    /// </summary>
    [EventHandlerInfo("组件回调", "三维触发器", "在三维触发器时执行所属流程")]
    [AddComponentMenu("")]
    public class Trigger : BasePhysicsEventHandler
    {
        [Tooltip("设置本步骤使用的接收碰撞体")]
        [VariableProperty(typeof(ColliderVariable))]
        [LabelText("接收碰撞体")]
        [SerializeField]
        protected ColliderVariable colliderVar;

        private void OnTriggerEnter(Collider col)
        {
            ProcessCollider(PhysicsMessageType.Enter, col);
        }

        private void OnTriggerStay(Collider col)
        {
            ProcessCollider(PhysicsMessageType.Stay, col);
        }

        private void OnTriggerExit(Collider col)
        {
            ProcessCollider(PhysicsMessageType.Exit, col);
        }

        protected void ProcessCollider(PhysicsMessageType from, Collider other)
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
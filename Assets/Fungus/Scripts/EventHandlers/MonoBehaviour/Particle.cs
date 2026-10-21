using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// The block will execute when the desired OnParticle message for the monobehaviour is received.
    /// </summary>
    [EventHandlerInfo("组件回调", "粒子事件", "在粒子事件时执行所属流程")]
    [AddComponentMenu("")]
    public class Particle : TagFilteredEventHandler
    {

        [System.Flags]
        public enum ParticleMessageFlags
        {
            [UnityEngine.InspectorName("粒子发生碰撞")]
            OnParticleCollision = 1 << 0,
            [UnityEngine.InspectorName("粒子触发器事件")]
            OnParticleTrigger = 1 << 1,
           
        }

        [Tooltip("设置本步骤使用的接收碰撞物体")]
        [SerializeField]
        [EnumFlag]
        [LabelText("触发时机")]
        protected ParticleMessageFlags FireOn = ParticleMessageFlags.OnParticleCollision;

        [Tooltip("设置本步骤使用的接收碰撞物体")]
        [VariableProperty(typeof(GameObjectVariable))]
        [LabelText("接收碰撞物体")]
        [SerializeField]
        protected GameObjectVariable GOcolliderVar;

        private void OnParticleCollision(GameObject other)
        {
            if ((FireOn & ParticleMessageFlags.OnParticleCollision) != 0)
            {
                if (DoesPassFilter(other.tag))
                {
                    if (GOcolliderVar != null)
                    {
                        GOcolliderVar.Value = other;
                    }

                    ExecuteBlock();
                }
            }
        }

        private void OnParticleTrigger()
        {
            if ((FireOn & ParticleMessageFlags.OnParticleTrigger) != 0)
            {
                ExecuteBlock();
            }
        }
    }
}

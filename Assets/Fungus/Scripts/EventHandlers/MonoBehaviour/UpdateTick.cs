using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// The block will execute every chosen Update, or FixedUpdate or LateUpdate.
    /// </summary>
    [EventHandlerInfo("组件回调", "帧更新", "在帧更新时执行所属流程")]
    [AddComponentMenu("")]
    public class UpdateTick : EventHandler
    {
        [System.Flags]
        public enum UpdateMessageFlags
        {
            [UnityEngine.InspectorName("每帧更新")]
            Update = 1 << 0,
            [UnityEngine.InspectorName("固定间隔更新")]
            FixedUpdate = 1 << 1,
            [UnityEngine.InspectorName("每帧更新结束后")]
            LateUpdate = 1 << 2,
        }

        [Tooltip("Which of the Update messages to trigger on.")]
        [SerializeField]
        [EnumFlag]
        [LabelText("触发时机")]
        protected UpdateMessageFlags FireOn = UpdateMessageFlags.Update;

        private void Update()
        {
            if ((FireOn & UpdateMessageFlags.Update) != 0)
            {
                ExecuteBlock();
            }
        }

        private void FixedUpdate()
        {
            if ((FireOn & UpdateMessageFlags.FixedUpdate) != 0)
            {
                ExecuteBlock();
            }
        }

        private void LateUpdate()
        {
            if ((FireOn & UpdateMessageFlags.LateUpdate) != 0)
            {
                ExecuteBlock();
            }
        }
    }
}
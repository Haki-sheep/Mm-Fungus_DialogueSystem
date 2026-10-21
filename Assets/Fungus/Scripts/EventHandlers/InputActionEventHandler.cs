using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

#if ENABLE_INPUT_SYSTEM

using UnityEngine;
using UnityEngine.InputSystem;

namespace Fungus
{
    /// <summary>
    /// Execute a block when a targeted InputAction is performed. Optionally reads the value from the action.
    /// </summary>
    [EventHandlerInfo("玩家输入", "输入动作触发", "在输入动作触发时执行所属流程")]
    [AddComponentMenu("")]
    public class InputActionEventHandler : EventHandler
    {
        [LabelText("输入动作")]
        [SerializeField]
        protected InputActionReference inputAction;

        [VariableProperty()]
        [LabelText("使用的变量")]
        [SerializeField]
        protected Variable variable;

        public virtual void OnEnable()
        {
            inputAction.action.performed += InputAction_performed;
        }

        public virtual void OnDisable()
        {
            inputAction.action.performed -= InputAction_performed;
        }

        private void InputAction_performed(InputAction.CallbackContext obj)
        {
            if (variable != null)
            {
                variable.SetValue(inputAction.action.ReadValueAsObject());
            }

            ExecuteBlock();
        }

        public override string GetSummary()
        {
            if (inputAction == null)
            {
                return "请先设置输入动作";
            }

            return inputAction.action.name;
        }
    }
}

#endif
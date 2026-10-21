using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;

namespace Fungus
{
    /// <summary>
    /// Sets a game object in the scene to be active / inactive.
    /// </summary>
    [CommandInfo("脚本与事件", "启用或隐藏对象", "对配置的目标执行启用或隐藏对象 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class SetActive : Command
    {
        [Tooltip("设置本步骤使用的目标场景对象")]
        [LabelText("目标场景对象")]
        [SerializeField]
        protected GameObjectData _targetGameObject;

        [Tooltip("设置本步骤使用的启用状态")]
        [LabelText("启用状态")]
        [SerializeField]
        protected BooleanData activeState;
    
        #region Public members

        public override void OnEnter()
        {
            if (_targetGameObject.Value != null)
            {
                _targetGameObject.Value.SetActive(activeState.Value);
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (_targetGameObject.Value == null)
            {
                return "请先设置场景对象";
            }

            return _targetGameObject.Value.name + " = " + activeState.GetDescription();
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return _targetGameObject.gameObjectRef == variable || activeState.booleanRef == variable || 
                base.HasReference(variable);
        }

        #endregion

        #region Backwards compatibility

        [HideInInspector] [FormerlySerializedAs("targetGameObject")] public GameObject targetGameObjectOLD;

        protected virtual void OnEnable()
        {
            if (targetGameObjectOLD != null)
            {
                _targetGameObject.Value = targetGameObjectOLD;
                targetGameObjectOLD = null;
            }
        }

        #endregion
    }
}
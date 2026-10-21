using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;

namespace Fungus
{
    /// <summary>
    /// Destroys a specified game object in the scene.
    /// </summary>
    [CommandInfo("脚本与事件", "销毁对象", "对配置的目标执行销毁对象 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class Destroy : Command
    {
        [Tooltip("设置本步骤使用的目标场景对象")]
        [LabelText("目标场景对象")]
        [SerializeField]
        protected GameObjectData _targetGameObject;

        [Tooltip("设置本步骤使用的延迟销毁（秒）")]
        [LabelText("延迟销毁（秒）")]
        [SerializeField]
        protected FloatData destroyInXSeconds = new FloatData(0);

        #region Public members

        public override void OnEnter()
        {
            if (_targetGameObject.Value != null)
            {
                if (destroyInXSeconds.Value != 0)
                    Destroy(_targetGameObject, destroyInXSeconds.Value);
                else
                    Destroy(_targetGameObject.Value);
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (_targetGameObject.Value == null)
            {
                return "请先设置场景对象";
            }

            return _targetGameObject.Value.name + (destroyInXSeconds.Value == 0 ? "" : " 在 " + AuthoringValueLabel.Format(destroyInXSeconds.Value));
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool HasReference(Variable variable)
        {
            if (_targetGameObject.gameObjectRef == variable || destroyInXSeconds.floatRef == variable)
                return true;

            return false;
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
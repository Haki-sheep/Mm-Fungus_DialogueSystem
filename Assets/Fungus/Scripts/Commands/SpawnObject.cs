using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;

namespace Fungus
{
    /// <summary>
    /// Spawns a new object based on a reference to a scene or prefab game object.
    /// </summary>
    [CommandInfo("脚本与事件", "生成对象", "对配置的目标执行生成对象 使用下方参数设置具体行为", 
        Priority = 10)]
    [CommandInfo("场景对象", "创建对象副本", "对配置的目标执行创建对象副本 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class SpawnObject : Command
    {
        [Tooltip("设置本步骤使用的原始对象")]
        [LabelText("原始对象")]
        [SerializeField]
        protected GameObjectData _sourceObject;

        [Tooltip("设置本步骤使用的父级对象")]
        [LabelText("父级对象")]
        [SerializeField]
        protected TransformData _parentTransform;

        [Tooltip("设置本步骤使用的生成在自身位置")]
        [LabelText("生成在自身位置")]
        [SerializeField]
        protected BooleanData _spawnAtSelf = new BooleanData(false);

        [Tooltip("设置本步骤使用的生成位置")]
        [LabelText("生成位置")]
        [SerializeField]
        protected Vector3Data _spawnPosition;

        [Tooltip("设置本步骤使用的生成旋转")]
        [LabelText("生成旋转")]
        [SerializeField]
        protected Vector3Data _spawnRotation;



        [Tooltip("设置本步骤使用的接收新生成对象")]
        [LabelText("接收新生成对象")]
        [SerializeField]
        protected GameObjectData _newlySpawnedObject;

        #region Public members

        public override void OnEnter()
        {
            if (_sourceObject.Value == null)
            {
                Continue();
                return;
            }

            GameObject newObject = null;

            if (_parentTransform.Value != null)
            {
                newObject = GameObject.Instantiate(_sourceObject.Value,_parentTransform.Value);
            }
            else
            {
                newObject = GameObject.Instantiate(_sourceObject.Value);
            }

            if (!_spawnAtSelf.Value)
            {
                newObject.transform.localPosition = _spawnPosition.Value;
                newObject.transform.localRotation = Quaternion.Euler(_spawnRotation.Value);
            }
            else
            {
                newObject.transform.SetPositionAndRotation(transform.position, transform.rotation);
            }

            _newlySpawnedObject.Value = newObject;

            Continue();
        }

        public override string GetSummary()
        {
            if (_sourceObject.Value == null)
            {
                return "请先设置原始场景对象";
            }

            return _sourceObject.Value.name;
        }
        
        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool HasReference(Variable variable)
        {
            if (_sourceObject.gameObjectRef == variable || _parentTransform.transformRef == variable ||
                _spawnAtSelf.booleanRef == variable || _spawnPosition.vector3Ref == variable ||
                _spawnRotation.vector3Ref == variable)
                return true;

            return false;
        }

        #endregion

        #region Backwards compatibility

        [HideInInspector] [FormerlySerializedAs("sourceObject")] public GameObject sourceObjectOLD;
        [HideInInspector] [FormerlySerializedAs("parentTransform")] public Transform parentTransformOLD;
        [HideInInspector] [FormerlySerializedAs("spawnPosition")] public Vector3 spawnPositionOLD;
        [HideInInspector] [FormerlySerializedAs("spawnRotation")] public Vector3 spawnRotationOLD;

        protected virtual void OnEnable()
        {
            if (sourceObjectOLD != null)
            {
                _sourceObject.Value = sourceObjectOLD;
                sourceObjectOLD = null;
            }
            if (parentTransformOLD != null)
            {
                _parentTransform.Value = parentTransformOLD;
                parentTransformOLD = null;
            }
            if (spawnPositionOLD != default(Vector3))
            {
                _spawnPosition.Value = spawnPositionOLD;
                spawnPositionOLD = default(Vector3);
            }
            if (spawnRotationOLD != default(Vector3))
            {
                _spawnRotation.Value = spawnRotationOLD;
                spawnRotationOLD = default(Vector3);
            }
        }

        #endregion
    }
}
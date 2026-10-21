using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections.Generic;

namespace Fungus
{
    /// <summary>
    /// Sets all collider (2d or 3d) components on the target objects to be active / inactive.
    /// </summary>
    [CommandInfo("精灵图片", "设置碰撞体", "对配置的目标执行设置碰撞体 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    public class SetCollider : Command
    {       
        [Tooltip("设置本步骤使用的目标对象列表")]
        [LabelText("目标对象列表")]
        [SerializeField]
        protected List<GameObject> targetObjects = new List<GameObject>();

        [Tooltip("设置本步骤使用的目标标签")]
        [LabelText("目标标签")]
        [SerializeField]
        protected string targetTag = "";

        [Tooltip("设置本步骤使用的启用状态")]
        [LabelText("启用状态")]
        [SerializeField]
        protected BooleanData activeState;

        protected virtual void SetColliderActive(GameObject go)
        {
            if (go != null)     
            {
                // 3D objects
                var colliders = go.GetComponentsInChildren<Collider>();
                for (int i = 0; i < colliders.Length; i++)
                {
                    var c = colliders[i];
                    c.enabled = activeState.Value;
                }

                // 2D objects
                var collider2Ds = go.GetComponentsInChildren<Collider2D>();
                for (int i = 0; i < collider2Ds.Length; i++)
                {
                    var c = collider2Ds[i];
                    c.enabled = activeState.Value;
                }
            }
        }

        #region Public members

        public override void OnEnter()  
        {
            for (int i = 0; i < targetObjects.Count; i++)
            {
                var go = targetObjects[i];
                SetColliderActive(go);
            }

            var taggedObjects = GameObject.FindGameObjectsWithTag(targetTag);

            if (taggedObjects != null)
            {
                for (int i = 0; i < taggedObjects.Length; i++)
                {
                    var go = taggedObjects[i];
                    SetColliderActive(go);
                }
            }

            Continue();
        }

        public override string GetSummary()
        {
            int count = targetObjects.Count;

            if (activeState.Value)
            {
                return "启用" + count + " 个碰撞体对象";
            }
            else
            {
                return "禁用" + count + " 个碰撞体对象";
            }
        }
        
        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255); 
        }

        public override bool IsReorderableArray(string propertyName)
        {
            return propertyName == "targetObjects";
        }

        public override bool HasReference(Variable variable)
        {
            return activeState.booleanRef == variable || base.HasReference(variable);
        }

        #endregion
    }
        
}
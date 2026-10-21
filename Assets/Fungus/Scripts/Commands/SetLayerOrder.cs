using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Sets the Renderer sorting layer of every child of a game object. Applies to all Renderers (including mesh, skinned mesh, and sprite).
    /// </summary>
    [CommandInfo("精灵图片", "设置渲染排序层", "对配置的目标执行设置渲染排序层 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    public class SetSortingLayer : Command 
    {
        [Tooltip("设置本步骤使用的目标对象")]
        [LabelText("目标对象")]
        [SerializeField]
        protected GameObject targetObject;
        
        [Tooltip("设置本步骤使用的排序层")]
        [LabelText("排序层")]
        [SerializeField]
        protected string sortingLayer;

        protected void ApplySortingLayer(Transform target, string layerName) 
        {
            var renderer = target.gameObject.GetComponent<Renderer>();
            if (renderer)
            {
                renderer.sortingLayerName = layerName;
                Debug.Log(target.name);
            }

            var targetTransform = target.transform;
            foreach (Transform child in targetTransform)
            {
                ApplySortingLayer(child, layerName);
            }
        }       

        #region Public members

        public override void OnEnter()
        {
            if (targetObject != null)
            {
                ApplySortingLayer(targetObject.transform, sortingLayer);
            }
            
            Continue();
        }
        
        public override string GetSummary()
        {
            if (targetObject == null)
            {
                return "请先设置场景对象";
            }
            
            return targetObject.name;
        }
        
        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        #endregion
    }
}
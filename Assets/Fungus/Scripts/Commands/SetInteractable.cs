using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Fungus
{
    /// <summary>
    /// Set the interactable state of selectable objects.
    /// </summary>
    [CommandInfo("界面", "设置界面交互状态", "对配置的目标执行设置界面交互状态 使用下方参数设置具体行为")]
    public class SetInteractable : Command 
    {
        [Tooltip("设置本步骤使用的目标对象列表")]
        [LabelText("目标对象列表")]
        [SerializeField]
        protected List<GameObject> targetObjects = new List<GameObject>();

        [Tooltip("设置本步骤使用的允许交互")]
        [LabelText("允许交互")]
        [SerializeField]
        protected BooleanData interactableState = new BooleanData(true);

        #region Public members

        public override void OnEnter()
        {
            if (targetObjects.Count == 0)
            {
                Continue();
                return;
            }

            for (int i = 0; i < targetObjects.Count; i++)
            {
                var targetObject = targetObjects[i];
                var selectables = targetObject.GetComponents<Selectable>();
                for (int j = 0; j < selectables.Length; j++)
                {
                    var selectable = selectables[j];
                    selectable.interactable = interactableState.Value;
                }
            }
                
            Continue();
        }

        public override string GetSummary()
        {
            if (targetObjects.Count == 0)
            {
                return "请先设置目标对象列表";
            }
            else if (targetObjects.Count == 1)
            {
                if (targetObjects[0] == null)
                {
                    return "请先设置目标对象列表";
                }
                return targetObjects[0].name + " = " + interactableState.Value;
            }
            
            string objectList = "";
            for (int i = 0; i < targetObjects.Count; i++)
            {
                var go = targetObjects[i];
                if (go == null)
                {
                    continue;
                }
                if (objectList == "")
                {
                    objectList += go.name;
                }
                else
                {
                    objectList += ", " + go.name;
                }
            }
            
            return objectList + " = " + interactableState.Value;
        }
        
        public override Color GetButtonColor()
        {
            return new Color32(180, 250, 250, 255);
        }

        public override void OnCommandAdded(Block parentBlock)
        {
            targetObjects.Add(null);
        }

        public override bool IsReorderableArray(string propertyName)
        {
            if (propertyName == "targetObjects")
            {
                return true;
            }

            return false;
        }

        public override bool HasReference(Variable variable)
        {
            return interactableState.booleanRef == variable || base.HasReference(variable);
        }

        #endregion
    }
}
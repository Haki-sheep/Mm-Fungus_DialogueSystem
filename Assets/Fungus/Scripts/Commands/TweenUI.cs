using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections.Generic;
using Fungus;
using PrimeTween;

namespace Fungus
{
    /// <summary>
    /// Abstract base class for TweenUI commands.
    /// </summary>
    public abstract class TweenUI : Command 
    {
        [Tooltip("设置本步骤使用的目标对象列表")]
        [LabelText("目标对象列表")]
        [SerializeField]
        protected List<GameObject> targetObjects = new List<GameObject>();
        
        [Tooltip("设置本步骤使用的缓动方式")]
        [LabelText("缓动方式")]
        [SerializeField]
        protected eTweenEase tweenType = eTweenEase.easeOutQuad;
        
        [Tooltip("勾选后等待本步骤完成再继续 不勾选则立即执行下一步")]
        [LabelText("等完成后再执行下一步")]
        [SerializeField]
        protected BooleanData waitUntilFinished = new BooleanData(true);
        
        [Tooltip("设置效果持续多久 单位为秒")]
        [LabelText("持续时间（秒）")]
        [SerializeField]
        protected FloatData duration = new FloatData(1f);

        protected virtual void ApplyTween()
        {
            for (int i = 0; i < targetObjects.Count; i++)
            {
                var targetObject = targetObjects[i];
                if (targetObject == null)
                {
                    continue;
                }
                ApplyTween(targetObject);
            }

            if (waitUntilFinished)
            {
                var Complete = CreateContinueCallback();
                TrackTween(Tween.Delay(this, duration.Value, _ => Complete()));
            }
        }

        protected abstract void ApplyTween(GameObject go);

        protected virtual void OnComplete()
        {
            Continue();
        }

        protected virtual string GetSummaryValue()
        {
            return "";
        }

        #region Public members

        public override void OnEnter()
        {
            if (targetObjects.Count == 0)
            {
                Continue();
                return;
            }
            
            ApplyTween();

            if (!waitUntilFinished)
            {
                Continue();
            }
        }

        public override void OnCommandAdded(Block parentBlock)
        {
            // Add an empty slot by default. Saves an unnecessary user click.
            if (targetObjects.Count == 0)
            {
                targetObjects.Add(null);
            }
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
                return targetObjects[0].name + " = " + GetSummaryValue();
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
            
            return objectList + " = " + GetSummaryValue();
        }
        
        public override Color GetButtonColor()
        {
            return new Color32(180, 250, 250, 255);
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
            return waitUntilFinished.booleanRef == variable || duration.floatRef == variable || base.HasReference(variable);
        }

        #endregion
    }
}
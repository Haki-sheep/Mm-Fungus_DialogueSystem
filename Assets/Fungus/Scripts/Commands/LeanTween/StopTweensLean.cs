using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrimeTween;


namespace Fungus
{
    /// <summary>
    /// 
    /// </summary> 
    [CommandInfo("补间动画", "停止补间动画", "对配置的目标执行停止补间动画 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class StopTweensLean : Command
    {
        [Tooltip("设置本步骤使用的目标对象")]
        [LabelText("目标对象")]
        [SerializeField]
        protected GameObjectData _targetObject;

        public override void OnEnter()
        {
            if (_targetObject.Value != null)
            {
                FungusTween.Stop(_targetObject.Value);
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (_targetObject.Value == null)
            {
                return "请先设置目标对象";
            }

            return "停止这个对象的全部补间动画：" + _targetObject.Value.name;
        }

        public override Color GetButtonColor()
        {
            return new Color32(233, 163, 180, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return _targetObject.gameObjectRef == variable;
        }
    }
}
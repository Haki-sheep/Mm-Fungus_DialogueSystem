using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.UI;

namespace Fungus 
{
    /// <summary>
    /// The block will execute when the user changes the value of target UI slider.
    /// </summary>
    [EventHandlerInfo("界面", "滑动条数值改变", "在滑动条数值改变时执行所属流程")]
    [AddComponentMenu("")]
    public class SliderChanged : EventHandler
    {
        [Tooltip("设置本步骤使用的目标滑动条")]
        [LabelText("目标滑动条")]
        [SerializeField]
        protected Slider targetSlider;

        [Tooltip("设置本步骤使用的接收小数结果")]
        [VariableProperty(typeof(FloatVariable))]
        [LabelText("接收小数结果")]
        [SerializeField]
        protected FloatVariable outFloatVariable;

        public virtual void Start()
        {
            if (targetSlider != null)
            {
                targetSlider.onValueChanged.AddListener(ValueChangedHandler);
            }
        }

        protected virtual void ValueChangedHandler(float arg0)
        {
            if(outFloatVariable != null)
            {
                outFloatVariable.Value = arg0;
            }

            ExecuteBlock();
        }

        public override string GetSummary()
        {
            if (targetSlider != null)
            {
                return targetSlider.name;
            }

            return "请先设置目标滑动条";
        }
    }
}

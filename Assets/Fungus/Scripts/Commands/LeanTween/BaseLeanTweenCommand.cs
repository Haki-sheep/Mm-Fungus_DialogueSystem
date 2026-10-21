using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;
using PrimeTween;

namespace Fungus
{
    /// <summary>
    /// Abstract base class for LeanTween commands.
    /// </summary>
    [ExecuteInEditMode]
    public abstract class BaseLeanTweenCommand : Command
    {
        [Tooltip("设置本步骤使用的目标对象")]
        [LabelText("目标对象")]
        [SerializeField]
        protected GameObjectData _targetObject;

        [Tooltip("设置效果持续多久 单位为秒")]
        [LabelText("持续时间（秒）")]
        [SerializeField]
        protected FloatData _duration = new FloatData(1f);

        public enum ToFrom { [UnityEngine.InspectorName("到目标值")]
        To, [UnityEngine.InspectorName("从指定值开始")]
        From }
        [Tooltip("设置本步骤使用的到目标或从目标")]
        [LabelText("到目标或从目标")]
        [SerializeField]
        protected ToFrom _toFrom;
        public bool IsInFromMode { get { return _toFrom == ToFrom.From; } }

        public enum AbsAdd { [UnityEngine.InspectorName("使用绝对值")]
        Absolute, [UnityEngine.InspectorName("叠加")]
        Additive }
        [Tooltip("设置本步骤使用的绝对值或增量")]
        [LabelText("绝对值或增量")]
        [SerializeField]
        protected AbsAdd _absAdd;
        public bool IsInAddativeMode { get { return _absAdd == AbsAdd.Additive; } }


        [Tooltip("设置本步骤使用的缓动曲线")]
        [LabelText("缓动曲线")]
        [SerializeField]
        protected eTweenEase easeType = eTweenEase.easeInOutQuad;
        
        [Tooltip("设置本步骤使用的循环方式")]
        [LabelText("循环方式")]
        [SerializeField]
        protected eTweenLoop loopType = eTweenLoop.Once;

        [Tooltip("设置本步骤使用的重复次数")]
        [LabelText("重复次数")]
        [SerializeField]
        protected int repeats = 0;

        [Tooltip("开始新补间动画前取消同一对象上已有的补间动画")]
        [LabelText("开始前停止已有动画")]
        [SerializeField]
        protected bool stopPreviousTweens = false;

        [Tooltip("勾选后等待本步骤完成再继续 不勾选则立即执行下一步")]
        [LabelText("等完成后再执行下一步")]
        [SerializeField]
        protected bool waitUntilFinished = true;


        [HideInInspector] protected Tween ourTween;


        protected virtual void OnTweenComplete()
        {
            Continue();
        }

        #region Public members

        public override void OnEnter()
        {
            if (_targetObject.Value == null)
            {
                Continue();
                return;
            }

            if (stopPreviousTweens)
            {
                FungusTween.Stop(_targetObject.Value);
            }

            ourTween = TrackTween(ExecuteTween());

            if (waitUntilFinished)
            {
                if(ourTween.isAlive)
                {
                    ourTween.OnComplete(CreateContinueCallback());
                }
            }
            else
            {
                Continue();
            }
        }

        protected TweenSettings CreateTweenSettings() => new TweenSettings(_duration.Value, TweenEasing.Get(easeType),
            loopType == eTweenLoop.Once ? 1 : repeats <= 0 ? -1 : repeats,
            loopType == eTweenLoop.PingPong ? CycleMode.Rewind : CycleMode.Restart);

        public abstract Tween ExecuteTween();

        public override string GetSummary()
        {
            if (_targetObject.Value == null)
            {
                return "请先设置目标对象";
            }

            return _targetObject.Value.name + " 持续 " + _duration.Value + " 秒";
        }

        public override Color GetButtonColor()
        {
            return new Color32(233, 163, 180, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return variable == _targetObject.gameObjectRef || variable == _duration.floatRef;
        }

        #endregion

    }
}

using Fungus.Inspector;
using PrimeTween;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;

namespace Fungus
{
    /// <summary>
    /// 保留旧命令字段布局 使用 PrimeTween 执行动画
    /// </summary>
    [ExecuteInEditMode]
    public abstract class iTweenCommand : Command
    {
        [Tooltip("设置本步骤使用的目标对象")]
        [LabelText("目标对象")]
        [SerializeField]
        protected GameObjectData _targetObject;

        [Tooltip("设置本步骤使用的动画名称")]
        [LabelText("动画名称")]
        [SerializeField]
        protected StringData _tweenName;

        [Tooltip("设置效果持续多久 单位为秒")]
        [LabelText("持续时间（秒）")]
        [SerializeField]
        protected FloatData _duration = new FloatData(1f);

        [Tooltip("设置本步骤使用的缓动曲线")]
        [LabelText("缓动曲线")]
        [SerializeField]
        protected eLegacyTweenEase easeType = eLegacyTweenEase.easeInOutQuad;

        [Tooltip("设置本步骤使用的循环方式")]
        [LabelText("循环方式")]
        [SerializeField]
        protected eLegacyTweenLoop loopType = eLegacyTweenLoop.None;

        [Tooltip("开始新补间动画前取消同一对象上已有的补间动画")]
        [LabelText("开始前停止已有动画")]
        [SerializeField]
        protected bool stopPreviousTweens = false;

        [Tooltip("勾选后等待本步骤完成再继续 不勾选则立即执行下一步")]
        [LabelText("等完成后再执行下一步")]
        [SerializeField]
        protected bool waitUntilFinished = true;

        protected virtual void OniTweenComplete(object param)
        {
            Command command = param as Command;
            if (command != null && command.Equals(this))
            {
                if (waitUntilFinished)
                {
                    Continue();
                }
            }
        }

        #region Public members

        public override void OnEnter()
        {
            if (_targetObject.Value == null)
            {
                Continue();
                return;
            }
            if (stopPreviousTweens) FungusTween.Stop(_targetObject.Value, true);
            var Tween = TrackTween(FungusTween.Track(_targetObject.Value, DoTween(), _tweenName.Value));
            if (waitUntilFinished) Tween.OnComplete(CreateContinueCallback());
            else Continue();
        }

        protected TweenSettings CreateTweenSettings() => new TweenSettings(_duration.Value, TweenEasing.Get(easeType),
            loopType == eLegacyTweenLoop.None ? 1 : -1,
            loopType == eLegacyTweenLoop.PingPong ? CycleMode.Rewind : CycleMode.Restart);

        public abstract Tween DoTween();

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
            return _targetObject.gameObjectRef == variable || _tweenName.stringRef == variable ||
                base.HasReference(variable);
        }

        #endregion

        #region Backwards compatibility

        [HideInInspector] [FormerlySerializedAs("target")] [FormerlySerializedAs("targetObject")] public GameObject targetObjectOLD;
        [HideInInspector] [FormerlySerializedAs("tweenName")] public string tweenNameOLD = "";
        [HideInInspector] [FormerlySerializedAs("duration")] public float durationOLD;

        protected virtual void OnEnable()
        {
            if (targetObjectOLD != null)
            {
                _targetObject.Value = targetObjectOLD;
                targetObjectOLD = null;
            }

            if (tweenNameOLD != "")
            {
                _tweenName.Value = tweenNameOLD;
                tweenNameOLD = "";
            }

            if (durationOLD != default(float))
            {
                _duration.Value = durationOLD;
                durationOLD = default(float);
            }       
        }

        #endregion
    }
}

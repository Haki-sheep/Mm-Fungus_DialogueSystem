using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;
using PrimeTween;

namespace Fungus
{
    /// <summary>
    /// Moves a game object by a specified offset over time.
    /// </summary>
    [CommandInfo("旧版补间动画", "按增量移动", "对配置的目标执行按增量移动 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    [System.Obsolete("Deprecated, 底层已迁移到 PrimeTween 建议新剧情使用补间动画分类中的命令")]
    [ExecuteInEditMode]
    public class MoveAdd : iTweenCommand
    {
        [Tooltip("设置本步骤使用的偏移量")]
        [LabelText("偏移量")]
        [SerializeField]
        protected Vector3Data _offset;

        [Tooltip("设置本步骤使用的使用的坐标系")]
        [LabelText("使用的坐标系")]
        [SerializeField]
        protected Space space = Space.Self;

        #region Public members

        public override Tween DoTween()
        {
            var Target = _targetObject.Value.transform;
            var Settings = CreateTweenSettings();
            var Start = Target.position;
            var End = Start + (space == Space.Self ? Target.TransformDirection(_offset.Value) : _offset.Value);
            return Tween.Position(Target, new TweenSettings<Vector3>(End, Settings));
        }

        public override bool HasReference(Variable variable)
        {
            return _offset.vector3Ref == variable ||
                base.HasReference(variable);
        }

        #endregion

        #region Backwards compatibility

        [HideInInspector] [FormerlySerializedAs("offset")] public Vector3 offsetOLD;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (offsetOLD != default(Vector3))
            {
                _offset.Value = offsetOLD;
                offsetOLD = default(Vector3);
            }
        }

        #endregion
    }
}
using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;

namespace Fungus
{
    /// <summary>
    /// Makes a sprite visible / invisible by setting the color alpha.
    /// </summary>
    [CommandInfo("精灵图片", "显示或隐藏精灵", "对配置的目标执行显示或隐藏精灵 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class ShowSprite : Command
    {
        [Tooltip("设置本步骤使用的精灵渲染器")]
        [LabelText("精灵渲染器")]
        [SerializeField]
        protected SpriteRenderer spriteRenderer;

        [Tooltip("设置本步骤使用的是否显示")]
        [LabelText("是否显示")]
        [SerializeField]
        protected BooleanData _visible = new BooleanData(false);

        [Tooltip("设置本步骤使用的同时影响子对象")]
        [LabelText("同时影响子对象")]
        [SerializeField]
        protected bool affectChildren = true;

        protected virtual void SetSpriteAlpha(SpriteRenderer renderer, bool visible)
        {
            Color spriteColor = renderer.color;
            spriteColor.a = visible ? 1f : 0f;
            renderer.color = spriteColor;
        }

        #region Public members

        public override void OnEnter()
        {
            if (spriteRenderer != null)
            {
                if (affectChildren)
                {
                    var spriteRenderers = spriteRenderer.gameObject.GetComponentsInChildren<SpriteRenderer>();
                    for (int i = 0; i < spriteRenderers.Length; i++)
                    {
                        var sr = spriteRenderers[i];
                        SetSpriteAlpha(sr, _visible.Value);
                    }
                }
                else
                {
                    SetSpriteAlpha(spriteRenderer, _visible.Value);
                }
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (spriteRenderer == null)
            {
                return "请先设置精灵渲染器";
            }

            return spriteRenderer.name + " 到 " + (_visible.Value ? "显示" : "隐藏");
        }

        public override Color GetButtonColor()
        {
            return new Color32(221, 184, 169, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return _visible.booleanRef == variable || base.HasReference(variable);
        }

        #endregion

        #region Backwards compatibility

        [HideInInspector] [FormerlySerializedAs("visible")] public bool visibleOLD;

        protected virtual void OnEnable()
        {
            if (visibleOLD != default(bool))
            {
                _visible.Value = visibleOLD;
                visibleOLD = default(bool);
            }
        }

        #endregion
    }
}
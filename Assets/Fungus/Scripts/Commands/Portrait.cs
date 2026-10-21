using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Controls a character portrait.
    /// </summary>
    [CommandInfo("剧情对白", "显示或切换立绘", "显示 隐藏 替换或移动角色立绘")]
    public class Portrait : ControlWithDisplay<DisplayType>
    {
        [Tooltip("设置本步骤使用的立绘舞台")]
        [LabelText("立绘舞台")]
        [SerializeField]
        protected Stage stage;

        [Tooltip("选择本句发言的角色 决定姓名颜色和角色音效")]
        [LabelText("说话角色")]
        [SerializeField]
        protected Character character;

        [Tooltip("设置本步骤使用的被替换角色")]
        [LabelText("被替换角色")]
        [SerializeField]
        protected Character replacedCharacter;

        [Tooltip("设置本步骤使用的角色表情")]
        [LabelText("角色表情")]
        [SerializeField]
        protected Sprite portrait;

        [Tooltip("设置本步骤使用的偏移量")]
        [LabelText("偏移量")]
        [SerializeField]
        protected PositionOffset offset;

        [Tooltip("设置本步骤使用的起始站位")]
        [LabelText("起始站位")]
        [SerializeField]
        protected RectTransform fromPosition;

        [Tooltip("设置本步骤使用的目标站位")]
        [LabelText("目标站位")]
        [SerializeField]
        protected RectTransform toPosition;

        [Tooltip("设置本步骤使用的朝向")]
        [LabelText("朝向")]
        [SerializeField]
        protected FacingDirection facing;

        [Tooltip("使用舞台组件中配置的时间和表现参数")]
        [LabelText("使用舞台默认设置")]
        [SerializeField]
        protected bool useDefaultSettings = true;

        [Tooltip("设置本步骤使用的淡入淡出时间（秒）")]
        [LabelText("淡入淡出时间（秒）")]
        [SerializeField]
        protected float fadeDuration = 0.5f;

        [Tooltip("设置本步骤使用的移动时间（秒）")]
        [LabelText("移动时间（秒）")]
        [SerializeField]
        protected float moveDuration = 1f;

        [Tooltip("设置本步骤使用的入场偏移量")]
        [LabelText("入场偏移量")]
        [SerializeField]
        protected Vector2 shiftOffset;

        [Tooltip("设置本步骤使用的移动到目标站位")]
        [LabelText("移动到目标站位")]
        [SerializeField]
        protected bool move;

        [Tooltip("设置本步骤使用的入场时滑入站位")]
        [LabelText("入场时滑入站位")]
        [SerializeField]
        protected bool shiftIntoPlace;

        [Tooltip("勾选后等待本步骤完成再继续 不勾选则立即执行下一步")]
        [LabelText("等完成后再执行下一步")]
        [SerializeField]
        protected bool waitUntilFinished = false;

        #region Public members

        /// <summary>
        /// Stage to display portrait on.
        /// </summary>
        public virtual Stage _Stage { get { return stage; } set { stage = value; } }

        /// <summary>
        /// Character to display.
        /// </summary>
        public virtual Character _Character { get { return character; } set { character = value; } }

        /// <summary>
        /// Portrait to display.
        /// </summary>
        public virtual Sprite _Portrait { get { return portrait; } set { portrait = value; } }

        /// <summary>
        /// Move the portrait from/to this offset position.
        /// </summary>
        public virtual PositionOffset Offset { get { return offset; } set { offset = value; } }

        /// <summary>
        /// Move the portrait from this position.
        /// </summary>
        public virtual RectTransform FromPosition { get { return fromPosition; } set { fromPosition = value;} }

        /// <summary>
        /// Move the portrait to this position.
        /// </summary>
        public virtual RectTransform ToPosition { get { return toPosition; } set { toPosition = value;} }

        /// <summary>
        /// Direction character is facing.
        /// </summary>
        public virtual FacingDirection Facing { get { return facing; } set { facing = value; } }

        /// <summary>
        /// Use Default Settings.
        /// </summary>
        public virtual bool UseDefaultSettings { get { return useDefaultSettings; } set { useDefaultSettings = value; } }

        /// <summary>
        /// Move portrait into new position.
        /// </summary>
        public virtual bool Move { get { return move; } set { move = value; } }

        /// <summary>
        /// Start from offset position.
        /// </summary>
        public virtual bool ShiftIntoPlace { get { return shiftIntoPlace; } set { shiftIntoPlace = value; } }

        public override void OnEnter()
        {
            // Selected "use default Portrait Stage"
            if (stage == null)
            {
                // If no default specified, try to get any portrait stage in the scene
                stage = Stage.GetActiveStage();
                // If portrait stage does not exist, do nothing
                if (stage == null)
                {
                    Continue();
                    return;
                }

            }

            // If no display specified, do nothing
            if (IsDisplayNone(display))
            {
                Continue();
                return;
            }

            PortraitOptions options = new PortraitOptions();
            
            options.character = character;
            options.replacedCharacter = replacedCharacter;
            options.portrait = portrait;
            options.display = display;
            options.offset = offset;
            options.fromPosition = fromPosition;
            options.toPosition = toPosition;
            options.facing = facing;
            options.useDefaultSettings = useDefaultSettings;
            options.fadeDuration = fadeDuration;
            options.moveDuration = moveDuration;
            options.shiftOffset = shiftOffset;
            options.move = move;
            options.shiftIntoPlace = shiftIntoPlace;
            options.waitUntilFinished = waitUntilFinished;

            stage.RunPortraitCommand(options, CreateContinueCallback());
            
        }
        
        public override string GetSummary()
        {
            if (display == DisplayType.None && character == null)
            {
                return "请先设置角色或立绘操作";
            }
            else if (display == DisplayType.None)
            {
                return "请先设置显示操作";
            }
            else if (character == null)
            {
                return "请先设置角色";
            }

            string displaySummary = "";
            string characterSummary = "";
            string fromPositionSummary = "";
            string toPositionSummary = "";
            string stageSummary = "";
            string portraitSummary = "";
            string facingSummary = "";
            
            displaySummary = StringFormatter.SplitCamelCase(AuthoringValueLabel.Format(display));

            if (display == DisplayType.Replace)
            {
                if (replacedCharacter != null)
                {
                    displaySummary += " \"" + replacedCharacter.name + "\" with";
                }
            }

            characterSummary = character.name;
            if (stage != null)
            {
                stageSummary = " on \"" + stage.name + "\"";
            }
            
            if (portrait != null)
            {
                portraitSummary = " " + portrait.name;
            }

            if (shiftIntoPlace)
            {
                if (offset != 0)
                {
                    fromPositionSummary = AuthoringValueLabel.Format(offset);
                    fromPositionSummary = " 从 " + "\"" + fromPositionSummary + "\"";
                }
            }
            else if (fromPosition != null)
            {
                fromPositionSummary = " 从 " + "\"" + fromPosition.name + "\"";
            }

            if (toPosition != null)
            {
                string toPositionPrefixSummary = "";
                if (move)
                {
                    toPositionPrefixSummary = " 到 ";
                }
                else
                {
                    toPositionPrefixSummary = " 在 ";
                }

                toPositionSummary = toPositionPrefixSummary + "\"" + toPosition.name + "\"";
            }

            if (facing != FacingDirection.None)
            {
                if (facing == FacingDirection.Left)
                {
                    facingSummary = "<--";
                }
                if (facing == FacingDirection.Right)
                {
                    facingSummary = "-->";
                }

                facingSummary = " facing \"" + facingSummary + "\"";
            }

            return displaySummary + " \"" + characterSummary + portraitSummary + "\"" + stageSummary + facingSummary + fromPositionSummary + toPositionSummary;
        }
        
        public override Color GetButtonColor()
        {
            return new Color32(230, 200, 250, 255);
        }
        
        public override void OnCommandAdded(Block parentBlock)
        {
            //Default to display type: show
            display = DisplayType.Show;
        }

        #endregion
    }
}

using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using PrimeTween;

namespace Fungus
{
    /// <summary>
    /// Supported display operations for Stage.
    /// </summary>
    public enum StageDisplayType
    {
        /// <summary> No operation </summary>
        [UnityEngine.InspectorName("不设置")]
        None,
        /// <summary> Show the stage and all portraits. </summary>
        [UnityEngine.InspectorName("显示")]
        Show,
        /// <summary> Hide the stage and all portraits. </summary>
        [UnityEngine.InspectorName("隐藏")]
        Hide,
        /// <summary> Swap the stage and all portraits with another stage. </summary>
        [UnityEngine.InspectorName("交换")]
        Swap,
        /// <summary> Move stage to the front. </summary>
        [UnityEngine.InspectorName("移到最前面")]
        MoveToFront,
        /// <summary> Undim all portraits on the stage. </summary>
        [UnityEngine.InspectorName("恢复所有立绘亮度")]
        UndimAllPortraits,
        /// <summary> Dim all non-speaking portraits on the stage. </summary>
        [UnityEngine.InspectorName("未发言角色变暗")]
        DimNonSpeakingPortraits
    }

    /// <summary>
    /// Controls the stage on which character portraits are displayed.
    /// </summary>
    [CommandInfo("剧情对白", "控制立绘舞台", "对配置的目标执行控制立绘舞台 使用下方参数设置具体行为")]
    public class ControlStage : ControlWithDisplay<StageDisplayType> 
    {
        [Tooltip("设置本步骤使用的立绘舞台")]
        [LabelText("立绘舞台")]
        [SerializeField]
        protected Stage stage;
        public virtual Stage _Stage { get { return stage; } }

        [Tooltip("设置本步骤使用的被替换舞台")]
        [LabelText("被替换舞台")]
        [SerializeField]
        protected Stage replacedStage;

        [Tooltip("使用舞台组件中配置的时间和表现参数")]
        [LabelText("使用舞台默认设置")]
        [SerializeField]
        protected bool useDefaultSettings = true;
        public virtual bool UseDefaultSettings { get { return useDefaultSettings; } }

        [Tooltip("设置本步骤使用的淡入淡出时间（秒）")]
        [LabelText("淡入淡出时间（秒）")]
        [SerializeField]
        protected float fadeDuration;
        
        [Tooltip("勾选后等待本步骤完成再继续 不勾选则立即执行下一步")]
        [LabelText("等完成后再执行下一步")]
        [SerializeField]
        protected bool waitUntilFinished = false;

        protected virtual void Show(Stage stage, bool visible, bool CompleteCommand = true) 
        {
            float duration = fadeDuration;
            float targetAlpha = visible ? 1f : 0f;

            CanvasGroup canvasGroup = stage.GetComponentInChildren<CanvasGroup>();
            if (canvasGroup == null)
            {
                if (CompleteCommand && waitUntilFinished) Continue();
                return;
            }

            var Complete = CompleteCommand && waitUntilFinished ? CreateContinueCallback() : (System.Action)null;
            TrackTween(FungusTween.Track(canvasGroup.gameObject,
                Tween.Alpha(canvasGroup, targetAlpha, duration))).OnComplete(Complete);
        }

        protected virtual void MoveToFront(Stage stage)
        {
            var activeStages = Stage.ActiveStages;
            for (int i = 0; i < activeStages.Count; i++)
            {
                var s = activeStages[i];
                if (s == stage)
                {
                    s.PortraitCanvas.sortingOrder = 1;
                }
                else
                {
                    s.PortraitCanvas.sortingOrder = 0;
                }
            }
        }

        protected virtual void UndimAllPortraits(Stage stage) 
        {
            stage.DimPortraits = false;
            var charactersOnStage = stage.CharactersOnStage;
            for (int i = 0; i < charactersOnStage.Count; i++)
            {
                var character = charactersOnStage[i];
                stage.SetDimmed(character, false);
            }
        }

        protected virtual void DimNonSpeakingPortraits(Stage stage) 
        {
            stage.DimPortraits = true;
        }

        protected virtual void OnComplete() 
        {
            if (waitUntilFinished)
            {
                Continue();
            }
        }

        #region Public members

        public override void OnEnter()
        {
            // If no display specified, do nothing
            if (IsDisplayNone(display))
            {
                Continue();
                return;
            }

            // Selected "use default Portrait Stage"
            if (stage == null)           
            {
                // If no default specified, try to get any portrait stage in the scene
            #if UNITY_6000
                stage = GameObject.FindFirstObjectByType<Stage>();
            #else
                stage = FindObjectOfType<Stage>();
            #endif

                // If portrait stage does not exist, do nothing
                if (stage == null)
                {
                    Continue();
                    return;
                }
            }
           
            // Selected "use default Portrait Stage"
            if (display == StageDisplayType.Swap)            // Default portrait stage selected
            {
                if (replacedStage == null)        // If no default specified, try to get any portrait stage in the scene
                {
                #if UNITY_6000
                    replacedStage = GameObject.FindFirstObjectByType<Stage>();
                #else
                    replacedStage = GameObject.FindObjectOfType<Stage>();
                #endif
                }
                // If portrait stage does not exist, do nothing
                if (replacedStage == null)
                {
                    Continue();
                    return;
                }
            }
            // Use default settings
            if (useDefaultSettings)
            {
                fadeDuration = stage.FadeDuration;
            }
            switch(display)
            {
            case (StageDisplayType.Show):
                Show(stage, true);
                break;
            case (StageDisplayType.Hide):
                Show(stage, false);
                break;
            case (StageDisplayType.Swap):
                Show(stage, true, false);
                Show(replacedStage, false);
                break;
            case (StageDisplayType.MoveToFront):
                MoveToFront(stage);
                break;
            case (StageDisplayType.UndimAllPortraits):
                UndimAllPortraits(stage);
                break;
            case (StageDisplayType.DimNonSpeakingPortraits):
                DimNonSpeakingPortraits(stage);
                break;
            }

            bool HasFade = display == StageDisplayType.Show || display == StageDisplayType.Hide || display == StageDisplayType.Swap;
            if (!waitUntilFinished || !HasFade)
            {
                Continue();
            }
        }

        public override string GetSummary()
        {
            string displaySummary = "";
            if (display != StageDisplayType.None)
            {
                displaySummary = StringFormatter.SplitCamelCase(AuthoringValueLabel.Format(display));
            }
            else
            {
                return "请先设置显示操作";
            }
            string stageSummary = "";
            if (stage != null)
            {
                stageSummary = " \"" + stage.name + "\"";
            }
            return displaySummary + stageSummary;
        }
        
        public override Color GetButtonColor()
        {
            return new Color32(230, 200, 250, 255);
        }

        public override void OnCommandAdded(Block parentBlock)
        {
            //Default to display type: show
            display = StageDisplayType.Show;
        }

        #endregion
    }
}
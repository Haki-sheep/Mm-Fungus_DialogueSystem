using Cysharp.Threading.Tasks;
using System.Threading;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using PrimeTween;

namespace Fungus
{
    /// <summary>
    /// Types of display operations supported by portraits.
    /// </summary>
    public enum DisplayType
    {
        /// <summary> Do nothing. </summary>
        [UnityEngine.InspectorName("不设置")]
        None,
        /// <summary> Show the portrait. </summary>
        [UnityEngine.InspectorName("显示")]
        Show,
        /// <summary> Hide the portrait. </summary>
        [UnityEngine.InspectorName("隐藏")]
        Hide,
        /// <summary> Replace the existing portrait. </summary>
        [UnityEngine.InspectorName("替换")]
        Replace,
        /// <summary> Move portrait to the front. </summary>
        [UnityEngine.InspectorName("移到最前面")]
        MoveToFront
    }

    /// <summary>
    /// Directions that character portraits can face.
    /// </summary>
    public enum FacingDirection
    {
        /// <summary> Unknown direction </summary>
        [UnityEngine.InspectorName("不设置")]
        None,
        /// <summary> Facing left. </summary>
        [UnityEngine.InspectorName("朝左")]
        Left,
        /// <summary> Facing right. </summary>
        [UnityEngine.InspectorName("朝右")]
        Right
    }

    /// <summary>
    /// Offset direction for position.
    /// </summary>
    public enum PositionOffset
    {
        /// <summary> Unknown offset direction. </summary>
        [UnityEngine.InspectorName("不设置")]
        None,
        /// <summary> Offset applies to the left. </summary>
        [UnityEngine.InspectorName("向左偏移")]
        OffsetLeft,
        /// <summary> Offset applies to the right. </summary>
        [UnityEngine.InspectorName("向右偏移")]
        OffsetRight
    }


    [AddComponentMenu("")]
    /// <summary>
    /// Controls the Portrait sprites on stage. 
    /// 
    /// Is only really used via Stage, it's child class. This class continues to exist to support existing API
    /// dependant code. All functionality is stage dependant.
    /// </summary>
    public class PortraitController : MonoBehaviour
    {
        private readonly AsyncOperationScope asyncScope = new AsyncOperationScope();
        private readonly Dictionary<Image, Tween> colorTweenDict = new Dictionary<Image, Tween>();
        private readonly List<Tween> portraitTweenList = new List<Tween>();

        public void CancelPendingOperations()
        {
            asyncScope.CancelAll();
            foreach (var Animation in portraitTweenList) Animation.Stop();
            portraitTweenList.Clear();
            colorTweenDict.Clear();
            waitTimer = 0f;
        }

        // Timer for waitUntilFinished functionality
        protected float waitTimer;

        protected Stage stage;

        protected virtual void Awake()
        {
            stage = GetComponentInParent<Stage>();
        }

        protected virtual void FinishCommand(PortraitOptions options)
        {
            bool HasFade = options.character.State.display == DisplayType.Show || options.character.State.display == DisplayType.Hide;
            float FadeDuration = HasFade ? options.fadeDuration : 0f;
            float MoveDuration = HasFade && (options.move || options.shiftIntoPlace) ? options.moveDuration : 0f;
            float Duration = Mathf.Max(FadeDuration, MoveDuration);
            if (options.onComplete != null)
            {
                if (!options.waitUntilFinished)
                {
                    options.onComplete();
                }
                else
                {
                    asyncScope.Run(this, Token => WaitUntilFinishedAsync(Duration, options.onComplete, Token)).Forget();
                }
            }
            else
            {
                asyncScope.Run(this, Token => WaitUntilFinishedAsync(Duration, CancellationToken: Token)).Forget();
            }
        }

        /// <summary>
        /// Makes sure all options are set correctly so it won't break whatever command it's sent to
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        protected virtual PortraitOptions CleanPortraitOptions(PortraitOptions options)
        {
            // Use default stage settings
            if (options.useDefaultSettings)
            {
                options.fadeDuration = stage.FadeDuration;
                options.moveDuration = stage.MoveDuration;
                options.shiftOffset = stage.ShiftOffset;
            }

            // if no previous portrait, use default portrait
            if (options.character.State.portrait == null)
            {
                options.character.State.SetPortraitImageBySprite(options.character.ProfileSprite);
            }

            // Selected "use previous portrait"
            if (options.portrait == null)
            {
                options.portrait = options.character.State.portrait;
            }

            // if no previous position, use default position
            if (options.character.State.position == null)
            {
                options.character.State.position = stage.DefaultPosition.rectTransform;
            }

            // Selected "use previous position"
            if (options.toPosition == null)
            {
                options.toPosition = options.character.State.position;
            }

            if (options.replacedCharacter != null)
            {
                // if no previous position, use default position
                if (options.replacedCharacter.State.position == null)
                {
                    options.replacedCharacter.State.position = stage.DefaultPosition.rectTransform;
                }
            }

            // If swapping, use replaced character's position
            if (options.display == DisplayType.Replace)
            {
                options.toPosition = options.replacedCharacter.State.position;
            }

            // Selected "use previous position"
            if (options.fromPosition == null)
            {
                options.fromPosition = options.character.State.position;
            }

            // if portrait not moving, use from position is same as to position
            if (!options.move)
            {
                options.fromPosition = options.toPosition;
            }

            if (options.display == DisplayType.Hide)
            {
                options.fromPosition = options.character.State.position;
            }

            // if no previous facing direction, use default facing direction
            if (options.character.State.facing == FacingDirection.None)
            {
                options.character.State.facing = options.character.PortraitsFace;
            }

            // Selected "use previous facing direction"
            if (options.facing == FacingDirection.None)
            {
                options.facing = options.character.State.facing;
            }

            if (options.character.State.portraitImage == null)
            {
                CreatePortraitObject(options.character, options.fadeDuration);
            }

            return options;
        }

        /// <summary>
        /// Creates and sets the portrait image for a character
        /// </summary>
        /// <param name="character"></param>
        /// <param name="fadeDuration"></param>
        protected virtual void CreatePortraitObject(Character character, float fadeDuration)
        {
            if (character.State.holder == null)
            {
                character.State.holder = new GameObject(character.name + " holder",
                                                   typeof(RectTransform)
                                                   //typeof(CanvasRenderer),
                                                   //typeof(Image)
                                                   ).GetComponent<RectTransform>();

                // Set it to be a child of the stage
                character.State.holder.transform.SetParent(stage.PortraitCanvas.transform, false);

                SetRectTransform(character.State.holder, stage.DefaultPosition.GetComponent<RectTransform>());
            }

            if (character.State.allPortraits.Count == 0)
            {
                foreach (var item in character.Portraits)
                {
                    if(item == null)
                    {
                        Debug.LogError("null in portrait list on character " + character.name);
                        continue;
                    }
                    // Create a new portrait object
                    GameObject po = new GameObject(item.name,
                                                            typeof(RectTransform),
                                                            typeof(CanvasRenderer),
                                                            typeof(Image));

                    // Set it to be a child of the stage
                    po.transform.SetParent(character.State.holder, false);

                    // Configure the portrait image
                    Image pi = po.GetComponent<Image>();
                    pi.preserveAspect = true;
                    pi.sprite = item;
                    pi.color = new Color(1f, 1f, 1f, 0f);

                    if (item == character.ProfileSprite)
                    {
                        character.State.portraitImage = pi;
                    }

                    //expand to fit parent
                    RectTransform rt = po.GetComponent<RectTransform>();
                    rt.sizeDelta = Vector2.zero;
                    rt.anchorMin = Vector2.zero;
                    rt.anchorMax = Vector2.one;
                    rt.pivot = Vector2.one * 0.5f;
                    rt.ForceUpdateRectTransforms();

                    po.SetActive(false);

                    character.State.allPortraits.Add(pi);
                }
            }
        }

        protected virtual async UniTask WaitUntilFinishedAsync(float duration, Action onComplete = null, CancellationToken CancellationToken = default)
        {
            // Each operation keeps its own timer so simultaneous portraits do not shorten one another
            float RemainingDuration = duration;
            while (RemainingDuration > 0f)
            {
                waitTimer = RemainingDuration;
                await UniTask.NextFrame(cancellationToken: CancellationToken);
                RemainingDuration -= Time.deltaTime;
            }
            waitTimer = 0f;

            // Wait until next frame just to be safe
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, CancellationToken);

            if (onComplete != null)
            {
                onComplete();
            }
        }

        protected virtual void SetupPortrait(PortraitOptions options)
        {
            if (options.character.State.holder == null)
                return;

            SetRectTransform(options.character.State.holder, options.fromPosition);

            if (options.character.State.facing != options.character.PortraitsFace)
            {
                options.character.State.holder.localScale = new Vector3(-1f, 1f, 1f);
            }
            else
            {
                options.character.State.holder.localScale = new Vector3(1f, 1f, 1f);
            }

            if (options.facing != options.character.PortraitsFace)
            {
                options.character.State.holder.localScale = new Vector3(-1f, 1f, 1f);
            }
            else
            {
                options.character.State.holder.localScale = new Vector3(1f, 1f, 1f);
            }
        }

        protected virtual void DoMoveTween(PortraitOptions options)
        {
            CleanPortraitOptions(options);

            FungusTween.Stop(options.character.State.holder.gameObject);

            float duration = options.move || options.shiftIntoPlace ? options.moveDuration : 0f;

            // The stage positions and holder use world coordinates
            TrackPortraitTween(FungusTween.Move(options.character.State.holder.gameObject, options.toPosition.position, duration, stage.FadeEaseType));

            if (options.waitUntilFinished)
            {
                waitTimer = duration;
            }
        }

        /// <summary>
        /// Performs a deep copy of all values from one RectTransform to another.
        /// </summary>
        public static void SetRectTransform(RectTransform target, RectTransform from)
        {
            target.eulerAngles = from.eulerAngles;
            target.position = from.position;
            target.rotation = from.rotation;
            target.anchoredPosition = from.anchoredPosition;
            target.sizeDelta = from.sizeDelta;
            target.anchorMax = from.anchorMax;
            target.anchorMin = from.anchorMin;
            target.pivot = from.pivot;
            target.localScale = from.localScale;
        }

        /// <summary>
        /// Using all portrait options available, run any portrait command.
        /// </summary>
        /// <param name="options">Portrait Options</param>
        /// <param name="onComplete">The function that will run once the portrait command finishes</param>
        public virtual void RunPortraitCommand(PortraitOptions options, Action onComplete)
        {
            waitTimer = 0f;

            // If no character specified, do nothing
            if (options.character == null)
            {
                onComplete();
                return;
            }

            // If Replace and no replaced character specified, do nothing
            if (options.display == DisplayType.Replace && options.replacedCharacter == null)
            {
                onComplete();
                return;
            }

            // Early out if hiding a character that's already hidden
            if (options.display == DisplayType.Hide &&
                !options.character.State.onScreen)
            {
                onComplete();
                return;
            }

            options = CleanPortraitOptions(options);
            options.onComplete = onComplete;

            switch (options.display)
            {
                case (DisplayType.Show):
                    Show(options);
                    break;

                case (DisplayType.Hide):
                    Hide(options);
                    break;

                case (DisplayType.Replace):
                    Show(options);
                    Hide(options.replacedCharacter, options.replacedCharacter.State.position.name);
                    break;

                case (DisplayType.MoveToFront):
                    MoveToFront(options);
                    break;
            }
        }

        
        /// <summary>
        /// Moves Character in front of other characters on stage
        /// </summary>
        public virtual void MoveToFront(PortraitOptions options)
        {
            options.character.State.holder.SetSiblingIndex(options.character.State.holder.parent.childCount);
            options.character.State.display = DisplayType.MoveToFront;
            FinishCommand(options);
        }

        /// <summary>
        /// Show portrait with the supplied portrait options
        /// </summary>
        /// <param name="options"></param>
        public virtual void Show(PortraitOptions options)
        {
            options = CleanPortraitOptions(options);

            if (options.shiftIntoPlace)
            {
                options.fromPosition = Instantiate(options.toPosition) as RectTransform;
                if (options.offset == PositionOffset.OffsetLeft)
                {
                    options.fromPosition.anchoredPosition =
                        new Vector2(options.fromPosition.anchoredPosition.x - Mathf.Abs(options.shiftOffset.x),
                            options.fromPosition.anchoredPosition.y - Mathf.Abs(options.shiftOffset.y));
                }
                else if (options.offset == PositionOffset.OffsetRight)
                {
                    options.fromPosition.anchoredPosition =
                        new Vector2(options.fromPosition.anchoredPosition.x + Mathf.Abs(options.shiftOffset.x),
                            options.fromPosition.anchoredPosition.y + Mathf.Abs(options.shiftOffset.y));
                }
                else
                {
                    options.fromPosition.anchoredPosition = new Vector2(options.fromPosition.anchoredPosition.x, options.fromPosition.anchoredPosition.y);
                }
            }

            SetupPortrait(options);

            float duration = options.fadeDuration;

            var prevPortrait = options.character.State.portrait;

            if (options.character.State.portrait != null && options.character.State.portrait != options.portrait)
            {
                HidePortrait(options.character.State.portraitImage.rectTransform, duration);
            }

            options.character.State.SetPortraitImageBySprite(options.portrait);
            options.character.State.portraitImage.rectTransform.gameObject.SetActive(true);
            
            FungusTween.Stop(options.character.State.portraitImage.gameObject);
            TweenPortraitColor(options.character.State.portraitImage, Color.white, duration);
            TrackPortraitTween(FungusTween.Alpha(options.character.State.portraitImage.rectTransform, 1f, duration, stage.FadeEaseType));

            DoMoveTween(options);

            if (!stage.CharactersOnStage.Contains(options.character))
            {
                stage.CharactersOnStage.Add(options.character);
            }

            options.character.State.holder.SetSiblingIndex(options.character.State.holder.parent.childCount);

            // Update character state after showing
            options.character.State.onScreen = true;
            options.character.State.display = DisplayType.Show;
            options.character.State.facing = options.facing;
            options.character.State.position = options.toPosition;
            FinishCommand(options);
        }

        protected virtual void HidePortrait(RectTransform rectTransform, float duration)
        {
            FungusTween.Stop(rectTransform.gameObject);
            TrackPortraitTween(FungusTween.Alpha(rectTransform, 0f, duration, stage.FadeEaseType)
                .OnComplete(() => rectTransform.gameObject.SetActive(false)));
        }

        /// <summary>
        /// Hide portrait with provided options
        /// </summary>
        public virtual void Hide(PortraitOptions options)
        {
            CleanPortraitOptions(options);

            if (options.character.State.display == DisplayType.None)
            {
                return;
            }

            SetupPortrait(options);

            float duration = options.fadeDuration;

            HidePortrait(options.character.State.portraitImage.rectTransform, duration);

            DoMoveTween(options);

            //update character state after hiding
            options.character.State.onScreen = false;
            options.character.State.facing = options.facing;
            options.character.State.position = options.toPosition;
            options.character.State.display = DisplayType.Hide;

            if (stage.CharactersOnStage.Remove(options.character))
            {
            }

            FinishCommand(options);
        }

        /// <summary>
        /// Sets the dimmed state of a character on the stage.
        /// </summary>
        public virtual void SetDimmed(Character character, bool dimmedState)
        {
            if (character.State.dimmed == dimmedState)
            {
                return;
            }

            character.State.dimmed = dimmedState;

            Color targetColor = dimmedState ? stage.DimColor : Color.white;

            float duration = stage.FadeDuration;

            TweenPortraitColor(character.State.portraitImage, targetColor, duration);
        }

        private void TweenPortraitColor(Image Target, Color End, float Duration)
        {
            if (colorTweenDict.TryGetValue(Target, out var Previous)) Previous.Stop();
            colorTweenDict[Target] = TrackPortraitTween(FungusTween.Track(Target.gameObject,
                Tween.Custom(Target, Target.color, End, Duration, (Image, Value) =>
                {
                    Value.a = Image.color.a;
                    Image.color = Value;
                }, TweenEasing.Get(stage.FadeEaseType))));
        }

        private Tween TrackPortraitTween(Tween Animation)
        {
            for (int Index = portraitTweenList.Count - 1; Index >= 0; Index--)
                if (!portraitTweenList[Index].isAlive) portraitTweenList.RemoveAt(Index);
            portraitTweenList.Add(Animation);
            return Animation;
        }

        protected virtual void OnDisable()
        {
            CancelPendingOperations();
        }

        #region Overloads and Helpers

        /// <summary>
        /// Shows character at a named position in the stage
        /// </summary>
        /// <param name="character"></param>
        /// <param name="position">Named position on stage</param>
        public virtual void Show(Character character, string position)
        {
            PortraitOptions options = new PortraitOptions(true);
            options.character = character;
            options.fromPosition = options.toPosition = stage.GetPosition(position);

            Show(options);
        }

        /// <summary>
        /// Shows character moving from a position to a position
        /// </summary>
        /// <param name="character"></param>
        /// <param name="portrait"></param>
        /// <param name="fromPosition">Where the character will appear</param>
        /// <param name="toPosition">Where the character will move to</param>
        public virtual void Show(Character character, string portrait, string fromPosition, string toPosition)
        {
            PortraitOptions options = new PortraitOptions(true);
            options.character = character;
            options.portrait = character.GetPortrait(portrait);
            options.fromPosition = stage.GetPosition(fromPosition);
            options.toPosition = stage.GetPosition(toPosition);
            options.move = true;

            Show(options);
        }

        /// <summary>
        /// Simple show command that shows the character with an available named portrait
        /// </summary>
        /// <param name="character">Character to show</param>
        /// <param name="portrait">Named portrait to show for the character, i.e. "angry", "happy", etc</param>
        public virtual void ShowPortrait(Character character, string portrait)
        {
            PortraitOptions options = new PortraitOptions(true);
            options.character = character;
            options.portrait = character.GetPortrait(portrait);

            if (character.State.position == null)
            {
                options.toPosition = options.fromPosition = stage.GetPosition("middle");
            }
            else
            {
                options.fromPosition = options.toPosition = character.State.position;
            }

            Show(options);
        }

        /// <summary>
        /// Simple character hide command
        /// </summary>
        /// <param name="character">Character to hide</param>
        public virtual void Hide(Character character)
        {
            PortraitOptions options = new PortraitOptions(true);
            options.character = character;

            Hide(options);
        }

        /// <summary>
        /// Move the character to a position then hide it
        /// </summary>
        /// <param name="character">Character to hide</param>
        /// <param name="toPosition">Where the character will disapear to</param>
        public virtual void Hide(Character character, string toPosition)
        {
            PortraitOptions options = new PortraitOptions(true);
            options.character = character;
            options.toPosition = stage.GetPosition(toPosition);
            options.move = true;

            Hide(options);
        }

        
        /// <summary>
        /// Moves Character in front of other characters on stage
        /// </summary>
        public virtual void MoveToFront(Character character)
        {
            PortraitOptions options = new PortraitOptions(true);
            options.character = character;

            MoveToFront(CleanPortraitOptions(options));
        }


        protected virtual void DoMoveTween(Character character, RectTransform fromPosition, RectTransform toPosition, float moveDuration, Boolean waitUntilFinished)
        {
            PortraitOptions options = new PortraitOptions(true);
            options.character = character;
            options.fromPosition = fromPosition;
            options.toPosition = toPosition;
            options.move = true;
            options.moveDuration = moveDuration;
            options.waitUntilFinished = waitUntilFinished;

            DoMoveTween(options);
        }

        #endregion 
    }
}

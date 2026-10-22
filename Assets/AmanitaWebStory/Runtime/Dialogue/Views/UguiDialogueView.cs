using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace AmanitaWebStory
{
    public sealed class UguiDialogueView : DialogueView
    {
        [SerializeField]
        private GameObject surface;
        [SerializeField]
        private TMP_Text speakerText;
        [SerializeField]
        private TMP_Text lineText;
        [SerializeField]
        private UnityEngine.UI.Image portrait;
        [SerializeField]
        private UnityEngine.UI.Image background;
        [SerializeField]
        private UnityEngine.UI.Button continueButton;
        [SerializeField]
        private RectTransform optionRoot;
        [SerializeField]
        private GameObject optionPanel;
        [SerializeField]
        private DialogueOptionButton optionPrefab;
        [SerializeField]
        private AudioSource voiceSource;
        [SerializeField]
        private AudioSource soundSource;
        [SerializeField, Min(0), Tooltip("每秒显示的字符数 0 为立即显示")]
        private float charactersPerSecond = 36;

        private readonly List<DialogueOptionButton> optionList = new List<DialogueOptionButton>();
        private UniTaskCompletionSource continueInput;
        private UniTaskCompletionSource<int> choiceInput;
        private bool revealRequested;

        #region 界面生命周期

        private void Awake() { InitComponents(); }

        private void InitComponents()
        {
            continueButton.onClick.AddListener(Continue);
            surface.SetActive(false);
        }

        private void OnDestroy() { continueButton.onClick.RemoveListener(Continue); }

        public override void Open(StoryAsset Story)
        {
            background.sprite = Story.Background;
            background.enabled = Story.Background != null;
            speakerText.text = "";
            lineText.text = "";
            portrait.enabled = false;
            continueButton.gameObject.SetActive(false);
            optionPanel.SetActive(false);
            surface.SetActive(true);
        }

        public override void Close()
        {
            continueInput?.TrySetCanceled();
            choiceInput?.TrySetCanceled();
            voiceSource.Stop();
            soundSource.Stop();
            ClearOptions();
            optionPanel.SetActive(false);
            speakerText.text = "";
            lineText.text = "";
            portrait.sprite = null;
            background.sprite = null;
            surface.SetActive(false);
        }

        #endregion

        #region 展示与输入

        public override async UniTask ShowLineAsync(StoryLine Line, CancellationToken Token)
        {
            speakerText.text = Line.Speaker;
            lineText.text = Line.Text;
            lineText.maxVisibleCharacters = 0;
            lineText.ForceMeshUpdate();
            portrait.sprite = Line.Portrait;
            portrait.enabled = Line.Portrait != null;
            voiceSource.clip = Line.Voice;
            if (Line.Voice != null)
                voiceSource.Play();
            revealRequested = false;
            continueInput = new UniTaskCompletionSource();
            continueButton.gameObject.SetActive(true);
            try
            {
                int Count = lineText.textInfo.characterCount;
                float Visible = 0;
                while (charactersPerSecond > 0 && Visible < Count && !revealRequested)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, Token);
                    Visible += charactersPerSecond * Time.unscaledDeltaTime;
                    lineText.maxVisibleCharacters = (int)Visible;
                }
                lineText.maxVisibleCharacters = int.MaxValue;
                await continueInput.Task.AttachExternalCancellation(Token);
            }
            finally
            {
                await UniTask.SwitchToMainThread();
                continueInput = null;
                voiceSource.Stop();
                continueButton.gameObject.SetActive(false);
            }
        }

        public override async UniTask<int> ChooseAsync(IReadOnlyList<StoryOptionData> Options, CancellationToken Token)
        {
            choiceInput = new UniTaskCompletionSource<int>();
            optionRoot.anchoredPosition = Vector2.zero;
            optionPanel.SetActive(true);
            try
            {
                for (int Index = 0; Index < Options.Count; Index++)
                {
                    var Button = Instantiate(optionPrefab, optionRoot);
                    Button.Bind(Options[Index].Text, Index, SelectOption);
                    Button.gameObject.SetActive(true);
                    optionList.Add(Button);
                }
                return await choiceInput.Task.AttachExternalCancellation(Token);
            }
            finally
            {
                await UniTask.SwitchToMainThread();
                choiceInput = null;
                ClearOptions();
                optionPanel.SetActive(false);
            }
        }

        public override async UniTask PlaySoundAsync(StorySound Sound, CancellationToken Token)
        {
            soundSource.PlayOneShot(Sound.Clip, Sound.Volume);
            if (Sound.WaitUntilFinished)
                await UniTask.Delay(TimeSpan.FromSeconds(Sound.Clip.length), ignoreTimeScale: true, cancellationToken: Token);
        }

        private void Continue()
        {
            if (lineText.maxVisibleCharacters < lineText.textInfo.characterCount)
                revealRequested = true;
            else
                continueInput?.TrySetResult();
        }

        private void SelectOption(int Index) { choiceInput?.TrySetResult(Index); }

        private void ClearOptions()
        {
            foreach (var Button in optionList)
            {
                Button.gameObject.SetActive(false);
                Destroy(Button.gameObject);
            }
            optionList.Clear();
        }

        #endregion
    }
}

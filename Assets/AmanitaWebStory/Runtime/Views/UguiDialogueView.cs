using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Fungus;
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
        [SerializeField, Tooltip("挂在 lineText 同一对象上 负责打字效果与 Fungus 文本标签")]
        private Writer writer;
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
        [SerializeField, Tooltip("站位数量即同屏角色上限 满员时替换最久未说话的角色")]
        private List<PortraitSlot> portraitSlotList = new List<PortraitSlot>();

        private readonly List<DialogueOptionButton> optionList = new List<DialogueOptionButton>();
        private UniTaskCompletionSource continueInput;
        private UniTaskCompletionSource<int> choiceInput;
        private int lineOrder;

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
            lineOrder = 0;
            continueButton.gameObject.SetActive(false);
            optionPanel.SetActive(false);
            ClearPortraits();
            surface.SetActive(true);
        }

        public override void Close()
        {
            continueInput?.TrySetCanceled();
            choiceInput?.TrySetCanceled();
            writer.Stop();
            voiceSource.Stop();
            soundSource.Stop();
            ClearOptions();
            optionPanel.SetActive(false);
            speakerText.text = "";
            lineText.text = "";
            background.sprite = null;
            ClearPortraits();
            surface.SetActive(false);
        }

        #endregion

        #region 展示与输入

        public override async UniTask ShowLineAsync(StoryLine Line, CancellationToken Token)
        {
            speakerText.text = Line.Actor != null ? Line.Actor.DisplayName : "";
            speakerText.color = Line.Actor != null ? Line.Actor.NameColor : Color.white;
            RefreshPortraits(Line.Actor, Line.Portrait);
            voiceSource.clip = Line.Voice;
            if (Line.Voice != null)
                voiceSource.Play();
            continueInput = new UniTaskCompletionSource();
            continueButton.gameObject.SetActive(true);
            try
            {
                await writer.WriteAsync(Line.Text, true, false, false, false, null, null, Token);
                await continueInput.Task.AttachExternalCancellation(Token);
            }
            finally
            {
                await UniTask.SwitchToMainThread();
                continueInput = null;
                writer.Stop();
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

        // Writer 开启立即补全时 打字中的输入只补全文字 不推进剧情
        private void Continue()
        {
            if (writer.IsWriting)
                writer.OnNextLineEvent();
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

        #region 多人立绘

        /// <summary>
        /// 说话角色有立绘时占用站位并高亮 其余站位压暗 旁白或无立绘时全部压暗
        /// </summary>
        private void RefreshPortraits(StoryActor Actor, Sprite Portrait)
        {
            PortraitSlot Speaking = null;
            if (Actor != null && Portrait != null)
            {
                Speaking = SelectSlot(Actor);
                Speaking.Show(Actor, Portrait, ++lineOrder);
            }
            foreach (var Slot in portraitSlotList)
                if (Slot != Speaking)
                    Slot.Dim();
        }

        private PortraitSlot SelectSlot(StoryActor Actor)
        {
            if (portraitSlotList.Count == 0)
                throw new InvalidOperationException($"界面 {name} 未配置立绘站位 无法显示 {Actor.DisplayName} 的立绘");
            PortraitSlot Selected = null;
            foreach (var Slot in portraitSlotList)
            {
                if (Slot.Actor == Actor)
                    return Slot;
                if (Selected == null || Rank(Slot) < Rank(Selected))
                    Selected = Slot;
            }
            return Selected;
        }

        // 空站位优先 其次替换最久未说话的角色
        private static int Rank(PortraitSlot Slot) => Slot.Actor == null ? int.MinValue : Slot.LastSpoken;

        private void ClearPortraits()
        {
            foreach (var Slot in portraitSlotList)
                Slot.Clear();
        }

        #endregion
    }
}

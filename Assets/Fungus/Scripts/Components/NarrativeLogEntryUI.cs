using Fungus.Inspector;
using Cysharp.Threading.Tasks;
using System.Threading;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Manages the UI aspects of the Narrative Log via EntryDisplay objects
    ///
    /// Originally contributed by https://github.com/CG-Tespy
    /// </summary>
    public class NarrativeLogEntryUI : MonoBehaviour
    {
        private readonly AsyncOperationScope asyncScope = new AsyncOperationScope();

        [Tooltip("设置本步骤使用的历史条目预制体")]
        [LabelText("历史条目预制体")]
        [SerializeField]
        protected NarrativeLogEntryDisplay entryDisplayPrefab;

        [LabelText("历史条目容器")]
        [SerializeField]
        protected RectTransform entryHolder;
        protected CanvasGroup canvasGroup;
        protected List<NarrativeLogEntryDisplay> entryDisplays = new List<NarrativeLogEntryDisplay>();
        protected UnityEngine.UI.ScrollRect scrollRect;

        protected virtual void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            var tmp = FungusManager.Instance.NarrativeLog;
            // Make sure to update the UI when new entries are added to or
            // cleared from the log.
            //using the fungusmanager to ensure that the narrativeLog is inited
            NarrativeLog.OnNarrativeAdded += OnNarrativeAdded;
            NarrativeLog.OnNarrativeLogClear += Clear;

            scrollRect = GetComponentInChildren<UnityEngine.UI.ScrollRect>();
        }

        public virtual void Clear()
        {
            for (int i = 0; i < entryDisplays.Count; i++)
            {
                var entryDisplay = entryDisplays[i];
                Destroy(entryDisplay.gameObject);
            }

            entryDisplays.Clear();
        }

        public virtual void Open()
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        public virtual void Close()
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        protected virtual void OnNarrativeAdded(NarrativeLogEntry entryAdded)
        {
            // Create a display for the new entry, and have it show in the UI.
            var newEntryDisplay = Instantiate(entryDisplayPrefab);
            newEntryDisplay.transform.SetParent(entryHolder, false);
            newEntryDisplay.ToDisplay = entryAdded;
            entryDisplays.Add(newEntryDisplay);
            asyncScope.Run(this, Token => ForceToBottomAsync(Token)).Forget();
        }

        private async UniTask ForceToBottomAsync(CancellationToken CancellationToken = default)
        {
            await UniTask.NextFrame(cancellationToken: CancellationToken);
            scrollRect.verticalNormalizedPosition = 0;
        }

        protected virtual void OnLogCleared()
        {
            Clear();
        }

        protected virtual void OnDestroy()
        {
            var fManInst = FungusManager.Instance;
            // Avoid this responding to signals when being destroyed.
            if (fManInst != null)
            {
                NarrativeLog.OnNarrativeAdded -= OnNarrativeAdded;
                NarrativeLog.OnNarrativeLogClear -= Clear;
            }
        }
        protected virtual void OnDisable()
        {
            asyncScope.CancelAll();
        }

    }
}
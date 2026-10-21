using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

#if UNITY_5_3_OR_NEWER

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PrimeTween;

namespace Fungus
{
    /// <summary>
    /// A singleton game object which displays a simple UI for the Narrative Log.
    /// </summary>
    public class NarrativeLogMenu : MonoBehaviour 
    {
        [Tooltip("设置本步骤使用的历史条目预制体")]
        [LabelText("历史条目预制体")]
        [SerializeField]
        protected NarrativeLogEntryDisplay entryDisplayPrefab;

        [Tooltip("设置本步骤使用的显示日志")]
        [LabelText("显示日志")]
        [SerializeField]
        protected bool showLog = true;

        [Tooltip("设置本步骤使用的之前的对白行")]
        [LabelText("之前的对白行")]
        [SerializeField]
        protected bool previousLines = true;

        [Tooltip("设置本步骤使用的对白历史界面")]
        [LabelText("对白历史界面")]
        [SerializeField]
        protected ScrollRect narrativeLogView;

        [Tooltip("设置本步骤使用的最大字符数")]
        [LabelText("最大字符数")]
        [SerializeField]
        protected int maxCharacters = 10000;

        protected TextAdapter narLogViewtextAdapter = new TextAdapter();
        
        [Tooltip("设置本步骤使用的对白历史菜单分组")]
        [LabelText("对白历史菜单分组")]
        [SerializeField]
        protected CanvasGroup narrativeLogMenuGroup;

        protected static bool narrativeLogActive = false;
        
        protected AudioSource clickAudioSource;

        protected Tween fadeTween;

        protected static NarrativeLogMenu instance;

        protected virtual void Awake()
        {
            if (showLog)
            {
                // Only one instance of NarrativeLogMenu may exist
                if (instance != null)
                {
                    Destroy(gameObject);
                    return;
                }

                instance = this;

                GameObject.DontDestroyOnLoad(this);

                clickAudioSource = GetComponent<AudioSource>();
            }
            else
            {
                GameObject logView = GameObject.Find("NarrativeLogView");
                logView.SetActive(false);
                this.enabled = false;
            }

            narLogViewtextAdapter.InitFromGameObject(narrativeLogView.gameObject, true);
        }

        protected virtual void Start()
        {
            if (!narrativeLogActive)
            {
                narrativeLogMenuGroup.alpha = 0f;
            }

            //Clear up the lorem ipsum
            UpdateNarrativeLogText();
        }

        protected virtual void OnEnable()
        {
            WriterSignals.OnWriterState += OnWriterState;
            SaveManagerSignals.OnSavePointLoaded += OnSavePointLoaded;
            SaveManagerSignals.OnSaveReset += OnSaveReset;
            BlockSignals.OnBlockEnd += OnBlockEnd;
            NarrativeLog.OnNarrativeAdded += OnNarrativeAdded;
        }
                
        protected virtual void OnDisable()
        {
            fadeTween.Stop();
            fadeTween = default;
            WriterSignals.OnWriterState -= OnWriterState;
            SaveManagerSignals.OnSavePointLoaded -= OnSavePointLoaded;
            SaveManagerSignals.OnSaveReset -= OnSaveReset;
            BlockSignals.OnBlockEnd -= OnBlockEnd;
            NarrativeLog.OnNarrativeAdded -= OnNarrativeAdded;
        }

        protected virtual void OnNarrativeAdded(NarrativeLogEntry data)
        {
            UpdateNarrativeLogText();
        }

        protected virtual void OnWriterState(Writer writer, WriterState writerState)
        {
            if (writerState == WriterState.Start)
            {
                UpdateNarrativeLogText();
            }
        }

        protected virtual void OnSavePointLoaded(string savePointKey)
        {
            UpdateNarrativeLogText();
        }

        protected virtual void OnSaveReset()
        {
            FungusManager.Instance.NarrativeLog.Clear();
            UpdateNarrativeLogText();
        }

        protected virtual void OnBlockEnd (Block block)
        {
            // At block end update to get the last line of the block
            bool defaultPreviousLines = previousLines;
            previousLines = false;
            UpdateNarrativeLogText();
            previousLines = defaultPreviousLines;
        }

        protected void UpdateNarrativeLogText()
        {
            if (narrativeLogView.enabled)
            {
                var prettyHistory = FungusManager.Instance.NarrativeLog.GetPrettyHistory();

                if (prettyHistory.Length > maxCharacters)
                {
                    prettyHistory = "... " + prettyHistory.Substring(prettyHistory.Length - maxCharacters, maxCharacters);
                }
                narLogViewtextAdapter.Text = prettyHistory;

                Canvas.ForceUpdateCanvases();
                narrativeLogView.verticalNormalizedPosition = 0f;
                Canvas.ForceUpdateCanvases();
            }
        }

        protected void PlayClickSound()
        {
            if (clickAudioSource != null)
            {
                clickAudioSource.Play();
            }
        }

        #region Public methods

        public virtual void ToggleNarrativeLogView()
        {
            fadeTween.Stop();
            fadeTween = default;

            if (narrativeLogActive)
            {
                // Switch menu off
                fadeTween = FungusTween.Track(narrativeLogMenuGroup.gameObject, Tween.Alpha(narrativeLogMenuGroup, 0f, 0.2f, Ease.OutQuint)).OnComplete(() => {
                    narrativeLogMenuGroup.alpha = 0f;
                });
                
            }
            else
            {
                // Switch menu on
                fadeTween = FungusTween.Track(narrativeLogMenuGroup.gameObject, Tween.Alpha(narrativeLogMenuGroup, 1f, 0.2f, Ease.OutQuint)).OnComplete(() => {
                    narrativeLogMenuGroup.alpha = 1f;
                });
                
            }

            narrativeLogActive = !narrativeLogActive;
        }
    
        #endregion
    }
}

#endif
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
    /// A singleton game object which displays a simple UI for the save system.
    /// </summary>
    public class SaveMenu : MonoBehaviour 
    {
        [Tooltip("设置本步骤使用的存档数据键")]
        [LabelText("存档数据键")]
        [SerializeField]
        protected string saveDataKey = FungusConstants.DefaultSaveDataKey;

        [Tooltip("设置本步骤使用的启动时读取")]
        [LabelText("启动时读取")]
        [SerializeField]
        protected bool loadOnStart = true;

        [Tooltip("设置本步骤使用的自动保存")]
        [LabelText("自动保存")]
        [SerializeField]
        protected bool autoSave = false;

        [Tooltip("设置本步骤使用的重新开始时删除存档")]
        [LabelText("重新开始时删除存档")]
        [SerializeField]
        protected bool restartDeletesSave = false;

        [Tooltip("设置本步骤使用的存档菜单分组")]
        [LabelText("存档菜单分组")]
        [SerializeField]
        protected CanvasGroup saveMenuGroup;

        [Tooltip("设置本步骤使用的存档菜单按钮")]
        [LabelText("存档菜单按钮")]
        [SerializeField]
        protected Button saveMenuButton;

        [Tooltip("设置本步骤使用的保存按钮")]
        [LabelText("保存按钮")]
        [SerializeField]
        protected Button saveButton;

        [Tooltip("设置本步骤使用的读取按钮")]
        [LabelText("读取按钮")]
        [SerializeField]
        protected Button loadButton;

        [Tooltip("设置本步骤使用的回退按钮")]
        [LabelText("回退按钮")]
        [SerializeField]
        protected Button rewindButton;

        [Tooltip("设置本步骤使用的前进按钮")]
        [LabelText("前进按钮")]
        [SerializeField]
        protected Button forwardButton;
        
        [Tooltip("设置本步骤使用的重新开始按钮")]
        [LabelText("重新开始按钮")]
        [SerializeField]
        protected Button restartButton;

        [Tooltip("设置本步骤使用的调试显示")]
        [LabelText("调试显示")]
        [SerializeField]
        protected ScrollRect debugView;

        protected static bool saveMenuActive = false;

        protected AudioSource clickAudioSource;

        protected Tween fadeTween;

        protected static SaveMenu instance;

        protected static bool hasLoadedOnStart = false;

        protected virtual void Awake()
        {
            // Only one instance of SaveMenu may exist
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;

            if (transform.parent == null)
            {
                GameObject.DontDestroyOnLoad(this);
            }
            else
            {
                Debug.LogError("Save Menu cannot be preserved across scene loads if it is a child of another GameObject.");
            }

            clickAudioSource = GetComponent<AudioSource>();
        }

        protected virtual void Start()
        {
            if (!saveMenuActive)
            {
                saveMenuGroup.alpha = 0f;
            }

            var saveManager = FungusManager.Instance.SaveManager;

            // Make a note of the current scene. This will be used when restarting the game.
            if (string.IsNullOrEmpty(saveManager.StartScene))
            {
                saveManager.StartScene = SceneManager.GetActiveScene().name;
            }

            if (loadOnStart && !hasLoadedOnStart)
            {
                hasLoadedOnStart = true;

                if (saveManager.SaveDataExists(saveDataKey))
                {
                    saveManager.Load(saveDataKey);
                }
            }
        }

        protected virtual void Update()
        {
            var saveManager = FungusManager.Instance.SaveManager;

            // Hide the Save and Load buttons if autosave is on

            bool showSaveAndLoad = !autoSave;
            if (saveButton.IsActive() != showSaveAndLoad)
            {
                saveButton.gameObject.SetActive(showSaveAndLoad);
                loadButton.gameObject.SetActive(showSaveAndLoad);
            }
 
            if (showSaveAndLoad)
            {
                if (saveButton != null)
                {
                    // Don't allow saving unless there's at least one save point in the history,
                    // This avoids the case where you could try to load a save data with 0 save points.
                    saveButton.interactable = saveManager.NumSavePoints > 0 && saveMenuActive;
                }
                if (loadButton != null)
                {
                    loadButton.interactable = saveManager.SaveDataExists(saveDataKey) && saveMenuActive;
                }
            }

            if (restartButton != null)
            {
                restartButton.interactable = saveMenuActive;
            }
            if (rewindButton != null)
            {
                rewindButton.interactable = saveManager.NumSavePoints > 0 && saveMenuActive;
            }
            if (forwardButton != null)
            {
                forwardButton.interactable = saveManager.NumRewoundSavePoints > 0 && saveMenuActive;
            }

            if (debugView.enabled)
            {
                var debugText = debugView.GetComponentInChildren<Text>();
                if (debugText != null)
                {
                    debugText.text = saveManager.GetDebugInfo();
                }
            }

        }

        protected virtual void OnEnable()
        {
            SaveManagerSignals.OnSavePointAdded += OnSavePointAdded;
        }

        protected virtual void OnDisable()
        {
            fadeTween.Stop();
            fadeTween = default;
            SaveManagerSignals.OnSavePointAdded -= OnSavePointAdded;
        }

        protected virtual void OnSavePointAdded(string savePointKey, string savePointDescription)
        {
            var saveManager = FungusManager.Instance.SaveManager;

            if (autoSave &&
                saveManager.NumSavePoints > 0)
            {
                saveManager.Save(saveDataKey);
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

        /// <summary>
        /// Gets the string key used to store save game data in Player Prefs. 
        /// </summary>
        public virtual string SaveDataKey { get { return saveDataKey; } }

        /// <summary>
        /// Toggles the expanded / collapsed state of the save menu.
        /// Uses a tween to fade the menu UI in and out.
        /// </summary>
        public virtual void ToggleSaveMenu()
        {
            fadeTween.Stop();
            fadeTween = default;

            if (saveMenuActive)
            {
                // Switch menu off
                fadeTween = FungusTween.Track(saveMenuGroup.gameObject, Tween.Alpha(saveMenuGroup, 0f, 0.2f, Ease.OutQuint)).OnComplete( () => {
                    saveMenuGroup.alpha = 0f;
                });
            }
            else
            {
                // Switch menu on
                fadeTween = FungusTween.Track(saveMenuGroup.gameObject, Tween.Alpha(saveMenuGroup, 1f, 0.2f, Ease.OutQuint)).OnComplete( () => {
                    saveMenuGroup.alpha = 1f;
                });
            }

            saveMenuActive = !saveMenuActive;
        }

        /// <summary>
        /// Handler function called when the Save button is pressed.
        /// </summary>
        public virtual void Save()
        {
            var saveManager = FungusManager.Instance.SaveManager;

            if (saveManager.NumSavePoints > 0)
            {
                PlayClickSound();
                saveManager.Save(saveDataKey);
            }
        }

        /// <summary>
        /// Handler function called when the Load button is pressed.
        /// </summary>
        public virtual void Load()
        {
            var saveManager = FungusManager.Instance.SaveManager;

            if (saveManager.SaveDataExists(saveDataKey))
            {
                PlayClickSound();
                saveManager.Load(saveDataKey);
            }

        }

        /// <summary>
        /// Handler function called when the Rewind button is pressed.
        /// </summary>
        public virtual void Rewind()
        {
            PlayClickSound();

            var saveManager = FungusManager.Instance.SaveManager;
            if (saveManager.NumSavePoints > 0)
            {
                saveManager.Rewind();
            }

        }

        /// <summary>
        /// Handler function called when the Fast Forward button is pressed.
        /// </summary>
        public virtual void FastForward()
        {
            PlayClickSound();

            var saveManager = FungusManager.Instance.SaveManager;
            if (saveManager.NumRewoundSavePoints > 0)
            {
                saveManager.FastForward();
            }
        }

        /// <summary>
        /// Handler function called when the Restart button is pressed.
        /// </summary>
        public virtual void Restart()
        {
            var saveManager = FungusManager.Instance.SaveManager;
            if (string.IsNullOrEmpty(saveManager.StartScene))
            {
                Debug.LogError("No start scene specified");
                return;
            }

            PlayClickSound();

            // Reset the Save History for a new game
            saveManager.ClearHistory();

            if (restartDeletesSave)
            {
                SaveManager.Delete(saveDataKey);
            }
            SaveManagerSignals.DoSaveReset();
            SceneManager.LoadScene(saveManager.StartScene);
        }

        #endregion
    }
}

#endif
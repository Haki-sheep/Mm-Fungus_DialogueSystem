using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

#if UNITY_5_3_OR_NEWER

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

namespace Fungus
{
    /// <summary>
    /// Serializable container for a Save Point's data. 
    /// All data is stored as strings, and the only concrete game class it depends on is the SaveData component.
    /// </summary>
    [System.Serializable]
    public class SavePointData
    {
        [LabelText("存档点标识")]
        [SerializeField]
        protected string savePointKey;

        [LabelText("存档点说明")]
        [SerializeField]
        protected string savePointDescription;

        [LabelText("场景名称")]
        [SerializeField]
        protected string sceneName;

        [LabelText("存档数据项")]
        [SerializeField]
        protected List<SaveDataItem> saveDataItems = new List<SaveDataItem>();

        protected static SavePointData Create(string _savePointKey, string _savePointDescription, string _sceneName)
        {
            var savePointData = new SavePointData();

            savePointData.savePointKey = _savePointKey;
            savePointData.savePointDescription = _savePointDescription;
            savePointData.sceneName = _sceneName;

            return savePointData;
        }

        #region Public methods

        /// <summary>
        /// Gets or sets the unique key for the Save Point.
        /// </summary>
        public string SavePointKey { get { return savePointKey; } set { savePointKey = value; } }

        /// <summary>
        /// Gets or sets the description for the Save Point.
        /// </summary>
        public string SavePointDescription { get { return savePointDescription; } set { savePointDescription = value; } }

        /// <summary>
        /// Gets or sets the scene name associated with the Save Point.
        /// </summary>
        public string SceneName { get { return sceneName; } set { sceneName = value; } }

        /// <summary>
        /// Gets the list of save data items.
        /// </summary>
        /// <value>The save data items.</value>
        public List<SaveDataItem> SaveDataItems { get { return saveDataItems; } }

        /// <summary>
        /// Encodes a new Save Point to data and converts it to JSON text format.
        /// </summary>
        public static string Encode(string _savePointKey, string _savePointDescription, string _sceneName)
        {
            var savePointData = Create(_savePointKey, _savePointDescription, _sceneName);

            // Look for a SaveData component in the scene to populate the save data items.
        #if UNITY_6000
            var saveData = GameObject.FindFirstObjectByType<SaveData>();
        #else
            var saveData = GameObject.FindObjectOfType<SaveData>();
        #endif
            if (saveData != null)
            {
                saveData.Encode(savePointData.SaveDataItems);
            }

            return JsonUtility.ToJson(savePointData, true);
        }

        /// <summary>
        /// Decodes a Save Point from JSON text format and loads it.
        /// </summary>
        public static void Decode(string saveDataJSON)
        {
            var savePointData = JsonUtility.FromJson<SavePointData>(saveDataJSON);

            UnityAction<Scene, LoadSceneMode> onSceneLoadedAction = null;

            onSceneLoadedAction = (scene, mode) =>  {
                // Additive scene loads and non-matching scene loads could happen if the client is using the
                // SceneManager directly. We just ignore these events and hope they know what they're doing!
                if (mode == LoadSceneMode.Additive ||
                    scene.name != savePointData.SceneName)
                {
                    return;
                }

                SceneManager.sceneLoaded -= onSceneLoadedAction;

                // Look for a SaveData component in the scene to process the save data items.
            #if UNITY_6000
                var saveData = GameObject.FindFirstObjectByType<SaveData>();
            #else
                var saveData = GameObject.FindObjectOfType<SaveData>();
            #endif
                if (saveData != null)
                {
                    saveData.Decode(savePointData.SaveDataItems);
                }

                SaveManagerSignals.DoSavePointLoaded(savePointData.savePointKey);
            };
                
            SceneManager.sceneLoaded += onSceneLoadedAction;
            SceneManager.LoadScene(savePointData.SceneName);
        }     

        #endregion
    }
}

#endif
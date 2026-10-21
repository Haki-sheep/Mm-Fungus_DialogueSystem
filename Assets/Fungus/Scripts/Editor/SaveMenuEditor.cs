// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

#if UNITY_5_3_OR_NEWER

using UnityEngine;
using UnityEditor;

namespace Fungus.EditorUtils
{
    [CustomEditor (typeof(SaveMenu), true)]
    public class SaveMenuEditor : Editor 
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button(new GUIContent("删除存档数据", "删除 PlayerPrefs 中与这个存档数据键关联的记录")))
            {
                var saveMenu = target as SaveMenu;

                if (saveMenu != null)
                {
                    SaveManager.Delete(saveMenu.SaveDataKey);
                    FlowchartWindow.ShowNotification("存档数据已删除");
                }
            }

            base.OnInspectorGUI();
        }
    }
}

#endif
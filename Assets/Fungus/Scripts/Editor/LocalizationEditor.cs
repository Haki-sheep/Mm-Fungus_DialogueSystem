// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEngine;
using System.IO;

namespace Fungus.EditorUtils
{
    [CustomEditor(typeof(Localization))]
    public class LocalizationEditor : Editor 
    {
        protected SerializedProperty activeLanguageProp;
        protected SerializedProperty localizationFileProp;

        protected virtual void OnEnable()
        {
            activeLanguageProp = serializedObject.FindProperty("activeLanguage");
            localizationFileProp = serializedObject.FindProperty("localizationFile");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Localization localization = target as Localization;

            FungusEditorGUI.PropertyField(activeLanguageProp);
            FungusEditorGUI.PropertyField(localizationFileProp);

            GUILayout.Space(10);

            EditorGUILayout.HelpBox("导出 CSV 翻译表 保存到项目后 将文件拖入上方翻译数据文件字段", MessageType.Info);

            if (GUILayout.Button(new GUIContent("导出翻译表")))
            {
                ExportLocalizationFile(localization);
            }

            GUILayout.Space(10);

            EditorGUILayout.HelpBox("导出场景中的原始对白 编辑文本后可使用导入功能写回", MessageType.Info);

            if (GUILayout.Button(new GUIContent("导出原始对白")))
            {
                ExportStandardText(localization);
            }

            if (GUILayout.Button(new GUIContent("导入原始对白")))
            {
                ImportStandardText(localization);
            }

            serializedObject.ApplyModifiedProperties();
        }

        public virtual void ExportLocalizationFile(Localization localization)
        {
            string path = EditorUtility.SaveFilePanelInProject("Export Localization File",
                                                               "localization.csv",
                                                               "csv",
                                                               "Please enter a filename to save the localization file to");
            if (path.Length == 0) 
            {
                return;
            }

            string csvData = localization.GetCSVData();         
            File.WriteAllText(path, csvData);
            AssetDatabase.ImportAsset(path);

            TextAsset textAsset = AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset)) as TextAsset;
            if (textAsset != null)
            {
                localization.LocalizationFile = textAsset;
            }

            ShowNotification(localization);
        }

        public virtual void ExportStandardText(Localization localization)
        {
            string path = EditorUtility.SaveFilePanel("Export Standard Text", "Assets/", "standard.txt", "");
            if (path.Length == 0) 
            {
                return;
            }

            localization.ClearLocalizeableCache();

            string textData = localization.GetStandardText();           
            File.WriteAllText(path, textData);
            AssetDatabase.Refresh();

            ShowNotification(localization);
        }
        
        public virtual void ImportStandardText(Localization localization)
        {
            string path = EditorUtility.OpenFilePanel("Import Standard Text", "Assets/", "txt");
            if (path.Length == 0) 
            {
                return;
            }

            localization.ClearLocalizeableCache();

            string textData = File.ReadAllText(path);
            localization.SetStandardText(textData);

            ShowNotification(localization);
        }

        protected virtual void ShowNotification(Localization localization)
        {
            FlowchartWindow.ShowNotification(localization.NotificationText);
            localization.NotificationText = "";
        }
    }
}

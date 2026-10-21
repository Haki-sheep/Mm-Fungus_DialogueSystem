// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEngine;

namespace Fungus.EditorUtils
{
    [CustomEditor (typeof(ControlStage))]
    public class StageEditor : CommandEditor
    {
        protected SerializedProperty displayProp;
        protected SerializedProperty stageProp;
        protected SerializedProperty replacedStageProp;
        protected SerializedProperty useDefaultSettingsProp;
        protected SerializedProperty fadeDurationProp;
        protected SerializedProperty waitUntilFinishedProp;

        public override void OnEnable()
        {
            base.OnEnable();

            displayProp = serializedObject.FindProperty("display");
            stageProp = serializedObject.FindProperty("stage");
            replacedStageProp = serializedObject.FindProperty("replacedStage");
            useDefaultSettingsProp = serializedObject.FindProperty("useDefaultSettings");
            fadeDurationProp = serializedObject.FindProperty("fadeDuration");
            waitUntilFinishedProp = serializedObject.FindProperty("waitUntilFinished");
        }
        
        public override void DrawCommandGUI() 
        {
            serializedObject.Update();
            
            ControlStage t = target as ControlStage;

            // Format Enum names
            string[] displayLabels = StringFormatter.FormatEnumNames(t.Display,"<None>");
            displayProp.enumValueIndex = EditorGUILayout.Popup("立绘操作", (int)displayProp.enumValueIndex, displayLabels);

            string replaceLabel = "Portrait Stage";
            if (t.Display == StageDisplayType.Swap)
            {
                CommandEditor.ObjectField<Stage>(replacedStageProp, 
                                                 new GUIContent("替换", "与其交换的角色"), 
                                                 new GUIContent("默认"),
                                                 Stage.ActiveStages);
                replaceLabel = "With";
            }

            if (Stage.ActiveStages.Count > 0)
            {
                CommandEditor.ObjectField<Stage>(stageProp, 
                                                 new GUIContent(replaceLabel, "选择用于显示角色立绘的舞台"), 
                                                 new GUIContent("默认"),
                                                 Stage.ActiveStages);
            }

            bool showOptionalFields = true;
            Stage s = t._Stage;
            // Only show optional portrait fields once required fields have been filled...
            if (t._Stage != null)                // Character is selected
            {
                if (t._Stage == null)        // If no default specified, try to get any portrait stage in the scene
                {
                #if UNITY_6000
                    s = GameObject.FindFirstObjectByType<Stage>();
                #else
                    s = GameObject.FindObjectOfType<Stage>();
                #endif
                }
                if (s == null)
                {
                    EditorGUILayout.HelpBox("请先指定立绘舞台", MessageType.Error);
                    showOptionalFields = false; 
                }
            }
            if (t.Display != StageDisplayType.None && showOptionalFields) 
            {
                FungusEditorGUI.PropertyField(useDefaultSettingsProp);
                if (!t.UseDefaultSettings)
                {
                    FungusEditorGUI.PropertyField(fadeDurationProp);
                }
                FungusEditorGUI.PropertyField(waitUntilFinishedProp);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
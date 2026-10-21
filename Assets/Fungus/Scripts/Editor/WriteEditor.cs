// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEngine;

namespace Fungus.EditorUtils
{
    [CustomEditor (typeof(Write))]
    public class WriteEditor : CommandEditor
    {
        public static bool showTagHelp;

        protected SerializedProperty textObjectProp;
        protected SerializedProperty textProp;
        protected SerializedProperty descriptionProp;
        protected SerializedProperty clearTextProp;
        protected SerializedProperty textColorProp;
        protected SerializedProperty setAlphaProp;
        protected SerializedProperty setColorProp;
        protected SerializedProperty waitUntilFinishedProp;

        public static void DrawTagHelpLabel()
        {
            string tagsText = "";
            tagsText += "\n";
            tagsText += TextTagParser.GetTagHelp();

            float pixelHeight = EditorStyles.miniLabel.CalcHeight(new GUIContent(tagsText), EditorGUIUtility.currentViewWidth);
            EditorGUILayout.SelectableLabel(tagsText, GUI.skin.GetStyle("HelpBox"), GUILayout.MinHeight(pixelHeight));
        }

        public override void OnEnable()
        {
            base.OnEnable();

            textObjectProp = serializedObject.FindProperty("textObject");
            textProp = serializedObject.FindProperty("text");
            descriptionProp = serializedObject.FindProperty("description");
            clearTextProp = serializedObject.FindProperty("clearText");
            textColorProp = serializedObject.FindProperty("textColor");
            setAlphaProp = serializedObject.FindProperty("setAlpha");
            setColorProp = serializedObject.FindProperty("setColor");
            waitUntilFinishedProp = serializedObject.FindProperty("waitUntilFinished");
        }
        
        public override void DrawCommandGUI() 
        {
            serializedObject.Update();

            FungusEditorGUI.PropertyField(textObjectProp);
            FungusEditorGUI.PropertyField(textProp);
            FungusEditorGUI.PropertyField(descriptionProp);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("文字标签说明", "查看支持的文字标签"), new GUIStyle(EditorStyles.miniButton)))
            {
                showTagHelp = !showTagHelp;
            }
            EditorGUILayout.EndHorizontal();

            if (showTagHelp)
            {
                DrawTagHelpLabel();
            }

            FungusEditorGUI.PropertyField(clearTextProp);

            FungusEditorGUI.PropertyField(textColorProp);
            switch ((TextColor)textColorProp.enumValueIndex)
            {
            case TextColor.Default:
                break;
            case TextColor.SetVisible:
                break;
            case TextColor.SetAlpha:
                FungusEditorGUI.PropertyField(setAlphaProp);
                break;
            case TextColor.SetColor:
                FungusEditorGUI.PropertyField(setColorProp);
                break;
            }

            FungusEditorGUI.PropertyField(waitUntilFinishedProp);

            serializedObject.ApplyModifiedProperties();
        }
    }    
}
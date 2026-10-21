// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEngine;

namespace Fungus.EditorUtils
{
    [CustomEditor (typeof(Character))]
    public class CharacterEditor : Editor
    {
        protected SerializedProperty nameTextProp;
        protected SerializedProperty nameColorProp;
        protected SerializedProperty soundEffectProp;
        protected SerializedProperty portraitsProp;
        protected SerializedProperty portraitsFaceProp;
        protected SerializedProperty descriptionProp;
        protected SerializedProperty setSayDialogProp;
        protected SerializedProperty effectAudioSourceProp;
        protected SerializedProperty voiceAudioSourceProp;

        protected virtual void OnEnable()
        {
            nameTextProp = serializedObject.FindProperty ("nameText");
            nameColorProp = serializedObject.FindProperty ("nameColor");
            soundEffectProp = serializedObject.FindProperty ("soundEffect");
            portraitsProp = serializedObject.FindProperty ("portraits");
            portraitsFaceProp = serializedObject.FindProperty ("portraitsFace");
            descriptionProp = serializedObject.FindProperty ("description");
            setSayDialogProp = serializedObject.FindProperty("setSayDialog");
            effectAudioSourceProp = serializedObject.FindProperty("effectAudioSource");
            voiceAudioSourceProp = serializedObject.FindProperty("voiceAudioSource");
        }

        public override void OnInspectorGUI() 
        {
            serializedObject.Update();

            Character t = target as Character;
            EditorGUI.BeginChangeCheck();

            FungusEditorGUI.PropertyField(nameTextProp, new GUIContent("角色姓名", "玩家在对白框中看到的姓名"));
            FungusEditorGUI.PropertyField(nameColorProp, new GUIContent("姓名颜色", "对白框中显示姓名时使用的颜色"));
            FungusEditorGUI.PropertyField(soundEffectProp, new GUIContent("角色打字音效", "角色发言时使用的打字音效 会覆盖对白界面的默认音效"));
            FungusEditorGUI.PropertyField(effectAudioSourceProp);
            FungusEditorGUI.PropertyField(voiceAudioSourceProp);
            FungusEditorGUI.PropertyField(setSayDialogProp);
            FungusEditorGUI.PropertyField(descriptionProp, new GUIContent("用途说明", "填写角色性格 背景等创作备注"));

            if (t.Portraits != null &&
                t.Portraits.Count > 0)
            {
                t.ProfileSprite = t.Portraits[0];
            }
            else
            {
                t.ProfileSprite = null;
            }
            
            if (t.ProfileSprite != null)
            {
                Texture2D characterTexture = t.ProfileSprite.texture;
                float aspect = (float)characterTexture.width / (float)characterTexture.height;
                Rect previewRect = GUILayoutUtility.GetAspectRect(aspect, GUILayout.Width(100), GUILayout.ExpandWidth(true));
                if (characterTexture != null)
                    GUI.DrawTexture(previewRect,characterTexture,ScaleMode.ScaleToFit,true,aspect);
            }

            FungusEditorGUI.PropertyField(portraitsProp, new GUIContent("可用立绘", "角色可在对白界面中显示的立绘图片"), true);

            EditorGUILayout.HelpBox("同一角色的立绘请使用相同分辨率 避免切换表情时位置或大小跳变", MessageType.Info);

            EditorGUILayout.Separator();

            string[] facingArrows = new string[]
            {
                "前排",
                "<--",
                "-->",
            };
            portraitsFaceProp.enumValueIndex = EditorGUILayout.Popup("立绘默认朝向", (int)portraitsFaceProp.enumValueIndex, facingArrows);

            EditorGUILayout.Separator();

            if(EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(t);

            serializedObject.ApplyModifiedProperties();
        }

    }
}
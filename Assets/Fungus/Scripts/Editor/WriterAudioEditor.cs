// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEngine;

namespace Fungus.EditorUtils
{
    [CustomEditor (typeof(WriterAudio))]
    public class WriterAudioEditor : Editor
    {
        protected SerializedProperty volumeProp;
        protected SerializedProperty loopProp;
        protected SerializedProperty targetAudioSourceProp;
        protected SerializedProperty audioModeProp;
        protected SerializedProperty beepSoundsProp;
        protected SerializedProperty soundEffectProp;
        protected SerializedProperty inputSoundProp;
        protected SerializedProperty useLegacyAudioLogicProp;

        protected virtual void OnEnable()
        {
            volumeProp = serializedObject.FindProperty("volume");
            loopProp = serializedObject.FindProperty("loop");
            targetAudioSourceProp = serializedObject.FindProperty("targetAudioSource");
            inputSoundProp = serializedObject.FindProperty("inputSound");
            audioModeProp = serializedObject.FindProperty("audioMode");
            beepSoundsProp = serializedObject.FindProperty("beepSounds");
            soundEffectProp = serializedObject.FindProperty("soundEffect");
            useLegacyAudioLogicProp = serializedObject.FindProperty("useLegacyAudioLogic");
        }

        public override void OnInspectorGUI() 
        {
            serializedObject.Update();

            FungusEditorGUI.PropertyField(volumeProp);
            FungusEditorGUI.PropertyField(loopProp);
            FungusEditorGUI.PropertyField(useLegacyAudioLogicProp);
            if (useLegacyAudioLogicProp.boolValue)
            {
                FungusEditorGUI.PropertyField(targetAudioSourceProp);
            }
            FungusEditorGUI.PropertyField(inputSoundProp);

            FungusEditorGUI.PropertyField(audioModeProp);
            if ((AudioMode)audioModeProp.enumValueIndex == AudioMode.Beeps)
            {
                FungusEditorGUI.PropertyField(beepSoundsProp, new GUIContent("打字音效列表", "打字时从这些音效中随机选择播放"),true);
            }
            else
            {
                FungusEditorGUI.PropertyField(soundEffectProp);
            }

            if( ( ((MonoBehaviour)target).GetComponent<AudioSource>() != null || targetAudioSourceProp.objectReferenceValue != null) &&
                !useLegacyAudioLogicProp.boolValue)
            {
                EditorGUILayout.HelpBox("已找到音频播放器 但当前配置不会使用它 " +
                    "\nToggle useLegacyAudioLogic to use the targetAudioSource and not the AudioSources that might be provided by the Character", MessageType.Warning);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }    
}
using Fungus.EditorUtils;
using UnityEditor;
using UnityEngine;

namespace AmanitaWebStory.Editor
{
    [CustomEditor(typeof(WebStoryHost))]
    [CanEditMultipleObjects]
    public sealed class WebStoryHostEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var Property = serializedObject.GetIterator();
            bool EnterChildren = true;
            while (Property.NextVisible(EnterChildren))
            {
                EnterChildren = false;
                using (new EditorGUI.DisabledScope(Property.propertyPath == "m_Script"))
                    FungusEditorGUI.PropertyField(Property, true);
            }
            serializedObject.ApplyModifiedProperties();
            using (new EditorGUI.DisabledScope(!Application.isPlaying))
                if (GUILayout.Button("从头播放"))
                    foreach (var Target in targets)
                        ((WebStoryHost)Target).Play();
        }
    }
}

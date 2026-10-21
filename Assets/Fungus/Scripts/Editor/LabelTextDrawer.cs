using Fungus.Inspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Fungus.EditorUtils
{
    [CustomPropertyDrawer(typeof(LabelTextAttribute))]
    public sealed class LabelTextDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect Position, SerializedProperty Property, GUIContent Label)
        {
            var LocalizedLabel = FungusEditorGUI.Label(Property, Label);
            EditorGUI.PropertyField(Position, Property, LocalizedLabel, true);
        }

        public override float GetPropertyHeight(SerializedProperty Property, GUIContent Label)
        {
            return EditorGUI.GetPropertyHeight(Property, Label, true);
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty Property)
        {
            var PropertyCopy = Property.Copy();
            return new IMGUIContainer(() =>
            {
                PropertyCopy.serializedObject.UpdateIfRequiredOrScript();
                float Height = EditorGUI.GetPropertyHeight(PropertyCopy, true);
                var Position = EditorGUILayout.GetControlRect(true, Height);
                EditorGUI.PropertyField(Position, PropertyCopy, new GUIContent(((LabelTextAttribute)attribute).Text), true);
                PropertyCopy.serializedObject.ApplyModifiedProperties();
            });
        }
    }
}

using System.Collections.Generic;
using Fungus.EditorUtils;
using UnityEditor;
using UnityEngine;

namespace AmanitaWebStory.Editor
{
    [CustomPropertyDrawer(typeof(StoryEventBinding))]
    public sealed class StoryEventBindingDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect Position, SerializedProperty Property, GUIContent Label)
        {
            EditorGUI.BeginProperty(Position, Label, Property);
            var Line = new Rect(Position.x, Position.y, Position.width, EditorGUIUtility.singleLineHeight);
            Property.isExpanded = EditorGUI.Foldout(Line, Property.isExpanded, Label, true);
            if (Property.isExpanded)
            {
                int Indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel++;
                float Y = Line.yMax + EditorGUIUtility.standardVerticalSpacing;
                foreach (string Name in FieldNames(Property))
                {
                    var Child = Property.FindPropertyRelative(Name);
                    float Height = EditorGUI.GetPropertyHeight(Child, true);
                    FungusEditorGUI.PropertyField(new Rect(Position.x, Y, Position.width, Height), Child, true);
                    Y += Height + EditorGUIUtility.standardVerticalSpacing;
                }
                EditorGUI.indentLevel = Indent;
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty Property, GUIContent Label)
        {
            float Height = EditorGUIUtility.singleLineHeight;
            if (Property.isExpanded)
                foreach (string Name in FieldNames(Property))
                    Height += EditorGUI.GetPropertyHeight(Property.FindPropertyRelative(Name), true) + EditorGUIUtility.standardVerticalSpacing;
            return Height;
        }

        private static IEnumerable<string> FieldNames(SerializedProperty Property)
        {
            foreach (string Name in new[] { "Id", "Title", "Help", "Kind", "ParameterLabel", "ParameterHelp", "DefaultValue" })
                yield return Name;
            switch ((eStoryValueKind)Property.FindPropertyRelative("Kind").enumValueIndex)
            {
                case eStoryValueKind.None: yield return "OnInvoke"; break;
                case eStoryValueKind.String: yield return "OnString"; break;
                case eStoryValueKind.Integer: yield return "OnInteger"; break;
                case eStoryValueKind.Float: yield return "OnFloat"; break;
                case eStoryValueKind.Boolean: yield return "OnBoolean"; break;
            }
        }
    }
}

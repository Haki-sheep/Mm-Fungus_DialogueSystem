using System;
using System.Collections.Generic;
using System.Reflection;
using Fungus.Inspector;
using UnityEditor;
using UnityEngine;

namespace Fungus.EditorUtils
{
    public static class FungusEditorGUI
    {
        private static readonly Dictionary<Type, Dictionary<string, FieldInfo>> fieldCacheDict =
            new Dictionary<Type, Dictionary<string, FieldInfo>>();

        public static GUIContent Label(SerializedProperty Property, GUIContent ExplicitLabel = null)
        {
            if (ExplicitLabel == GUIContent.none)
                return ExplicitLabel;
            var Field = FindField(Property.serializedObject.targetObject.GetType(), Property.propertyPath);
            var Attribute = Field?.GetCustomAttribute<LabelTextAttribute>();
            string Text = ExplicitLabel?.text;
            if (string.IsNullOrEmpty(Text) || Text == Property.displayName)
                Text = Attribute?.Text ?? Property.displayName;
            string Tooltip = ExplicitLabel?.tooltip;
            if (string.IsNullOrEmpty(Tooltip))
                Tooltip = Field?.GetCustomAttribute<TooltipAttribute>()?.tooltip ?? Property.tooltip;
            return new GUIContent(Text, Tooltip);
        }

        public static bool PropertyField(SerializedProperty Property, params GUILayoutOption[] Options)
        {
            return PropertyField(Property, null, false, Options);
        }

        public static bool PropertyField(SerializedProperty Property, bool IncludeChildren, params GUILayoutOption[] Options)
        {
            return PropertyField(Property, null, IncludeChildren, Options);
        }

        public static bool PropertyField(SerializedProperty Property, GUIContent Label, params GUILayoutOption[] Options)
        {
            return PropertyField(Property, Label, false, Options);
        }

        public static bool PropertyField(SerializedProperty Property, GUIContent Label, bool IncludeChildren,
            params GUILayoutOption[] Options)
        {
            return EditorGUILayout.PropertyField(Property, FungusEditorGUI.Label(Property, Label), IncludeChildren, Options);
        }

        public static bool PropertyField(Rect Rect, SerializedProperty Property)
        {
            return PropertyField(Rect, Property, null, false);
        }

        public static bool PropertyField(Rect Rect, SerializedProperty Property, GUIContent Label)
        {
            return PropertyField(Rect, Property, Label, false);
        }

        public static bool PropertyField(Rect Rect, SerializedProperty Property, bool IncludeChildren)
        {
            return PropertyField(Rect, Property, null, IncludeChildren);
        }

        public static bool PropertyField(Rect Rect, SerializedProperty Property, GUIContent Label, bool IncludeChildren)
        {
            return EditorGUI.PropertyField(Rect, Property, FungusEditorGUI.Label(Property, Label), IncludeChildren);
        }

        private static FieldInfo FindField(Type Type, string Path)
        {
            if (!fieldCacheDict.TryGetValue(Type, out var FieldDict))
            {
                FieldDict = new Dictionary<string, FieldInfo>();
                fieldCacheDict.Add(Type, FieldDict);
            }
            string NormalizedPath = System.Text.RegularExpressions.Regex.Replace(Path, @"data\[\d+\]", "data[]");
            if (FieldDict.TryGetValue(NormalizedPath, out var Cached))
                return Cached;
            var CurrentType = Type;
            FieldInfo Field = null;
            foreach (string Segment in NormalizedPath.Split('.'))
            {
                if (Segment == "Array")
                    continue;
                if (Segment == "data[]")
                {
                    CurrentType = CurrentType.IsArray ? CurrentType.GetElementType() : CurrentType.GetGenericArguments()[0];
                    continue;
                }
                Field = null;
                for (var Parent = CurrentType; Parent != null; Parent = Parent.BaseType)
                {
                    Field = Parent.GetField(Segment, BindingFlags.Instance | BindingFlags.Public |
                        BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                    if (Field != null)
                        break;
                }
                if (Field == null)
                    break;
                CurrentType = Field.FieldType;
            }
            FieldDict.Add(NormalizedPath, Field);
            return Field;
        }
    }
}

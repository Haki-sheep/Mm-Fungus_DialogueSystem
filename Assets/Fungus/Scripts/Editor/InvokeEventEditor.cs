// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;

namespace Fungus.EditorUtils
{
    [CustomEditor (typeof(InvokeEvent))]
    public class InvokeEventEditor : CommandEditor 
    {
        protected SerializedProperty descriptionProp;
        protected SerializedProperty delayProp;
        protected SerializedProperty invokeTypeProp;
        protected SerializedProperty staticEventProp;
        protected SerializedProperty booleanParameterProp;
        protected SerializedProperty booleanEventProp;
        protected SerializedProperty integerParameterProp;
        protected SerializedProperty integerEventProp;
        protected SerializedProperty floatParameterProp;
        protected SerializedProperty floatEventProp;
        protected SerializedProperty stringParameterProp;
        protected SerializedProperty stringEventProp;

        public override void OnEnable()
        {
            base.OnEnable();

            descriptionProp = serializedObject.FindProperty("description");
            delayProp = serializedObject.FindProperty("delay");
            invokeTypeProp = serializedObject.FindProperty("invokeType");
            staticEventProp = serializedObject.FindProperty("staticEvent");
            booleanParameterProp = serializedObject.FindProperty("booleanParameter");
            booleanEventProp = serializedObject.FindProperty("booleanEvent");
            integerParameterProp = serializedObject.FindProperty("integerParameter");
            integerEventProp = serializedObject.FindProperty("integerEvent");
            floatParameterProp = serializedObject.FindProperty("floatParameter");
            floatEventProp = serializedObject.FindProperty("floatEvent");
            stringParameterProp = serializedObject.FindProperty("stringParameter");
            stringEventProp = serializedObject.FindProperty("stringEvent");
        }

        public override void DrawCommandGUI()
        {
            serializedObject.Update();

            FungusEditorGUI.PropertyField(descriptionProp);
            FungusEditorGUI.PropertyField(delayProp);
            FungusEditorGUI.PropertyField(invokeTypeProp);

            switch ((InvokeType)invokeTypeProp.enumValueIndex)
            {
            case InvokeType.Static:
                FungusEditorGUI.PropertyField(staticEventProp);
                break;
            case InvokeType.DynamicBoolean:
                FungusEditorGUI.PropertyField(booleanEventProp);
                FungusEditorGUI.PropertyField(booleanParameterProp);
                break;
            case InvokeType.DynamicInteger:
                FungusEditorGUI.PropertyField(integerEventProp);
                FungusEditorGUI.PropertyField(integerParameterProp);
                break;
            case InvokeType.DynamicFloat:
                FungusEditorGUI.PropertyField(floatEventProp);
                FungusEditorGUI.PropertyField(floatParameterProp);
                break;
            case InvokeType.DynamicString:
                FungusEditorGUI.PropertyField(stringEventProp);
                FungusEditorGUI.PropertyField(stringParameterProp);
                break;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

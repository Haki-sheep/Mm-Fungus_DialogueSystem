// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEngine;

namespace Fungus.EditorUtils
{
    [CustomEditor (typeof(Call))]
    public class CallEditor : CommandEditor 
    {
        protected SerializedProperty targetFlowchartProp;
        protected SerializedProperty targetBlockProp;
        protected SerializedProperty startLabelProp;
        protected SerializedProperty startIndexProp;
        protected SerializedProperty callModeProp;

        public override void OnEnable()
        {
            base.OnEnable();

            targetFlowchartProp = serializedObject.FindProperty("targetFlowchart");
            targetBlockProp = serializedObject.FindProperty("targetBlock");
            startLabelProp = serializedObject.FindProperty("startLabel");
            startIndexProp = serializedObject.FindProperty("startIndex");
            callModeProp = serializedObject.FindProperty("callMode");
        }

        public override void DrawCommandGUI()
        {
            serializedObject.Update();

            Call t = target as Call;

            Flowchart flowchart = null;
            if (targetFlowchartProp.objectReferenceValue == null)
            {
                flowchart = (Flowchart)t.GetFlowchart();
            }
            else
            {
                flowchart = targetFlowchartProp.objectReferenceValue as Flowchart;
            }

            FungusEditorGUI.PropertyField(targetFlowchartProp);

            if (flowchart != null)
            {
                BlockEditor.BlockField(targetBlockProp,
                                       new GUIContent("目标流程", "选择要调用的流程"), 
                                       new GUIContent("未设置"), 
                                       flowchart);

                FungusEditorGUI.PropertyField(startLabelProp);

                FungusEditorGUI.PropertyField(startIndexProp);
            }

            FungusEditorGUI.PropertyField(callModeProp);

            serializedObject.ApplyModifiedProperties();
        }
    }
}

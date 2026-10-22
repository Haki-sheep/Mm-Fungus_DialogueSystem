using UnityEditor;
using UnityEngine;

namespace AmanitaWebStory.Editor
{
    [CustomEditor(typeof(StoryAsset))]
    public sealed class StoryAssetEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var Story = (StoryAsset)target;
            EditorGUILayout.LabelField(Story.Title, EditorStyles.boldLabel);
            EditorGUILayout.LabelField("标识", Story.Id);
            EditorGUILayout.LabelField("入口", Story.Entry);
            EditorGUILayout.LabelField("内容", $"{Story.Steps.Count} 个节点 · {Story.Variables.Count} 个初始变量");
            EditorGUILayout.HelpBox("将此资产交给 StoryPlayer 开始播放\n内容在网页编辑后重新导出\n事件请通过播放器 EventRaised 接入场景业务", MessageType.Info);
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("background"), new GUIContent("剧情背景"));
            serializedObject.ApplyModifiedProperties();
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("variableList"), new GUIContent("初始变量"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("stepList"), new GUIContent("剧情节点"), true);
            }
        }
    }
}

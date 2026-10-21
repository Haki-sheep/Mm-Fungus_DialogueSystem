using System;
using System.IO;
using System.Reflection;
using Fungus;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AmanitaWebStory.Editor
{
    public static class StoryTemplateBuilder
    {
        public const string Root = "Assets/AmanitaWebStory";
        public const string TemplatePath = Root + "/StoryTemplate.prefab";
        public const string GeneratedRoot = Root + "/Generated";
        private const string NativePrefabs = "Assets/Fungus/Resources/Prefabs/";

        #region 模板构建

        public static void CreateIfMissing()
        {
            if (AssetDatabase.LoadAssetAtPath<GameObject>(TemplatePath) != null)
                return;
            var Scene = EditorSceneManager.NewPreviewScene();
            GameObject RootObject = null;
            try
            {
                RootObject = new GameObject("网页剧情模板");
                UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(RootObject, Scene);
                var Host = RootObject.AddComponent<WebStoryHost>();
                Host.Flowchart = RootObject.AddComponent<Flowchart>();
                Host.Flowchart.Version = FungusConstants.CurrentVersion;
                Host.AutoPlay = true;
                Host.EventSystemPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(NativePrefabs + "EventSystem.prefab");
                var Font = AssetDatabase.LoadAssetAtPath<Font>(Root + "/Fonts/MmDialogueSC.ttf");
                if (Font == null)
                    throw new FileNotFoundException("默认对白字体尚未导入");
                Host.SayDialog = InstantiateDialog<SayDialog>("SayDialog", RootObject.transform, Font);
                Host.MenuDialog = InstantiateDialog<MenuDialog>("MenuDialog", RootObject.transform, Font);
                Set(Host.SayDialog.GetComponent<Writer>(), "writingSpeed", 36f);
                Set(Host.SayDialog.GetComponent<DialogInput>(), "clickMode", ClickMode.ClickOnDialog);
                Set(Host.SayDialog, "fitTextWithImage", false);
                var Continue = new SerializedObject(Host.SayDialog).FindProperty("continueButton").objectReferenceValue as UnityEngine.UI.Button;
                foreach (var Text in Host.SayDialog.GetComponentsInChildren<Text>(true))
                    if (Text.transform.IsChildOf(Continue.transform))
                        Text.text = "继续";
                var ActorObject = new GameObject("speaker");
                ActorObject.transform.SetParent(RootObject.transform, false);
                var Actor = ActorObject.AddComponent<Character>();
                Set(Actor, "nameText", "角色");
                PrefabUtility.SaveAsPrefabAsset(RootObject, TemplatePath, out bool Success);
                if (!Success)
                    throw new IOException("网页剧情模板保存失败");
            }
            finally
            {
                if (RootObject != null)
                    UnityEngine.Object.DestroyImmediate(RootObject);
                EditorSceneManager.ClosePreviewScene(Scene);
            }
        }

        private static T InstantiateDialog<T>(string Name, Transform Parent, Font Font) where T : Component
        {
            var Prefab = AssetDatabase.LoadAssetAtPath<GameObject>(NativePrefabs + Name + ".prefab");
            var Instance = UnityEngine.Object.Instantiate(Prefab, Parent);
            Instance.name = Name;
            foreach (var Text in Instance.GetComponentsInChildren<Text>(true))
                Text.font = Font;
            Instance.SetActive(false);
            return Instance.GetComponent<T>();
        }

        #endregion

        #region 序列化字段桥接

        public static void Set(object Target, string Name, object Value)
        {
            for (var Type = Target.GetType(); Type != null; Type = Type.BaseType)
            {
                var Field = Type.GetField(Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                if (Field == null)
                    continue;
                Field.SetValue(Target, Value);
                return;
            }
            throw new MissingFieldException(Target.GetType().FullName, Name);
        }

        #endregion
    }
}

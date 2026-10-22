using System;
using System.IO;
using AmanitaWebStory.Samples;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

namespace AmanitaWebStory.Editor
{
    public static class StorySeparationSampleBuilder
    {
        public const string Root = StoryAssetExporter.Root + "/Samples/Separation";
        public const string ScenePath = Root + "/StorySeparation.unity";

        [MenuItem("Tools/MmFungus/打开分离试用示例", false, 20)]
        private static async void Open()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                throw new InvalidOperationException("请退出 Play 后打开示例");
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;
            if (!File.Exists(ScenePath))
            {
                var Active = SceneManager.GetActiveScene();
                if (string.IsNullOrEmpty(Active.path) && !EditorSceneManager.SaveScene(Active))
                    return;
                await BuildAsync();
            }
            EditorSceneManager.OpenScene(ScenePath);
        }

        public static async UniTask<string> BuildAsync()
        {
            if (File.Exists(ScenePath))
                throw new InvalidOperationException("示例已存在 不覆盖已有修改 " + ScenePath);
            if (string.IsNullOrEmpty(SceneManager.GetActiveScene().path))
                throw new InvalidOperationException("创建示例前请先保存当前未命名场景");
            if (Resources.Load<TMP_Settings>("TMP Settings") == null)
            {
                TMP_PackageResourceImporter.ImportResources(true, false, false);
                await UniTask.WaitUntil(() => Resources.Load<TMP_Settings>("TMP Settings") != null).Timeout(TimeSpan.FromMinutes(1));
            }
            var Font = CreateFont();
            CreateActor();
            var Story = StoryAssetExporter.Export(File.ReadAllText(Root + "/GateStory.json"), Root + "/GateStory.asset");
            var Dialogue = DialogueSampleUi.CreateView(Root + "/DialogueView.prefab", Font, false);
            var Radio = DialogueSampleUi.CreateView(Root + "/RadioView.prefab", Font, true);
            var Previous = SceneManager.GetActiveScene();
            var Scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            try
            {
                SceneManager.SetActiveScene(Scene);
                var Camera = new GameObject("Camera").AddComponent<Camera>();
                Camera.clearFlags = CameraClearFlags.SolidColor;
                Camera.backgroundColor = new Color(0.04f, 0.07f, 0.11f);
                Camera.gameObject.AddComponent<AudioListener>();
                var EventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
                var Player = new GameObject("StoryPlayer").AddComponent<StoryPlayer>();
                var DialogueInstance = (GameObject)PrefabUtility.InstantiatePrefab(Dialogue, Scene);
                var RadioInstance = (GameObject)PrefabUtility.InstantiatePrefab(Radio, Scene);
                DialogueInstance.name = "对白界面";
                RadioInstance.name = "通讯界面";
                var DialogueView = DialogueInstance.GetComponent<DialogueView>();
                var RadioView = RadioInstance.GetComponent<DialogueView>();
                DialogueSampleUi.Set(Player, "story", Story);
                DialogueSampleUi.Set(Player, "view", DialogueView);
                var Demo = new GameObject("DemoControls").AddComponent<StorySeparationDemo>();
                var Canvas = DialogueSampleUi.CanvasObject(Demo.transform, "Controls");
                Canvas.GetComponent<Canvas>().sortingOrder = 10;
                DialogueSampleUi.Label(Canvas.transform, "Title", "同一份剧情 · 两种界面", Font, new Vector2(0.06f, 0.88f), new Vector2(0.94f, 0.98f), 36, Color.white);
                var Status = DialogueSampleUi.Label(Canvas.transform, "Status", "选择界面开始  |  选择不同答案观察分支与业务事件", Font,
                    new Vector2(0.06f, 0.78f), new Vector2(0.94f, 0.88f), 21, new Color(0.65f, 0.78f, 0.82f));
                var First = DialogueSampleUi.Button(Canvas.transform, "PlayDialogue", "对白界面", Font,
                    new Vector2(0.06f, 0.65f), new Vector2(0.20f, 0.74f), Color.white, Color.black);
                var Second = DialogueSampleUi.Button(Canvas.transform, "PlayRadio", "通讯界面", Font,
                    new Vector2(0.22f, 0.65f), new Vector2(0.36f, 0.74f), Color.white, Color.black);
                var Stop = DialogueSampleUi.Button(Canvas.transform, "Stop", "停止", Font,
                    new Vector2(0.38f, 0.65f), new Vector2(0.49f, 0.74f), Color.white, Color.black);
                DialogueSampleUi.Set(Demo, "story", Story); DialogueSampleUi.Set(Demo, "player", Player);
                DialogueSampleUi.Set(Demo, "dialogueView", DialogueView); DialogueSampleUi.Set(Demo, "radioView", RadioView);
                DialogueSampleUi.Set(Demo, "statusText", Status);
                UnityEventTools.AddPersistentListener(First.onClick, Demo.PlayDialogue);
                UnityEventTools.AddPersistentListener(Second.onClick, Demo.PlayRadio);
                UnityEventTools.AddPersistentListener(Stop.onClick, Demo.Stop);
                if (!EditorSceneManager.SaveScene(Scene, ScenePath))
                    throw new IOException("示例场景保存失败 " + ScenePath);
            }
            finally
            {
                EditorSceneManager.CloseScene(Scene, true);
                SceneManager.SetActiveScene(Previous);
            }
            AssetDatabase.SaveAssets();
            return ScenePath;
        }

        private static void CreateActor()
        {
            string Path = Root + "/GuardActor.asset";
            if (AssetDatabase.LoadAssetAtPath<StoryActor>(Path) != null)
                return;
            var Actor = ScriptableObject.CreateInstance<StoryActor>();
            DialogueSampleUi.Set(Actor, "actorId", "speaker");
            DialogueSampleUi.Set(Actor, "displayName", "守卫");
            DialogueSampleUi.Set(Actor, "nameColor", new Color(0.28f, 0.76f, 0.65f));
            AssetDatabase.CreateAsset(Actor, Path);
            AssetDatabase.SaveAssets();
        }

        private static TMP_FontAsset CreateFont()
        {
            string Path = Root + "/DialogueFont.asset";
            var Existing = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(Path);
            if (Existing != null)
                return Existing;
            var Source = AssetDatabase.LoadAssetAtPath<Font>(StoryAssetExporter.Root + "/Fonts/MmDialogueSC.ttf");
            var Font = TMP_FontAsset.CreateFontAsset(Source);
            Font.name = "DialogueFont";
            AssetDatabase.CreateAsset(Font, Path);
            AssetDatabase.AddObjectToAsset(Font.material, Font);
            foreach (var Atlas in Font.atlasTextures)
                AssetDatabase.AddObjectToAsset(Atlas, Font);
            EditorUtility.SetDirty(Font);
            AssetDatabase.SaveAssets();
            return Font;
        }
    }
}

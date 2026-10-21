// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Fungus
{
    namespace EditorUtils
    {
        /// <summary>
        /// Shows Fungus section in the Edit->Preferences in unity allows you to configure Fungus behaviour
        /// 
        /// ref https://docs.unity3d.com/ScriptReference/PreferenceItem.html
        /// </summary>
        [InitializeOnLoad]
        public static class FungusEditorPreferences
        {
            // Have we loaded the prefs yet
            private static bool prefsLoaded = false;
            private const string HIDE_MUSH_KEY = "hideMushroomInHierarchy";
            private const string USE_LEGACY_MENUS = "useLegacyMenus";
            private const string USE_GRID_SNAP = "useGridSnap";
            private const string COMMAND_LIST_ITEM_TINT = "commandListTint";
            private const string FLOWCHART_WINDIOW_BLOCK_TINT = "flowchartWindowBlockTint";
            private const string SUPPRESS_HELP_BOXES = "suppressHelpBoxes";
            private const string NAV_CMD_WITH_ARROWS = "navigateCmdListWithArrows";

            public static bool hideMushroomInHierarchy;
            public static bool useLegacyMenus;
            public static bool useGridSnap;
            public static Color commandListTint = Color.white;
            public static Color flowchatBlockTint = Color.white;
            public static bool suppressHelpBoxes = false;
            public static bool navigateCmdListWithArrows = false;

            static FungusEditorPreferences()
            {
                LoadOnScriptLoad();
            }

#if UNITY_2019_1_OR_NEWER
            [SettingsProvider]
            public static SettingsProvider CreateFungusSettingsProvider()
            {
                // First parameter is the path in the Settings window.
                // Second parameter is the scope of this setting: it only appears in the Project Settings window.
                var provider = new SettingsProvider("Project/Amanita 剧情工具", SettingsScope.Project)
                {
                    // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
                    guiHandler = (searchContext) => PreferencesGUI()

                    // // Populate the search keywords to enable smart search filtering and label highlighting:
                    // keywords = new HashSet<string>(new[] { "Number", "Some String" })
                };

                return provider;
            }

#else

            [PreferenceItem("Fungus")]
#endif
            private static void PreferencesGUI()
            {
                // Load the preferences
                if (!prefsLoaded)
                {
                    LoadOnScriptLoad();
                }

                // Preferences GUI
                hideMushroomInHierarchy = EditorGUILayout.Toggle("隐藏流程图蘑菇图标", hideMushroomInHierarchy);
                useLegacyMenus = EditorGUILayout.Toggle(new GUIContent("使用旧版菜单", "触发事件 添加变量和添加步骤时使用旧版菜单"), useLegacyMenus);
                useGridSnap = EditorGUILayout.Toggle(new GUIContent("对齐网格", "将节点的位置和宽度吸附到画布网格"), useGridSnap);
                flowchatBlockTint = EditorGUILayout.ColorField(new GUIContent("流程节点底色", "流程图节点使用的底色 默认为白色"), flowchatBlockTint);
                commandListTint = EditorGUILayout.ColorField(new GUIContent("执行步骤列表底色", "流程面板中执行步骤列表使用的底色 默认为白色"), commandListTint);
                suppressHelpBoxes = EditorGUILayout.Toggle(new GUIContent("隐藏操作说明", "隐藏流程面板中触发事件和执行步骤下方的说明"), suppressHelpBoxes);
                navigateCmdListWithArrows = EditorGUILayout.Toggle(new GUIContent("允许方向键选择步骤", "在执行步骤列表中使用上下方向键切换选中项"), navigateCmdListWithArrows);

                EditorGUILayout.Space();
                //ideally if any are null, but typically it is all or nothing that have broken links due to version changes or moving files external to Unity
                if (FungusEditorResources.Add == null)
                {
                    EditorGUILayout.HelpBox("编辑器图标资源引用不完整 请重新同步资源", MessageType.Error);
                }

                if (GUILayout.Button(new GUIContent("定位编辑器资源配置", "图标显示异常时 请检查编辑器资源配置中的引用 点击下方按钮可定位配置")))
                {
                    var ids = AssetDatabase.FindAssets("t:FungusEditorResources");
                    if (ids.Length > 0)
                    {
                        var p = AssetDatabase.GUIDToAssetPath(ids[0]);
                        var asset = AssetDatabase.LoadAssetAtPath<FungusEditorResources>(p);
                        Selection.activeObject = asset;
                    }
                    else
                    {
                        Debug.LogError("No FungusEditorResources found!");
                    }
                }

                if (GUILayout.Button("打开版本变更记录"))
                {
                    //From project path down, look for our Fungus\Docs\ChangeLog.txt
                    var projectPath = System.IO.Directory.GetParent(Application.dataPath);
                    var fileMacthes = System.IO.Directory.GetFiles(projectPath.FullName, "CHANGELOG.txt", System.IO.SearchOption.AllDirectories);

                    fileMacthes = fileMacthes.Where((x) =>
                    {
                        var fileFolder = System.IO.Directory.GetParent(x);
                        return fileFolder.Name == "Docs" && fileFolder.Parent.Name == "Fungus";
                    }).ToArray();

                    if (fileMacthes == null || fileMacthes.Length == 0)
                    {
                        Debug.LogWarning("Cannot locate Fungus\\Docs\\CHANGELONG.txt");
                    }
                    else
                    {
                        Application.OpenURL(fileMacthes[0]);
                    }
                }

                // Save the preferences
                if (GUI.changed)
                {
                    EditorPrefs.SetBool(HIDE_MUSH_KEY, hideMushroomInHierarchy);
                    EditorPrefs.SetBool(USE_LEGACY_MENUS, useLegacyMenus);
                    EditorPrefs.SetBool(USE_GRID_SNAP, useGridSnap);
                    var colAsString = "#" + ColorUtility.ToHtmlStringRGBA(commandListTint);
                    EditorPrefs.SetString(COMMAND_LIST_ITEM_TINT, colAsString); 
                    colAsString = "#" + ColorUtility.ToHtmlStringRGBA(flowchatBlockTint);
                    EditorPrefs.SetString(FLOWCHART_WINDIOW_BLOCK_TINT, colAsString);
                    EditorPrefs.SetBool(SUPPRESS_HELP_BOXES, suppressHelpBoxes); 
                    EditorPrefs.SetBool(NAV_CMD_WITH_ARROWS, navigateCmdListWithArrows); 
                }
            }

            public static void LoadOnScriptLoad()
            {
                hideMushroomInHierarchy = EditorPrefs.GetBool(HIDE_MUSH_KEY, false);
                useLegacyMenus = EditorPrefs.GetBool(USE_LEGACY_MENUS, false);
                useGridSnap = EditorPrefs.GetBool(USE_GRID_SNAP, false);

                if(ColorUtility.TryParseHtmlString(EditorPrefs.GetString(COMMAND_LIST_ITEM_TINT), out var col))
                {
                    commandListTint = col;
                }
                if (ColorUtility.TryParseHtmlString(EditorPrefs.GetString(FLOWCHART_WINDIOW_BLOCK_TINT), out col))
                {
                    flowchatBlockTint = col;
                }
                suppressHelpBoxes = EditorPrefs.GetBool(SUPPRESS_HELP_BOXES, false);
                navigateCmdListWithArrows = EditorPrefs.GetBool(NAV_CMD_WITH_ARROWS, false);
                prefsLoaded = true;
            }
        }
    }
}

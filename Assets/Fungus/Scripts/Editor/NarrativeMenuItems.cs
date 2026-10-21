// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEditor;

namespace Fungus.EditorUtils
{
    // The prefab names are prefixed with Fungus to avoid clashes with any other prefabs in the project
    public class NarrativeMenuItems 
    {

        [MenuItem("Tools/Amanita/创建/角色", false, 50)]
        static void CreateCharacter()
        {
            GameObject go = FlowchartMenuItems.SpawnPrefab("Character");
            go.transform.position = Vector3.zero;
        }

        [MenuItem("Tools/Amanita/创建/对白界面", false, 51)]
        static void CreateSayDialog()
        {
            GameObject go = FlowchartMenuItems.SpawnPrefab("SayDialog");
            go.transform.position = Vector3.zero;
        }

        [MenuItem("Tools/Amanita/创建/选项界面", false, 52)]
        static void CreateMenuDialog()
        {
            GameObject go = FlowchartMenuItems.SpawnPrefab("MenuDialog");
            go.transform.position = Vector3.zero;
        }

        [MenuItem("Tools/Amanita/创建/文字标签", false, 53)]
        static void CreateTag()
        {
            GameObject go = FlowchartMenuItems.SpawnPrefab("Tag");
            go.transform.position = Vector3.zero;
        }

        [MenuItem("Tools/Amanita/创建/音频标签", false, 54)]
        static void CreateAudioTag()
        {
            GameObject go = FlowchartMenuItems.SpawnPrefab("AudioTag");
            go.transform.position = Vector3.zero;
        }

        [MenuItem("Tools/Amanita/创建/立绘舞台", false, 55)]
        static void CreateStage()
        {
            GameObject go = FlowchartMenuItems.SpawnPrefab("Stage");
            go.transform.position = Vector3.zero;
        }
        
        [MenuItem("Tools/Amanita/创建/立绘站位", false, 56)]
        static void CreateStagePosition()
        {
            FlowchartMenuItems.SpawnPrefab("StagePosition");
        }

        [MenuItem("Tools/Amanita/创建/多语言翻译", false, 57)]
        static void CreateLocalization()
        {
            GameObject go = FlowchartMenuItems.SpawnPrefab("Localization");
            go.transform.position = Vector3.zero;
        }
    }
}

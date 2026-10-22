// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEditor;

namespace Fungus.EditorUtils
{
    // The prefab names are prefixed with Fungus to avoid clashes with any other prefabs in the project
    public class NarrativeMenuItems 
    {

        [MenuItem("Tools/MmFungus/Fungus 原生/创建角色", false, 121)]
        static void CreateCharacter()
        {
            GameObject go = FlowchartMenuItems.SpawnPrefab("Character");
            go.transform.position = Vector3.zero;
        }

        [MenuItem("Tools/MmFungus/Fungus 原生/创建对白界面", false, 122)]
        static void CreateSayDialog()
        {
            GameObject go = FlowchartMenuItems.SpawnPrefab("SayDialog");
            go.transform.position = Vector3.zero;
        }

        [MenuItem("Tools/MmFungus/Fungus 原生/创建选项界面", false, 123)]
        static void CreateMenuDialog()
        {
            GameObject go = FlowchartMenuItems.SpawnPrefab("MenuDialog");
            go.transform.position = Vector3.zero;
        }

        [MenuItem("Tools/MmFungus/Fungus 原生/创建立绘舞台", false, 130)]
        static void CreateStage()
        {
            GameObject go = FlowchartMenuItems.SpawnPrefab("Stage");
            go.transform.position = Vector3.zero;
        }
        
        [MenuItem("Tools/MmFungus/Fungus 原生/创建立绘站位", false, 131)]
        static void CreateStagePosition()
        {
            FlowchartMenuItems.SpawnPrefab("StagePosition");
        }

        [MenuItem("Tools/MmFungus/Fungus 原生/创建多语言翻译", false, 150)]
        static void CreateLocalization()
        {
            GameObject go = FlowchartMenuItems.SpawnPrefab("Localization");
            go.transform.position = Vector3.zero;
        }
    }
}

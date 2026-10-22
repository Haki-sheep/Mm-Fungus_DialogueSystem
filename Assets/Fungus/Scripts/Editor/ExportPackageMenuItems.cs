using System.Linq;
using UnityEditor;

namespace Fungus.EditorUtils
{
    public static class ExportPackageMenuItems
    {
        [MenuItem("Tools/MmFungus/导出核心包", false, 40)]
        private static void ExportFungusPackageFull()
        {
            string Path = EditorUtility.SaveFilePanel("导出对话核心包", "", "Mm-Fungus-DialogueSystem", "unitypackage");
            if (Path.Length != 0)
                ExportRelease(Path);
        }

        public static void ExportRelease(string Path)
        {
            var AssetList = AssetDatabase.GetAllAssetPaths().Where(Asset =>
                Asset == "Assets/Fungus" || Asset == "Assets/AmanitaWebStory" ||
                Asset.StartsWith("Assets/Fungus/") ||
                Asset.StartsWith("Assets/AmanitaWebStory/CLI/") ||
                Asset.StartsWith("Assets/AmanitaWebStory/Editor/") ||
                Asset.StartsWith("Assets/AmanitaWebStory/Runtime/") ||
                Asset.StartsWith("Assets/AmanitaWebStory/Web/") ||
                Asset.StartsWith("Assets/AmanitaWebStory/Fonts/")).ToArray();
            AssetDatabase.ExportPackage(AssetList, Path, ExportPackageOptions.Default);
        }
    }
}

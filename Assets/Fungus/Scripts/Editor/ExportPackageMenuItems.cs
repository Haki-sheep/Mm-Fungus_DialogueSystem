using System.Linq;
using UnityEditor;

namespace Fungus.EditorUtils
{
    public static class ExportPackageMenuItems
    {
        [MenuItem("Tools/Fungus/导出核心包（含网页和 CLI）", false, 200)]
        private static void ExportFungusPackageFull()
        {
            string Path = EditorUtility.SaveFilePanel("导出对话核心包", "", "Mm-Fungus-DialogueSystem", "unitypackage");
            if (Path.Length != 0)
                ExportRelease(Path);
        }

        [MenuItem("Tools/Fungus/仅导出 Fungus 原生核心", false, 201)]
        private static void ExportFungusPackageLite()
        {
            string Path = EditorUtility.SaveFilePanel("导出原生核心", "", "Mm-Fungus-Core", "unitypackage");
            if (Path.Length != 0)
                AssetDatabase.ExportPackage("Assets/Fungus", Path, ExportPackageOptions.Recurse);
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

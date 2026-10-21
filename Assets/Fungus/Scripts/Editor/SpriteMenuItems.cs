// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;

namespace Fungus.EditorUtils
{
    public class SpriteMenuItems 
    {
        [MenuItem("Tools/Amanita/创建/可点击精灵", false, 150)]
        static void CreateClickableSprite()
        {
            FlowchartMenuItems.SpawnPrefab("ClickableSprite");
        }

        [MenuItem("Tools/Amanita/创建/可拖动精灵", false, 151)]
        static void CreateDraggableSprite()
        {
            FlowchartMenuItems.SpawnPrefab("DraggableSprite");
        }

        [MenuItem("Tools/Amanita/创建/拖动目标", false, 152)]
        static void CreateDragTargetSprite()
        {
            FlowchartMenuItems.SpawnPrefab("DragTargetSprite");
        }

        [MenuItem("Tools/Amanita/创建/视差精灵", false, 152)]
        static void CreateParallaxSprite()
        {
            FlowchartMenuItems.SpawnPrefab("ParallaxSprite");
        }
    }
}
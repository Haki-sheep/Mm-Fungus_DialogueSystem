using System.Collections.Generic;
using UnityEngine;

namespace AmanitaWebStory
{
    /// <summary> 剧情角色 网页按 Id 引用角色 按立绘 Sprite 名称引用表情 </summary>
    [CreateAssetMenu(fileName = "NewActor", menuName = "剧情/角色", order = 100)]
    public sealed class StoryActor : ScriptableObject
    {
        [SerializeField, Tooltip("英文字母开头 可用数字和下划线 同一工程内唯一")]
        private string actorId;
        [SerializeField]
        private string displayName;
        [SerializeField]
        private Color nameColor = Color.white;
        [SerializeField, Tooltip("同一角色内 Sprite 名称不能重复")]
        private List<Sprite> portraitList = new List<Sprite>();

        public string Id => actorId;
        public string DisplayName => displayName;
        public Color NameColor => nameColor;
        public IReadOnlyList<Sprite> Portraits => portraitList.AsReadOnly();
    }
}

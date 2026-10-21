using System;
using System.Globalization;
using Fungus.Inspector;
using UnityEngine;

namespace AmanitaWebStory
{
    public sealed class StorySignalReceiver : MonoBehaviour
    {
        [LabelText("剧情事件 SO")]
        public StorySignalCatalog Catalog;
        [LabelText("仅接收指定剧情实例"), Tooltip("留空时接收此事件 SO 的所有实例")]
        public WebStoryHost Source;
        [LabelText("事件与响应")]
        public StoryEventBinding Response = new StoryEventBinding();

        private void OnEnable()
        {
            var Definition = Catalog.DefinitionList.Find(Item => Item.Id == Response.Id);
            if (Definition == null || Definition.Kind != Response.Kind)
                throw new InvalidOperationException($"事件接收器 {name} 的标识或参数类型与事件 SO 不一致");
            Catalog.Raised += Receive;
        }

        private void OnDisable()
        {
            Catalog.Raised -= Receive;
        }

        private void Receive(WebStoryHost Sender, string Id, string Value)
        {
            if (Id != Response.Id || Source != null && Sender != Source)
                return;
            switch (Response.Kind)
            {
                case eStoryValueKind.None: Response.OnInvoke.Invoke(); break;
                case eStoryValueKind.String: Response.OnString.Invoke(Value); break;
                case eStoryValueKind.Integer: Response.OnInteger.Invoke(int.Parse(Value, CultureInfo.InvariantCulture)); break;
                case eStoryValueKind.Float: Response.OnFloat.Invoke(float.Parse(Value, CultureInfo.InvariantCulture)); break;
                case eStoryValueKind.Boolean: Response.OnBoolean.Invoke(bool.Parse(Value)); break;
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}

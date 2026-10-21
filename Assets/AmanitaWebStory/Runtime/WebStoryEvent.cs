using Fungus;
using Fungus.Inspector;
using UnityEngine;

namespace AmanitaWebStory
{
    [CommandInfo("网页剧情", "执行剧情事件", "广播网页定义的事件 或调用已接入的 Unity 能力")]
    public sealed class WebStoryEvent : Command
    {
        [SerializeField, LabelText("剧情宿主")]
        private WebStoryHost host;
        [SerializeField, LabelText("已登记事件")]
        private string eventId;
        [SerializeField, LabelText("参数内容")]
        private string value;
        [SerializeField, LabelText("广播网页定义的事件")]
        private bool broadcast;

        public override void OnEnter()
        {
            if (broadcast)
                host.SignalCatalog.Raise(host, eventId, value);
            else
                host.Raise(eventId, value);
            Continue();
        }

        public override string GetSummary()
        {
            return eventId + " · " + value;
        }
    }
}

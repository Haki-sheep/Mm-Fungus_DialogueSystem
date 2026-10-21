using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Calls DontDestroyOnLoad on the target gameobject.
    /// </summary>
    [CommandInfo("脚本与事件", "设置跨场景保留", "对配置的目标执行设置跨场景保留 使用下方参数设置具体行为")]
    [AddComponentMenu("")]
    public class DestroyOnLoad : Command
    {
        [LabelText("目标")]
        [SerializeField]
        protected GameObjectData target;

        public override void OnEnter()
        {
            DontDestroyOnLoad(target.Value);

            Continue();
        }

        public override string GetSummary()
        {
            return target.Value != null ? target.Value.name : "请先设置目标";
        }

        public override bool HasReference(Variable variable)
        {
            return variable == target.gameObjectRef;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }
    }
}
using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// The block will execute when the desired Rendering related message for the monobehaviour is received.
    /// </summary>
    [EventHandlerInfo("组件回调", "渲染回调", "在渲染回调时执行所属流程")]
    [AddComponentMenu("")]
    public class Render : EventHandler
    {

        [System.Flags]
        public enum RenderMessageFlags
        {
            [UnityEngine.InspectorName("渲染后")]
            OnPostRender = 1 << 0,
            [UnityEngine.InspectorName("剔除画面对象前")]
            OnPreCull = 1 << 1,
            [UnityEngine.InspectorName("渲染前")]
            OnPreRender = 1 << 2,
            //OnRenderImage = 1 << 3,
            [UnityEngine.InspectorName("渲染对象时")]
            OnRenderObject = 1 << 4,
            [UnityEngine.InspectorName("即将渲染对象时")]
            OnWillRenderObject = 1 << 5,
            [UnityEngine.InspectorName("对象离开画面")]
            OnBecameInvisible = 1 << 6,
            [UnityEngine.InspectorName("对象进入画面")]
            OnBecameVisible = 1 << 7,
        }

        [Tooltip("Which of the Rendering messages to trigger on.")]
        [SerializeField]
        [EnumFlag]
        [LabelText("触发时机")]
        protected RenderMessageFlags FireOn = RenderMessageFlags.OnWillRenderObject;

        private void OnPostRender()
        {
            if((FireOn & RenderMessageFlags.OnPostRender) != 0)
            {
                ExecuteBlock();
            }
        }

        private void OnPreCull()
        {
            if ((FireOn & RenderMessageFlags.OnPreCull) != 0)
            {
                ExecuteBlock();
            }
        }

        private void OnPreRender()
        {
            if ((FireOn & RenderMessageFlags.OnPreRender) != 0)
            {
                ExecuteBlock();
            }
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            //TODO
        }

        private void OnRenderObject()
        {
            if ((FireOn & RenderMessageFlags.OnRenderObject) != 0)
            {
                ExecuteBlock();
            }
        }

        private void OnWillRenderObject()
        {
            if ((FireOn & RenderMessageFlags.OnWillRenderObject) != 0)
            {
                ExecuteBlock();
            }
        }

        private void OnBecameInvisible()
        {
            if ((FireOn & RenderMessageFlags.OnBecameInvisible) != 0)
            {
                ExecuteBlock();
            }
        }

        private void OnBecameVisible()
        {
            if ((FireOn & RenderMessageFlags.OnBecameVisible) != 0)
            {
                ExecuteBlock();
            }
        }
    }
}

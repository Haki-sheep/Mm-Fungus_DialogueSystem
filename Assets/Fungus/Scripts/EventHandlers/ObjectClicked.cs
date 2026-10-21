using Fungus.Inspector;
using Cysharp.Threading.Tasks;
using System.Threading;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using System.Collections;

namespace Fungus 
{
    /// <summary>
    /// The block will execute when the user clicks or taps on the clickable object.
    /// </summary>
    [EventHandlerInfo("精灵图片", "对象被点击", "在对象被点击时执行所属流程")]
    [AddComponentMenu("")]
    public class ObjectClicked : EventHandler
    {
        private readonly AsyncOperationScope asyncScope = new AsyncOperationScope();
   
        public class ObjectClickedEvent
        {
            public Clickable2D ClickableObject;
            public ObjectClickedEvent(Clickable2D clickableObject)
            {
                ClickableObject = clickableObject;
            }
        }

        [Tooltip("设置本步骤使用的可点击对象")]
        [LabelText("可点击对象")]
        [SerializeField]
        protected Clickable2D clickableObject;

        [Tooltip("设置本步骤使用的等待帧数")]
        [LabelText("等待帧数")]
        [SerializeField]
        protected int waitFrames = 1;

        protected EventDispatcher eventDispatcher;

        protected virtual void OnEnable()
        {
            eventDispatcher = FungusManager.Instance.EventDispatcher;

            eventDispatcher.AddListener<ObjectClickedEvent>(OnObjectClickedEvent);
        }

        protected virtual void OnDisable()
        {
            eventDispatcher.RemoveListener<ObjectClickedEvent>(OnObjectClickedEvent);

            eventDispatcher = null;
        }

        void OnObjectClickedEvent(ObjectClickedEvent evt)
        {
            OnObjectClicked(evt.ClickableObject);
        }

        /// <summary>
        /// Executing a block on the same frame that the object is clicked can cause
        /// input problems (e.g. auto completing Say Dialog text). A single frame delay 
        /// fixes the problem.
        /// </summary>
        protected virtual async UniTask DoExecuteBlockAsync(int numFrames, CancellationToken CancellationToken = default)
        {
            if (numFrames == 0)
            {
                ExecuteBlock();
                return;
            }

            int count = Mathf.Max(waitFrames, 1);
            while (count > 0)
            {
                count--;
                await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, CancellationToken);
            }

            ExecuteBlock();
        }

        #region Public members

        /// <summary>
        /// Called by the Clickable2D object when it is clicked.
        /// </summary>
        public virtual void OnObjectClicked(Clickable2D clickableObject)
        {
            if (clickableObject == this.clickableObject)
            {
                asyncScope.Run(this, Token => DoExecuteBlockAsync(waitFrames, Token)).Forget();
            }
        }

        public override string GetSummary()
        {
            if (clickableObject != null)
            {
                return clickableObject.name;
            }

            return "请先设置可点击对象";
        }

        #endregion
    }
}

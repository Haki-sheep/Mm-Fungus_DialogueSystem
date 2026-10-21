using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// The block will execute when the player starts dragging an object.
    /// </summary>
    [EventHandlerInfo("精灵图片", "开始拖动", "在开始拖动时执行所属流程")]
    [AddComponentMenu("")]
    public class DragStarted : EventHandler, ISerializationCallbackReceiver
    {
        public class DragStartedEvent
        {
            public Draggable2D DraggableObject;

            public DragStartedEvent(Draggable2D draggableObject)
            {
                DraggableObject = draggableObject;
            }
        }

        [VariableProperty(typeof(GameObjectVariable))]
        [LabelText("拖动物体引用")]
        [SerializeField]
        protected GameObjectVariable draggableRef;

        [LabelText("可拖动对象列表")]
        [SerializeField]
        protected List<Draggable2D> draggableObjects;

        [HideInInspector]
        [LabelText("可拖动对象")]
        [SerializeField]
        protected Draggable2D draggableObject;

        protected EventDispatcher eventDispatcher;

        protected virtual void OnEnable()
        {
            eventDispatcher = FungusManager.Instance.EventDispatcher;

            eventDispatcher.AddListener<DragStartedEvent>(OnDragStartedEvent);
        }

        protected virtual void OnDisable()
        {
            eventDispatcher.RemoveListener<DragStartedEvent>(OnDragStartedEvent);

            eventDispatcher = null;
        }

        private void OnDragStartedEvent(DragStartedEvent evt)
        {
            OnDragStarted(evt.DraggableObject);
        }

        #region Compatibility

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            //add any dragableobject already present to list for backwards compatability
            if (draggableObject != null)
            {
                if (!draggableObjects.Contains(draggableObject))
                {
                    draggableObjects.Add(draggableObject);
                }
                draggableObject = null;
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        #endregion Compatibility

        #region Public members

        /// <summary>
        /// Called by the Draggable2D object when the drag starts.
        /// </summary>
        public virtual void OnDragStarted(Draggable2D draggableObject)
        {
            if (draggableObjects.Contains(draggableObject))
            {
                if (draggableRef != null)
                {
                    draggableRef.Value = draggableObject.gameObject;
                }
                ExecuteBlock();
            }
        }

        public override string GetSummary()
        {
            if(draggableObjects.Count(x=> x != null) == 0)
            {
                return "配置错误：请先指定可拖动对象";
            }

            string summary = "可拖动对象：";
            if (this.draggableObjects != null && this.draggableObjects.Count != 0)
            {
                for (int i = 0; i < this.draggableObjects.Count; i++)
                {
                    if (draggableObjects[i] != null)
                    {
                        summary += draggableObjects[i].name + ",";
                    }
                }
            }

            return summary;
        }

        #endregion Public members
    }
}
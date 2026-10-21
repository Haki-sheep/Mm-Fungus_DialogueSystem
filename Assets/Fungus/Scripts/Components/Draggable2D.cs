using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using System.Collections.Generic;
using PrimeTween;

namespace Fungus
{
    /// <summary>
    /// Detects drag and drop interactions on a Game Object, and sends events to all Flowchart event handlers in the scene.
    /// The Game Object must have Collider2D & RigidBody components attached. 
    /// The Collider2D must have the Is Trigger property set to true.
    /// The RigidBody would typically have the Is Kinematic property set to true, unless you want the object to move around using physics.
    /// Use in conjunction with the Drag Started, Drag Completed, Drag Cancelled, Drag Entered & Drag Exited event handlers.
    /// </summary>
    public class Draggable2D : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Tooltip("设置本步骤使用的允许拖动")]
        [LabelText("允许拖动")]
        [SerializeField]
        protected bool dragEnabled = true;

        [Tooltip("设置本步骤使用的取消拖动时返回")]
        [FormerlySerializedAs("returnToStartPos")]
        [LabelText("取消拖动时返回")]
        [SerializeField]
        protected bool returnOnCancelled = true;

        [Tooltip("设置本步骤使用的完成拖动时返回")]
        [LabelText("完成拖动时返回")]
        [SerializeField]
        protected bool returnOnCompleted = true;

        [Tooltip("设置本步骤使用的返回时间（秒）")]
        [LabelText("返回时间（秒）")]
        [SerializeField]
        protected float returnDuration = 1f;

        [Tooltip("设置本步骤使用的悬停时鼠标指针")]
        [LabelText("悬停时鼠标指针")]
        [SerializeField]
        protected Texture2D hoverCursor;

        [Tooltip("设置本步骤使用的使用界面事件系统")]
        [LabelText("使用界面事件系统")]
        [SerializeField]
        protected bool useEventSystem;

        [LabelText("正在被拖动")]
        [SerializeField]
        protected bool beingDragged;

        public virtual bool BeingDragged
        {
            get { return beingDragged; }
            set { beingDragged = value; }
        }

        protected Vector3 startingPosition;
        protected bool updatePosition = false;
        protected Vector3 newPosition;
        protected Vector3 delta = Vector3.zero;

        #region DragCompleted handlers
        protected List<DragCompleted> dragCompletedHandlers = new List<DragCompleted>();

        public void RegisterHandler(DragCompleted handler)
        {
            dragCompletedHandlers.Add(handler);
        }

        public void UnregisterHandler(DragCompleted handler)
        {
            if(dragCompletedHandlers.Contains(handler))
            {
                dragCompletedHandlers.Remove(handler);
            }
        }
        #endregion

        protected virtual void LateUpdate()
        {
            // Position is applied after animation updates so dragging keeps control of the object
            // To make sure this doesn't happen, we force the position change to happen in LateUpdate.
            if (updatePosition)
            {
                transform.position = newPosition;
                updatePosition = false;
            }
        }

        protected virtual void OnTriggerEnter2D(Collider2D other) 
        {
            if (!dragEnabled)
            {
                return;
            }

            var eventDispatcher = FungusManager.Instance.EventDispatcher;

            eventDispatcher.Raise(new DragEntered.DragEnteredEvent(this, other));
        }

        protected virtual void OnTriggerExit2D(Collider2D other) 
        {
            if (!dragEnabled)
            {
                return;
            }

            var eventDispatcher = FungusManager.Instance.EventDispatcher;

            eventDispatcher.Raise(new DragExited.DragExitedEvent(this, other));
        }

        protected virtual void DoBeginDrag()
        {
            beingDragged = true;

            // Offset the object so that the drag is anchored to the exact point where the user clicked it

#if ENABLE_INPUT_SYSTEM
            var mousePos = UnityEngine.InputSystem.Mouse.current?.position.ReadValue() ?? Vector2.zero;
#else
            var mousePos = Input.mousePosition;
#endif
            delta = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f)) - transform.position;
            delta.z = 0f;

            startingPosition = transform.position;

            var eventDispatcher = FungusManager.Instance.EventDispatcher;

            eventDispatcher.Raise(new DragStarted.DragStartedEvent(this));
        }

        protected virtual void DoDrag()
        {
            if (!dragEnabled)
            {
                return;
            }

#if ENABLE_INPUT_SYSTEM
            var mousePos = UnityEngine.InputSystem.Mouse.current?.position.ReadValue() ?? Vector2.zero;
#else
            var mousePos = Input.mousePosition;
#endif
            float x = mousePos.x;
            float y = mousePos.y;
            float z = transform.position.z;

            newPosition = Camera.main.ScreenToWorldPoint(new Vector3(x, y, 10f)) - delta;
            newPosition.z = z;
            updatePosition = true;
        }

        protected virtual void DoEndDrag()
        {
            if (!dragEnabled)
            {
                return;
            }

            var eventDispatcher = FungusManager.Instance.EventDispatcher;
            bool dragCompleted = false;

            for (int i = 0; i < dragCompletedHandlers.Count; i++)
            {
                var handler = dragCompletedHandlers[i];
                if (handler != null && handler.DraggableObjects.Contains(this))
                {
                    if (handler.IsOverTarget())
                    {
                        dragCompleted = true;

                        eventDispatcher.Raise(new DragCompleted.DragCompletedEvent(this));
                    }
                }
            }

            if (!dragCompleted)
            {
                eventDispatcher.Raise(new DragCancelled.DragCancelledEvent(this));

                if (returnOnCancelled)
                {
                    FungusTween.Stop(gameObject);
                    FungusTween.Move(gameObject, startingPosition, returnDuration, eTweenEase.easeOutExpo);
                }
            }
            else if(returnOnCompleted)
            {
                FungusTween.Stop(gameObject);
                FungusTween.Move(gameObject, startingPosition, returnDuration, eTweenEase.easeOutExpo);
            }

            beingDragged = false;
        }

        protected virtual void DoPointerEnter()
        {
            ChangeCursor(hoverCursor);
        }

        protected virtual void DoPointerExit()
        {
            SetMouseCursor.ResetMouseCursor();
        }

        protected virtual void ChangeCursor(Texture2D cursorTexture)
        {
            if (!dragEnabled)
            {
                return;
            }

            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        }

        #region Legacy OnMouseX methods

        protected virtual void OnMouseDown()
        {
            if (!useEventSystem)
            {
                DoBeginDrag();
            }
        }

        protected virtual void OnMouseDrag()
        {
            if (!useEventSystem)
            {
                DoDrag();
            }
        }

        protected virtual void OnMouseUp()
        {
            if (!useEventSystem)
            {
                DoEndDrag();
            }
        }

        protected virtual void OnMouseEnter()
        {
            if (!useEventSystem)
            {
                DoPointerEnter();
            }
        }
        
        protected virtual void OnMouseExit()
        {
            if (!useEventSystem)
            {
                DoPointerExit();
            }
        }

        #endregion

        #region Public members

        /// <summary>
        /// Is object drag and drop enabled.
        /// </summary>
        /// <value><c>true</c> if drag enabled; otherwise, <c>false</c>.</value>
        public virtual bool DragEnabled { get { return dragEnabled; } set { dragEnabled = value; } }

        #endregion

        #region IBeginDragHandler implementation

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (useEventSystem)
            {
                DoBeginDrag();
            }
        }

        #endregion

        #region IDragHandler implementation

        public void OnDrag(PointerEventData eventData)
        {
            if (useEventSystem)
            {
                DoDrag();
            }
        }

        #endregion

        #region IEndDragHandler implementation

        public void OnEndDrag(PointerEventData eventData)
        {
            if (useEventSystem)
            {
                DoEndDrag();
            }
        }

        #endregion

        #region IPointerEnterHandler implementation

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (useEventSystem)
            {
                DoPointerEnter();
            }
        }

        #endregion

        #region IPointerExitHandler implementation

        public void OnPointerExit(PointerEventData eventData)
        {
            if (useEventSystem)
            {
                DoPointerExit();
            }
        }

        #endregion
    }
}

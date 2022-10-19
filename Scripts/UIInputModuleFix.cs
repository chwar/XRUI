// MIT License
// Copyright (c) 2022 Pablo Gutierrez Millares
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UnityEngine.XR.Interaction.Toolkit.UI
{
    /// <summary>
    /// Base class for input modules that send UI input.
    /// </summary>
    /// <remarks>
    /// Multiple input modules may be placed on the same event system. In such a setup,
    /// the modules will synchronize with each other.
    /// </remarks>
    [DefaultExecutionOrder(XRInteractionUpdateOrder.k_UIInputModule)]
    public abstract partial class UIInputModuleFix : BaseInputModule
    {
        [SerializeField, FormerlySerializedAs("clickSpeed")]
        [Tooltip("The maximum time (in seconds) between two mouse presses for it to be consecutive click.")]
        float m_ClickSpeed = 0.3f;

        [FormerlySerializedAs("trackedDeviceDragThresholdMultiplier")]
        [SerializeField, Tooltip("Scales the EventSystem.pixelDragThreshold, for tracked devices, to make selection easier.")]
        float m_TrackedDeviceDragThresholdMultiplier = 2f;

        /// <summary>
        /// The <see cref="Camera"/> that is used to perform 2D raycasts when determining the screen space location of a tracked device cursor.
        /// </summary>
        public Camera uiCamera { get; set; }

        AxisEventData m_CachedAxisEvent;
        PointerEventData m_CachedPointerEvent;
        TrackedDeviceEventData m_CachedTrackedDeviceEventData;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        /// <remarks>
        /// Processing is postponed from earlier in the frame (<see cref="EventSystem"/> has a
        /// script execution order of <c>-1000</c>) until this Update to allow other systems to
        /// update the poses that will be used to generate the raycasts used by this input module.
        /// <br />
        /// For Ray Interactor, it must wait until after the Controller pose updates and Locomotion
        /// moves the Rig in order to generate the current sample points used to create the rays used
        /// for this frame. Those positions will be determined during <see cref="DoProcess"/>.
        /// Ray Interactor needs the UI raycasts to be completed by the time <see cref="XRInteractionManager"/>
        /// calls into <see cref="XRBaseInteractor.GetValidTargets"/> since that is dependent on
        /// whether a UI hit was closer than a 3D hit. This processing must therefore be done
        /// between Locomotion and <see cref="XRBaseInteractor.ProcessInteractor"/> to minimize latency.
        /// </remarks>
        protected virtual void Update()
        {
            // Check to make sure that Process should still be called.
            // It would likely cause unexpected results if processing was done
            // when this module is no longer the current one.
            if (eventSystem.IsActive() && eventSystem.currentInputModule == this && eventSystem == EventSystem.current)
            {
                DoProcess();
            }
        }

        /// <summary>
        /// Process the current tick for the module.
        /// </summary>
        /// <remarks>
        /// Executed once per Update call. Override for custom processing.
        /// </remarks>
        /// <seealso cref="Process"/>
        protected virtual void DoProcess()
        {
            SendUpdateEventToSelectedObject();
        }

        /// <inheritdoc />
        public override void Process()
        {
            // Postpone processing until later in the frame
        }

        /// <summary>
        /// Sends an update event to the currently selected object.
        /// </summary>
        /// <returns>Returns whether the update event was used by the selected object.</returns>
        protected bool SendUpdateEventToSelectedObject()
        {
            var selectedGameObject = eventSystem.currentSelectedGameObject;
            if (selectedGameObject == null)
                return false;

            var data = GetBaseEventData();
            updateSelected?.Invoke(selectedGameObject, data);
            ExecuteEvents.Execute(selectedGameObject, data, ExecuteEvents.updateSelectedHandler);
            return data.used;
        }

        RaycastResult PerformRaycast(PointerEventData eventData)
        {
            if (eventData == null)
                throw new ArgumentNullException(nameof(eventData));

            eventSystem.RaycastAll(eventData, m_RaycastResultCache);
            finalizeRaycastResults?.Invoke(eventData, m_RaycastResultCache);
            var result = FindFirstRaycast(m_RaycastResultCache);
            m_RaycastResultCache.Clear();
            return result;
        }

        void ProcessMouseMovement(PointerEventData eventData)
        {
            var currentPointerTarget = eventData.pointerCurrentRaycast.gameObject;

            foreach (GameObject hovered in eventData.hovered)
                ExecuteEvents.Execute(hovered, eventData, ExecuteEvents.pointerMoveHandler);
            
            // another way of triggering the pointerMoveHandler
            // if (currentPointerTarget != null)
            //     ExecuteEvents.Execute(currentPointerTarget, eventData, ExecuteEvents.pointerMoveHandler);

            // If we have no target or pointerEnter has been deleted,
            // we just send exit events to anything we are tracking
            // and then exit.
            if (currentPointerTarget == null || eventData.pointerEnter == null)
            {
                foreach (var hovered in eventData.hovered)
                {
                    pointerExit?.Invoke(hovered, eventData);
                    ExecuteEvents.Execute(hovered, eventData, ExecuteEvents.pointerExitHandler);
                }

                eventData.hovered.Clear();

                if (currentPointerTarget == null)
                {
                    eventData.pointerEnter = null;
                    return;
                }
            }

            if (eventData.pointerEnter == currentPointerTarget)
                return;

            var commonRoot = FindCommonRoot(eventData.pointerEnter, currentPointerTarget);

            // We walk up the tree until a common root and the last entered and current entered object is found.
            // Then send exit and enter events up to, but not including, the common root.
            if (eventData.pointerEnter != null)
            {
                var target = eventData.pointerEnter.transform;

                while (target != null)
                {
                    if (commonRoot != null && commonRoot.transform == target)
                        break;

                    var targetGameObject = target.gameObject;
                    pointerExit?.Invoke(targetGameObject, eventData);
                    ExecuteEvents.Execute(targetGameObject, eventData, ExecuteEvents.pointerExitHandler);

                    eventData.hovered.Remove(targetGameObject);

                    target = target.parent;
                }
            }

            eventData.pointerEnter = currentPointerTarget;
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse -- Could be null if it was destroyed immediately after executing above
            if (currentPointerTarget != null)
            {
                var target = currentPointerTarget.transform;

                while (target != null && target.gameObject != commonRoot)
                {
                    var targetGameObject = target.gameObject;
                    pointerEnter?.Invoke(targetGameObject, eventData);
                    ExecuteEvents.Execute(targetGameObject, eventData, ExecuteEvents.pointerEnterHandler);

                    eventData.hovered.Add(targetGameObject);

                    target = target.parent;
                }
            }
        }

        void ProcessMouseButton(ButtonDeltaState mouseButtonChanges, PointerEventData eventData)
        {
            var hoverTarget = eventData.pointerCurrentRaycast.gameObject;

            if ((mouseButtonChanges & ButtonDeltaState.Pressed) != 0)
            {
                eventData.eligibleForClick = true;
                eventData.delta = Vector2.zero;
                eventData.dragging = false;
                eventData.pressPosition = eventData.position;
                eventData.pointerPressRaycast = eventData.pointerCurrentRaycast;

                var selectHandler = ExecuteEvents.GetEventHandler<ISelectHandler>(hoverTarget);

                // If we have clicked something new, deselect the old thing
                // and leave 'selection handling' up to the press event.
                if (selectHandler != eventSystem.currentSelectedGameObject)
                    eventSystem.SetSelectedGameObject(null, eventData);

                // search for the control that will receive the press.
                // if we can't find a press handler set the press
                // handler to be what would receive a click.

                pointerDown?.Invoke(hoverTarget, eventData);
                var newPressed = ExecuteEvents.ExecuteHierarchy(hoverTarget, eventData, ExecuteEvents.pointerDownHandler);

                // We didn't find a press handler, so we search for a click handler.
                if (newPressed == null)
                    newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(hoverTarget);

                var time = Time.unscaledTime;

                if (newPressed == eventData.lastPress && ((time - eventData.clickTime) < m_ClickSpeed))
                    ++eventData.clickCount;
                else
                    eventData.clickCount = 1;

                eventData.clickTime = time;

                eventData.pointerPress = newPressed;
                eventData.rawPointerPress = hoverTarget;

                // Save the drag handler for drag events during this mouse down.
                var dragObject = ExecuteEvents.GetEventHandler<IDragHandler>(hoverTarget);
                eventData.pointerDrag = dragObject;

                if (dragObject != null)
                {
                    initializePotentialDrag?.Invoke(dragObject, eventData);
                    ExecuteEvents.Execute(dragObject, eventData, ExecuteEvents.initializePotentialDrag);
                }
            }

            if ((mouseButtonChanges & ButtonDeltaState.Released) != 0)
            {
                var target = eventData.pointerPress;
                pointerUp?.Invoke(target, eventData);
                ExecuteEvents.Execute(target, eventData, ExecuteEvents.pointerUpHandler);

                var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(hoverTarget);
                var pointerDrag = eventData.pointerDrag;
                if (target == pointerUpHandler && eventData.eligibleForClick)
                {
                    pointerClick?.Invoke(target, eventData);
                    ExecuteEvents.Execute(target, eventData, ExecuteEvents.pointerClickHandler);
                }
                else if (eventData.dragging && pointerDrag != null)
                {
                    drop?.Invoke(hoverTarget, eventData);
                    ExecuteEvents.ExecuteHierarchy(hoverTarget, eventData, ExecuteEvents.dropHandler);

                    endDrag?.Invoke(pointerDrag, eventData);
                    ExecuteEvents.Execute(pointerDrag, eventData, ExecuteEvents.endDragHandler);
                }

                eventData.eligibleForClick = eventData.dragging = false;
                eventData.pointerPress = eventData.rawPointerPress = eventData.pointerDrag = null;
            }
        }

        void ProcessMouseButtonDrag(PointerEventData eventData, float pixelDragThresholdMultiplier = 1.0f)
        {
            if (!eventData.IsPointerMoving() ||
                Cursor.lockState == CursorLockMode.Locked ||
                eventData.pointerDrag == null)
            {
                return;
            }

            if (!eventData.dragging)
            {
                if ((eventData.pressPosition - eventData.position).sqrMagnitude >= ((eventSystem.pixelDragThreshold * eventSystem.pixelDragThreshold) * pixelDragThresholdMultiplier))
                {
                    var target = eventData.pointerDrag;
                    beginDrag?.Invoke(target, eventData);
                    ExecuteEvents.Execute(target, eventData, ExecuteEvents.beginDragHandler);
                    eventData.dragging = true;
                }
            }

            if (eventData.dragging)
            {
                // If we moved from our initial press object, process an up for that object.
                var target = eventData.pointerPress;
                if (target != eventData.pointerDrag)
                {
                    pointerUp?.Invoke(target, eventData);
                    ExecuteEvents.Execute(target, eventData, ExecuteEvents.pointerUpHandler);

                    eventData.eligibleForClick = false;
                    eventData.pointerPress = null;
                    eventData.rawPointerPress = null;
                }

                drag?.Invoke(eventData.pointerDrag, eventData);
                ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.dragHandler);
            }
        }

        void ProcessMouseScroll(PointerEventData eventData)
        {
            var scrollDelta = eventData.scrollDelta;
            if (!Mathf.Approximately(scrollDelta.sqrMagnitude, 0f))
            {
                var scrollHandler = ExecuteEvents.GetEventHandler<IScrollHandler>(eventData.pointerEnter);
                scroll?.Invoke(scrollHandler, eventData);
                ExecuteEvents.ExecuteHierarchy(scrollHandler, eventData, ExecuteEvents.scrollHandler);
            }
        }

        internal void ProcessTrackedDevice(ref TrackedDeviceModel deviceState, bool force = false)
        {
            if (!deviceState.changedThisFrame && !force)
                return;

            var eventData = GetOrCreateCachedTrackedDeviceEvent();
            eventData.Reset();
            deviceState.CopyTo(eventData);

            eventData.button = PointerEventData.InputButton.Left;

            // Demolish the screen position so we don't trigger any hits from a GraphicRaycaster component on a Canvas.
            // The position value is not used by the TrackedDeviceGraphicRaycaster.
            // Restore the original value after the Raycast is complete.
            var savedPosition = eventData.position;
            // eventData.position = new Vector2(float.MinValue, float.MinValue);
            eventData.pointerCurrentRaycast = PerformRaycast(eventData);
            eventData.position = savedPosition;

            // Get associated camera, or main-tagged camera, or camera from raycast, and if *nothing* exists, then abort processing this frame.
            // ReSharper disable once LocalVariableHidesMember
            var camera = uiCamera != null ? uiCamera : (uiCamera = Camera.main);
            if (camera == null)
            {
                var module = eventData.pointerCurrentRaycast.module;
                if (module != null)
                    camera = module.eventCamera;
            }

            if (camera != null)
            {
                Vector2 screenPosition;
                if (eventData.pointerCurrentRaycast.isValid)
                {
                    screenPosition = camera.WorldToScreenPoint(eventData.pointerCurrentRaycast.worldPosition);
                }
                else
                {
                    var endPosition = eventData.rayPoints.Count > 0 ? eventData.rayPoints[eventData.rayPoints.Count - 1] : Vector3.zero;
                    screenPosition = camera.WorldToScreenPoint(endPosition);
                    eventData.position = screenPosition;
                }

                var thisFrameDelta = screenPosition - eventData.position;
                eventData.position = screenPosition;
                eventData.delta = thisFrameDelta;

                ProcessMouseButton(deviceState.selectDelta, eventData);
                ProcessMouseMovement(eventData);
                ProcessMouseScroll(eventData);
                ProcessMouseButtonDrag(eventData, m_TrackedDeviceDragThresholdMultiplier);

                deviceState.CopyFrom(eventData);
            }

            deviceState.OnFrameFinished();
        }

        TrackedDeviceEventData GetOrCreateCachedTrackedDeviceEvent()
        {
            var result = m_CachedTrackedDeviceEventData;
            if (result == null)
            {
                result = new TrackedDeviceEventData(eventSystem);
                m_CachedTrackedDeviceEventData = result;
            }

            return result;
        }
    }
        public abstract partial class UIInputModuleFix
    {
        /// <summary>
        /// Calls the methods in its invocation list after the input module collects a list of type <see cref="RaycastResult"/>, but before the results are used.
        /// Note that not all fields of the event data are still valid or up to date at this point in the UI event processing.
        /// This event can be used to read, modify, or reorder results.
        /// After the event, the first result in the list with a non-null GameObject will be used.
        /// </summary>
        public event Action<PointerEventData, List<RaycastResult>> finalizeRaycastResults;

        /// <summary>
        /// This occurs when a UI pointer enters an element.
        /// </summary>
        public event Action<GameObject, PointerEventData> pointerEnter;

        /// <summary>
        /// This occurs when a UI pointer exits an element.
        /// </summary>
        public event Action<GameObject, PointerEventData> pointerExit;

        /// <summary>
        /// This occurs when a select button down occurs while a UI pointer is hovering an element.
        /// This event is executed using ExecuteEvents.ExecuteHierarchy when sent to the target element.
        /// </summary>
        public event Action<GameObject, PointerEventData> pointerDown;

        /// <summary>
        /// This occurs when a select button up occurs while a UI pointer is hovering an element.
        /// </summary>
        public event Action<GameObject, PointerEventData> pointerUp;

        /// <summary>
        /// This occurs when a select button click occurs while a UI pointer is hovering an element.
        /// </summary>
        public event Action<GameObject, PointerEventData> pointerClick;

        /// <summary>
        /// This occurs when a potential drag occurs on an element.
        /// </summary>
        public event Action<GameObject, PointerEventData> initializePotentialDrag;

        /// <summary>
        /// This occurs when a drag first occurs on an element.
        /// </summary>
        public event Action<GameObject, PointerEventData> beginDrag;

        /// <summary>
        /// This occurs every frame while dragging an element.
        /// </summary>
        public event Action<GameObject, PointerEventData> drag;

        /// <summary>
        /// This occurs on the last frame an element is dragged.
        /// </summary>
        public event Action<GameObject, PointerEventData> endDrag;

        /// <summary>
        /// This occurs when a dragged element is dropped on a drop handler.
        /// </summary>
        public event Action<GameObject, PointerEventData> drop;

        /// <summary>
        /// This occurs when an element is scrolled
        /// This event is executed using ExecuteEvents.ExecuteHierarchy when sent to the target element.
        /// </summary>
        public event Action<GameObject, PointerEventData> scroll;

        /// <summary>
        /// This occurs on update for the currently selected object.
        /// </summary>
        public event Action<GameObject, BaseEventData> updateSelected;

        /// <summary>
        /// This occurs when the move axis is activated.
        /// </summary>
        public event Action<GameObject, AxisEventData> move;

        /// <summary>
        /// This occurs when the submit button is pressed.
        /// </summary>
        public event Action<GameObject, BaseEventData> submit;

        /// <summary>
        /// This occurs when the cancel button is pressed.
        /// </summary>
        public event Action<GameObject, BaseEventData> cancel;
    }
}

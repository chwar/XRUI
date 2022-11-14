// MIT License
// Copyright (c) 2021 Chris Warin
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.chwar.xrui.UIElements
{
    [ExecuteAlways]
    public class XRUIElement : MonoBehaviour
    {
        public bool PointerOverUI { get; private set; }
        public WorldUIParameters worldUIParameters;
        
        internal UIDocument UIDocument;
        internal VisualElement RootElement;
        private DeviceOrientation _previousOrientation;
        private bool _hasOrientationChanged;
        
        protected internal virtual void Init()
        {
            // To override
        }

        public void Awake()
        {
            UIDocument = GetComponent<UIDocument>();
            RootElement = UIDocument.rootVisualElement.Q(null, "xrui");
            _previousOrientation = Input.deviceOrientation;
            
            // The element is not initialized at this moment, but only during the app's lifetime if it has been re-enabled.
            // During the initial run, all XRUIElement are initialized by the XRUI Instance to make sure that the instance is running first.
            if (XRUI.Instance is null) return;

            Init();
            UpdateUI();
        }

        protected void OnEnable()
        {
            if (RootElement is null) return;
            if (RootElement.childCount == 0)
            {
                throw new NullReferenceException($"The root visual element is empty! " +
                                                 $"You must provide a VisualTreeElement to the UIDocument");
            }
            // Register event handlers for pointer clicks on the UI
            RootElement.RegisterCallback<PointerEnterEvent>(OnPointerEnter);
            RootElement.RegisterCallback<PointerLeaveEvent>(OnPointerLeave);

            // Init();
            UpdateUI();
        }

        protected virtual void OnDisable()
        {
            // When Destroying, Visual Element is null, and no need to unregister the callbacks
            if (RootElement is null) return;
            
            // Unregister event handlers for pointer clicks on the UI
            RootElement.UnregisterCallback<PointerEnterEvent>(OnPointerEnter);
            RootElement.UnregisterCallback<PointerLeaveEvent>(OnPointerLeave);
        }

        private void Update()
        {
            if (!Input.deviceOrientation.Equals(_previousOrientation))
                StartCoroutine(UpdateUIOnRotation());
        }
        
        /*
         * XR UI Methods
         */
        
        /// <summary>
        /// Changes the visibility of the UIDocument with the USS `display` property.
        /// </summary>
        /// <param name="bShow">Visibility value, sets USS to `Flex` or `None`.</param>
        public void Show(bool bShow)
        {
            Show(RootElement, bShow);
        }

        /// <summary>
        /// Changes the visibility of the UIDocument with the USS `display` property.
        /// </summary>
        /// <param name="element">Element to change the visibility of</param>
        /// <param name="bShow">Visibility value, sets USS to `Flex` or `None`.</param>
        public void Show(VisualElement element, bool bShow)
        {
            element.style.display = bShow ? DisplayStyle.Flex : DisplayStyle.None;
            // Hide the panel if in 3D
            if (XRUI.IsCurrentXRUIFormat(XRUI.XRUIFormat.ThreeDimensional))
                GetComponent<MeshRenderer>().enabled = bShow;
        }

        /// <summary>
        /// Adds a UI Element as a child of given parent.
        /// </summary>
        /// <param name="uiParent"></param>
        /// <param name="uiElement"></param>
        /// <exception cref="ArgumentException"></exception>
        public void AddUIElement(VisualElement uiElement, string uiParent)
        {
            VisualElement parent = RootElement.Q(uiParent);
            if (parent != null)
            {
                parent.Add(uiElement);
            }
            else
            {
                throw new ArgumentException(
                    $"There is no Visual Element matching the name \"{uiParent}\" to attach something to");
            }
        }

        /// <summary>
        /// Removes an UIElement
        /// </summary>
        /// <param name="uiElement"></param>
        public void RemoveUIElement(VisualElement uiElement)
        {
            uiElement.RemoveFromHierarchy();
        }
        
        /// <summary>
        /// Method to override in order to update specific UI
        /// </summary>
        internal virtual void UpdateUI()
        {
            if (RootElement is null)
            {
                // If a custom element was added without the .xrui class, we can't find it at runtime
                if(Application.isPlaying)
                    Debug.LogWarning($"The .xrui USS class was not found in the provided visual " +
                                     $"element ({UIDocument.name}). Please add it to the root element and try again.");
                return;
            }

            if (XRUI.IsCurrentXRUIFormat(XRUI.XRUIFormat.TwoDimensional))
            {
                // Check for device orientation to refine Mobile AR USS styles
                bool isLandscape = Input.deviceOrientation == DeviceOrientation.LandscapeLeft 
                                   || Input.deviceOrientation == DeviceOrientation.LandscapeRight 
                                   || (Application.isEditor && XRUI.Instance != null && !XRUI.Instance.setTwoDimensionalFormatToPortraitInEditor);

                RootElement.EnableInClassList("landscape", isLandscape);
                RootElement.EnableInClassList("portrait", !isLandscape);
            }
            else if (XRUI.IsCurrentXRUIFormat(XRUI.XRUIFormat.ThreeDimensional) && Application.isPlaying)
            {
                // Create a runtime VR panel after the layout pass
                RootElement.RegisterCallback<GeometryChangedEvent, UIDocument>(XRUI.GetWorldUIPanel, UIDocument);
            }

            RootElement.EnableInClassList(XRUI.GetCurrentXRUIFormat(), true);
        }
        
        /// <summary>
        /// Updates the UIDocument when the device's rotation changed
        /// </summary>
        /// <returns></returns>
        private IEnumerator UpdateUIOnRotation()
        {
            _previousOrientation = Input.deviceOrientation;
            yield return new WaitForSeconds(0.75f);
            UpdateUI();
        }

        /// <summary>
        /// Callback fired when a pointer is hovering the UI element
        /// </summary>
        /// <param name="evt">The pointer event</param>
        protected void OnPointerEnter(PointerEnterEvent evt)
        {
            PointerOverUI = true;
        }
        
        /// <summary>
        /// Callback fired when a pointer is leaving the UI element
        /// </summary>
        /// <param name="evt">The pointer event</param>
        protected void OnPointerLeave(PointerLeaveEvent evt)
        {
            PointerOverUI = false;
        }
    }

    [Serializable]
    public struct WorldUIParameters
    {
        [Tooltip("Alters the VR panel by slightly bending it")]
        public bool bendPanel;
        [Tooltip("Defines if the VR Panel will be anchored to the camera or not")]
        public bool anchorPanelToCamera;
        [Tooltip("By default, XRUI uses the ratio of the element's dimensions defined in the USS. You can define a custom size here in Unity units here.")]
        public Vector2 customPanelDimensions;
        [Tooltip("By default, the VR panel will be positioned at (0,0,1). You can define a custom position here.")]
        public Vector3 customPanelPosition;
        [Tooltip("Alters the scale of the VR Panel in the virtual world. This parameter is overridden if custom dimensions are specified")]
        public int PanelScale;
    }
}
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.chwar.xrui.UIElements
{
    [ExecuteAlways]
    public class XRUIElement : MonoBehaviour
    {
        public bool PointerOverUI { get; private set; }
        public VRParameters vrParameters;
        
        internal UIDocument UIDocument;
        private DeviceOrientation _previousOrientation;
        private bool _hasOrientationChanged;
        private bool _isInitialized;
        
        protected internal virtual void Init()
        {
            // To override
            UIDocument = GetComponent<UIDocument>();
            _previousOrientation = Input.deviceOrientation;
            _isInitialized = true;
        }

        public void OnValidate()
        {
            if (UIDocument is null)
            {
                if(!Application.isPlaying)
                    Init();
                else
                    return;
            } else if(UIDocument.rootVisualElement is null)
                return;
            UpdateUI();
        }

        protected void OnEnable()
        {
            if (UIDocument is null || UIDocument.rootVisualElement is null) return;
            if (UIDocument.rootVisualElement.childCount == 0)
            {
                throw new NullReferenceException($"The root visual element is empty! " +
                                                 $"You must provide a VisualTreeElement to the UIDocument");
            }
            // Register event handlers for pointer clicks on the UI
            UIDocument.rootVisualElement.ElementAt(0).RegisterCallback<PointerEnterEvent>(OnPointerEnter);
            UIDocument.rootVisualElement.ElementAt(0).RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
            
            // This does not run during the initial run, only during the app's lifetime if an element has been re-enabled.
            // During the initial run, all XRUIElement are initialized by the XRUI Instance to make sure that the instance is running first.
            if (!_isInitialized)
            {
                Init();
                UpdateUI();
            }
        }

        protected virtual void OnDisable()
        {
            _isInitialized = false;
            if (UIDocument is null || UIDocument.rootVisualElement is null) return;
            // Unregister event handlers for pointer clicks on the UI
            UIDocument.rootVisualElement.ElementAt(0).UnregisterCallback<PointerEnterEvent>(OnPointerEnter);
            UIDocument.rootVisualElement.ElementAt(0).UnregisterCallback<PointerLeaveEvent>(OnPointerLeave);
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
            Show(UIDocument.rootVisualElement, bShow);
        }

        /// <summary>
        /// Changes the visibility of the UIDocument with the USS `display` property.
        /// </summary>
        /// <param name="element">Element to change the visibility of</param>
        /// <param name="bShow">Visibility value, sets USS to `Flex` or `None`.</param>
        public void Show(VisualElement element, bool bShow)
        {
            element.style.display = bShow ? DisplayStyle.Flex : DisplayStyle.None;
        }

        /// <summary>
        /// Adds a UI Element as a child of given parent.
        /// </summary>
        /// <param name="uiParent"></param>
        /// <param name="uiElement"></param>
        /// <exception cref="ArgumentException"></exception>
        public void AddUIElement(VisualElement uiElement, string uiParent)
        {
            VisualElement parent = UIDocument.rootVisualElement.Q(uiParent);
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
            XRUI.UpdateDocumentUI(UIDocument);
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
            Debug.Log($"Pointer entered. Element: {(evt.target as VisualElement)?.name }");
        }
        
        /// <summary>
        /// Callback fired when a pointer is leaving the UI element
        /// </summary>
        /// <param name="evt">The pointer event</param>
        protected void OnPointerLeave(PointerLeaveEvent evt)
        {
            PointerOverUI = false;
            Debug.Log($"Pointer left. Element: {(evt.target as VisualElement)?.name }");
        }
    }

    [Serializable]
    public struct VRParameters
    {
        [Tooltip("Alters the VR panel by slightly bending it")]
        public bool bendVRPanel;
        [Tooltip("Defines if the VR Panel will be anchored to the camera or not")]
        public bool anchorVRPanelToCamera;
        [Tooltip("By default, XRUI uses the ratio of the element's dimensions defined in the USS. You can define a custom size here in Unity units here.")]
        public Vector2 customVRPanelDimensions;
        [Tooltip("By default, the VR panel will be automatically anchored in front of the camera. You can define a custom position here.")]
        public Vector3 customVRPanelAnchorPosition;
    }
}
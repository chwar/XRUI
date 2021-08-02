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
        
        protected UIDocument UIDocument;
        
        private DeviceOrientation _previousOrientation;
        private bool _hasOrientationChanged;
        
        /*
         * Unity Events
         */
        
        public virtual void Awake()
        {
            UIDocument = GetComponent<UIDocument>();
            _previousOrientation = Input.deviceOrientation;
        }

        protected virtual void Init()
        {
            // To override
            UpdateUI();
        }

        public virtual void OnValidate()
        {
            if (UIDocument is null || UIDocument.rootVisualElement is null) return;
            UpdateUI();
        }

        protected virtual void OnEnable()
        {
            if (UIDocument is null || UIDocument.rootVisualElement is null) return;
            // Register event handlers for pointer clicks on the UI
            UIDocument.rootVisualElement.ElementAt(0).RegisterCallback<PointerEnterEvent>(OnPointerEnter);
            UIDocument.rootVisualElement.ElementAt(0).RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
            Init();
        }

        protected virtual void OnDisable()
        {
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
            //UIDocument.rootVisualElement.Remove(uiElement);
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
}
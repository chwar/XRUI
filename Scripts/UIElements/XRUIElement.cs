// MIT License
// Copyright (c) 2021 Chris Warin
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
using System;
using System.Collections;
using UnityEditor;
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
        public VisualElement RootElement;

        private DeviceOrientation _cachedDeviceOrientation;
        
        protected internal virtual void Init()
        {
            // To override
            UIDocument = GetComponent<UIDocument>();
            RootElement = UIDocument.rootVisualElement.Q(null, "xrui");
            _cachedDeviceOrientation = Input.deviceOrientation;
        }

        public void Awake()
        {
            // The element is not initialized at this moment, but only during the app's lifetime if it has been re-enabled.
            // During the initial run, all XRUIElement are initialized by the XRUI Instance to make sure that the instance is running first.
            if (XRUI.Instance is null) return;

            Init();
            UpdateUI();
        }

        #if UNITY_EDITOR
        private void OnValidate()
        {
            // This method is only used for updating the elements in the Editor
            if(EditorApplication.isPlayingOrWillChangePlaymode || !gameObject.activeSelf) return;
            Init();
            UpdateUI();
        }
        #endif
        
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

            UpdateUI();
            if (worldUIParameters.anchorPanelToCamera)
                StartCoroutine(FollowCamera());
        }

        protected virtual void OnDisable()
        {
            // When Destroying, Visual Element is null, and no need to unregister the callbacks
            if (RootElement is null) return;
            
            // Unregister event handlers for pointer clicks on the UI
            RootElement.UnregisterCallback<PointerEnterEvent>(OnPointerEnter);
            RootElement.UnregisterCallback<PointerLeaveEvent>(OnPointerLeave);
            
            if (worldUIParameters.anchorPanelToCamera)
                StopCoroutine(FollowCamera());
        }

        private void Update()
        {
            if (DeviceAutoRotationIsOn() && !Input.deviceOrientation.Equals(_cachedDeviceOrientation))
                StartCoroutine(UpdateUIOnRotation());
        }
        
        /*
         * XRUI Methods
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
        /// For World UI, the MeshRenderer and MeshCollider are enabled/disabled.
        /// </summary>
        /// <param name="element">Element to change the visibility of</param>
        /// <param name="bShow">Visibility value, sets USS to `Flex` or `None`.</param>
        public void Show(VisualElement element, bool bShow)
        {
            element.style.display = bShow ? DisplayStyle.Flex : DisplayStyle.None;
            // Hide the panel if in 3D
            if (XRUI.IsCurrentXRUIFormat(XRUI.XRUIFormat.ThreeDimensional) && Application.isPlaying)
            {
                GetComponent<MeshRenderer>().enabled = bShow;
                GetComponent<MeshCollider>().enabled = bShow;
            }
        }

        /// <summary>
        /// Adds a UI Element as a child of given parent.
        /// </summary>
        /// <param name="uiParentClass"></param>
        /// <param name="uiElement"></param>
        /// <exception cref="ArgumentException"></exception>
        public void AddUIElement(VisualElement uiElement, string uiParentClass)
        {
            VisualElement parent = RootElement.Q(null, uiParentClass);
            if (parent != null)
            {
                parent.Add(uiElement);
            }
            else
            {
                throw new ArgumentException(
                    $"There is no Visual Element matching the USS class \"{uiParentClass}\" to attach something to");
            }
        }

        /// <summary>
        /// Removes a UIElement
        /// </summary>
        /// <param name="uiElement"></param>
        public void RemoveUIElement(VisualElement uiElement)
        {
            uiElement.RemoveFromHierarchy();
        }

        /// <summary>
        /// Returns a Visual Element from the Visual Tree Asset. 
        /// </summary>
        /// <param name="xruiClass">The name of the XRUI USS class that matches the element to get.</param>
        /// <typeparam name="T">The type of the Visual Element</typeparam>
        /// <returns>The wanted XRUI Visual Element</returns>
        public T GetXRUIVisualElement<T>(string xruiClass) where T : VisualElement
        {
            return RootElement?.Q<T>(null, xruiClass);
        }
        
        /// <summary>
        /// Returns a Visual Element from the Visual Tree Asset. 
        /// </summary>
        /// <param name="xruiClass">The name of the XRUI USS class that matches the element to get.</param>
        /// <returns>The wanted XRUI Visual Element</returns>
        public VisualElement GetXRUIVisualElement(string xruiClass) 
        {
            return RootElement?.Q(null, xruiClass);
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

            RootElement.RemoveFromClassList(XRUI.XRUIFormat.TwoDimensional.ToString().ToLower());
            RootElement.RemoveFromClassList(XRUI.XRUIFormat.ThreeDimensional.ToString().ToLower());
            RootElement.EnableInClassList(XRUI.GetCurrentXRUIFormat(), true);

            if (XRUI.IsCurrentXRUIFormat(XRUI.XRUIFormat.TwoDimensional))
            {
                // Check for device orientation to refine Mobile AR USS styles
                bool isLandscape = Input.deviceOrientation == DeviceOrientation.LandscapeLeft 
                                   || Input.deviceOrientation == DeviceOrientation.LandscapeRight 
                                   || (Application.isEditor && !Convert.ToBoolean(PlayerPrefs.GetInt("XRUIFormatOrientationPortrait")));

                RootElement.EnableInClassList("landscape", isLandscape);
                RootElement.EnableInClassList("portrait", !isLandscape);
                Show(true);
            }
            else if (XRUI.IsCurrentXRUIFormat(XRUI.XRUIFormat.ThreeDimensional))
            {
                if (Application.isPlaying)
                {
                    // Create new PanelSettings to which we will assign a specific RenderTexture
                    // Since assigning a render texture to PanelSettings removes the linked VisualElement from the original panel's hierarchy,
                    // we need to do this before the layout pass to prevent the old PanelSettings from keeping an incorrect index of its children nodes
                    UIDocument.panelSettings = Instantiate(Resources.Load<PanelSettings>("DefaultWorldUIPanelSettings"));
                    // Create a world UI panel after the layout pass
                    RootElement.RegisterCallback<GeometryChangedEvent, UIDocument>(XRUI.GetWorldUIPanel, UIDocument);
                }
                else
                {
                    Show(false);
                }
            }
        }
        
        /// <summary>
        /// Updates the UIDocument when the device's rotation changed
        /// </summary>
        /// <returns></returns>
        private IEnumerator UpdateUIOnRotation()
        {
            _cachedDeviceOrientation = Input.deviceOrientation;
            yield return new WaitForSeconds(0.5f);
            UpdateUI();
        }

        /// <summary>
        /// Makes World UI lazily follow the camera when it gets too far away from the panel
        /// </summary>
        /// <returns></returns>
        protected IEnumerator FollowCamera()
        {
            while (true)
            {
                // Reposition in front of camera
                if (Camera.main != null)
                {
                    var cachedTarget = Camera.main.transform.TransformPoint(new Vector3(0, 0, .5f));
                    // Slightly delay the following mechanism to let the camera move freely without aggressively repositioning the panel
                    yield return new WaitForSeconds(.5f);
                    // If camera gets too far from the panel
                    if (Vector3.Distance(transform.position, cachedTarget) > worldUIParameters.cameraFollowThreshold)
                    {
                        var cameraFollowVelocity = Vector3.zero;
                        // Reposition until panel reaches the front of the camera
                        while (Vector3.Distance(transform.position, cachedTarget) > .05f)
                        {
                            transform.position = Vector3.SmoothDamp(transform.position, cachedTarget, ref cameraFollowVelocity, .3f);
                            transform.LookAt(2 * transform.position - Camera.main.transform.position);
                            yield return new WaitForEndOfFrame();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Makes World UI fade in/out
        /// </summary>
        /// <param name="bFadeOut">Whether to make the panel fade out</param>
        /// <returns></returns>
        protected IEnumerator FadeWorldPanel(bool bFadeOut = true)
        {
            // Disable collider if fading out
            GetComponent<MeshCollider>().enabled = !bFadeOut;
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            Color color = meshRenderer.materials[0].color;

            while (bFadeOut ? color.a > 0 : color.a < 1)
            {
                if (!bFadeOut) color.a = 0;
                color.a += bFadeOut? -.05f : .05f;
                meshRenderer.materials[0].color = color;
                yield return new WaitForEndOfFrame();
            }
            // End when alpha reaches 0 or 1
            yield return new WaitUntil(() => bFadeOut ? meshRenderer.materials[0].color.a <= 0f : meshRenderer.materials[0].color.a >= 1f);
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
        
        private bool DeviceAutoRotationIsOn()
        {
            // Thanks to swifter14: https://forum.unity.com/threads/lock-auto-rotation-on-android-doesnt-work.842893/
            #if UNITY_ANDROID && !UNITY_EDITOR
                using (var actClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    var context = actClass.GetStatic<AndroidJavaObject>("currentActivity");
                    AndroidJavaClass systemGlobal = new AndroidJavaClass("android.provider.Settings$System");
                    var rotationOn = systemGlobal.CallStatic<int>("getInt", context.Call<AndroidJavaObject>("getContentResolver"), "accelerometer_rotation");
                    return rotationOn == 1;
                }
            #endif
            return true;
        }

        /// <summary>
        /// Helper to activate Camera following when the 3D panel is first created
        /// </summary>
        internal void StartFollowingCamera()
        {
            StartCoroutine(FollowCamera());
        }
    }

    [Serializable]
    public struct WorldUIParameters
    {
        [Tooltip("Alters the VR panel geometry by slightly bending it")]
        public bool bendPanel;
        [Tooltip("Defines if the VR Panel will be anchored to the camera or not")]
        public bool anchorPanelToCamera;
        [Tooltip("Defines the distance that the camera needs to travel away from the panel before the panel starts following it.")]
        public float cameraFollowThreshold;
        [Tooltip("By default, XRUI uses the ratio of the element's dimensions defined in the USS. You can define a custom size here in Unity units here.")]
        public Vector2 customPanelDimensions;
        [Tooltip("By default, the VR panel will be positioned at (0,0,1). You can define a custom position here.")]
        public Vector3 customPanelPosition;
        [Tooltip("Alters the scale of the VR Panel in the virtual world. This parameter is overridden if custom dimensions are specified")]
        public float panelScale;
    }
}
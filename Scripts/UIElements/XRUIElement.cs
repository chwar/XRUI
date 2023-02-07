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
    /// <summary>
    /// XRUI Element class. Inherited by all XRUI elements.
    /// </summary>
    [ExecuteAlways]
    public class XRUIElement : MonoBehaviour
    {
        /// <summary>
        /// True if the pointer is hovering the current element.
        /// </summary>
        public bool PointerOverUI { get; private set; }
        /// <summary>
        /// Set of <see cref="worldUIParameters"/> that define the behaviour of the UI when rendered in world space.
        /// </summary>
        public WorldUIParameters worldUIParameters;
        /// <summary>
        /// <see cref="UIDocument"/> of the element.
        /// </summary>
        internal UIDocument UIDocument;
        /// <summary>
        /// Root <see cref="VisualElement"/> that contains the `.xrui` USS class.
        /// </summary>
        public VisualElement RootElement;
        /// <summary>
        /// Last cached orientation of the device. Used for updating UI when a rotation is detected on smartphones/tablets.
        /// </summary>
        private DeviceOrientation _cachedDeviceOrientation;
        
        /// <summary>
        /// Initializes the UI Element. 
        /// </summary>
        protected internal virtual void Init()
        {
            // To override
            UIDocument = GetComponent<UIDocument>();
            RootElement = UIDocument.rootVisualElement.Q(null, "xrui");
            _cachedDeviceOrientation = Input.deviceOrientation;
        }

        /// <summary>
        /// Unity method. Checks if the <see cref="XRUI"/> instance is running yet before initialising elements. 
        /// </summary>
        public void Awake()
        {
            // The element is not initialized at this moment, but only during the app's lifetime if it has been re-enabled.
            // During the initial run, all XRUIElement are initialized by the XRUI Instance to make sure that the instance is running first.
            if (XRUI.Instance is null) return;

            Init();
            UpdateUI();
        }

        /// <summary>
        /// Unity Editor method. Updates UI when scripts are reloaded.
        /// </summary>
        #if UNITY_EDITOR
        private void OnValidate()
        {
            // This method is only used for updating the elements in the Editor
            if(EditorApplication.isPlayingOrWillChangePlaymode || !gameObject.activeSelf) return;
            Init();
            UpdateUI();
        }
        #endif
        
        /// <summary>
        /// Unity method. Subscribes UI element to <see cref="OnPointerEnter"/> and <see cref="OnPointerLeave"/>.
        /// </summary>
        /// <exception cref="NullReferenceException">Fired when <see cref="RootElement"/> has no children.</exception>
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

        /// <summary>
        /// Unity method. Unsubscribes UI element from <see cref="OnPointerEnter"/> and <see cref="OnPointerLeave"/>.
        /// </summary>
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

        /// <summary>
        /// Unity method. Checks for device rotation.
        /// </summary>
        private void Update()
        {
            if (!Input.deviceOrientation.Equals(_cachedDeviceOrientation))
            {
                bool allowedToRotate = true;
                // For Android, also check if the OS rotation lock is on.
                if (Application.platform == RuntimePlatform.Android) 
                    allowedToRotate = IsAndroidAutoRotateOn();
                if (allowedToRotate &&
                    Screen.autorotateToLandscapeLeft && Screen.autorotateToLandscapeRight &&
                    Screen.autorotateToPortrait && Screen.autorotateToPortraitUpsideDown)
                {
                    StartCoroutine(UpdateUIOnRotation());
                }
            }
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
            // If trying to hide the Root Element, hide the panel if in 3D
            if (XRUI.IsCurrentXRUIFormat(XRUI.XRUIFormat.ThreeDimensional) && element.Equals(RootElement) && Application.isPlaying)
            {
                var mr = GetComponent<MeshRenderer>();
                if(mr != null) mr.enabled = bShow;
                var mc = GetComponent<MeshCollider>();
                if(mc != null) mc.enabled = bShow;
            }
        }

        /// <summary>
        /// Adds a UI Element as a child of given parent.
        /// </summary>
        /// <param name="uiParentClass">The USS class of the container in which to append the element.</param>
        /// <param name="uiElement">The <see cref="VisualElement"/> to add.</param>
        /// <exception cref="ArgumentException">Fired when the parent container is not found.</exception>
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
        /// Removes a UI Element.
        /// </summary>
        /// <param name="uiElement">The <see cref="VisualElement"/> to remove.</param>
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
                bool forcePortrait = Convert.ToBoolean(PlayerPrefs.GetInt("XRUIFormatOrientationPortrait")); 
                bool isLandscape =  !forcePortrait && (Input.deviceOrientation == DeviceOrientation.LandscapeLeft
                                   || Input.deviceOrientation == DeviceOrientation.LandscapeRight
                                   || (!Application.isEditor && (Application.platform != RuntimePlatform.Android || Application.platform != RuntimePlatform.IPhonePlayer) && Input.deviceOrientation == DeviceOrientation.Unknown));
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
        
        /// <summary>
        /// Checks if the auto rotation on Android devices is on.
        /// </summary>
        /// <returns>True if the auto rotation is on.</returns>
        private bool IsAndroidAutoRotateOn()
        {
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

    /// <summary>
    /// Set of parameters to alter the rendering of UI in world space.
    /// </summary>
    [Serializable]
    public struct WorldUIParameters
    {
        /// <summary>
        /// Bends the panel. Common practice in VR apps.
        /// </summary>
        [Tooltip("Alters the panel geometry by slightly bending it")]
        public bool bendPanel;
        /// <summary>
        /// Makes the panel follow the gaze of the camera, with a slight delay.
        /// </summary>
        [Tooltip("Defines if the panel will be anchored to the camera or not")]
        public bool anchorPanelToCamera;
        /// <summary>
        /// Defines the minimum distance that needs to be between the panel and the camera gaze before the panel recenters itself.
        /// </summary>
        [Tooltip("Defines the distance that the camera needs to travel away from the panel before the panel starts following it.")]
        public float cameraFollowThreshold;
        /// <summary>
        /// Overrides the size of the panel, which is otherwise calculated from the ratio of the width and height of the UI element defined in the USS sheet.
        /// </summary>
        [Tooltip("By default, XRUI uses the ratio of the element's dimensions defined in the USS. You can define a custom size here in Unity units here.")]
        public Vector2 customPanelDimensions;
        /// <summary>
        /// Sets the panel to the specified position in world coordinates. This is overriden if <see cref="anchorPanelToCamera"/> is true.
        /// </summary>
        [Tooltip("By default, the VR panel will be positioned at (0,0,1). You can define a custom position here.")]
        public Vector3 customPanelPosition;
        /// <summary>
        /// Alters the scale of the panel. By default, the size of panels tend towards one world space unit (one meter).
        /// </summary>
        [Tooltip("Alters the scale of the panel in the virtual world. This parameter is overridden if custom dimensions are specified")]
        public float panelScale;
    }
}
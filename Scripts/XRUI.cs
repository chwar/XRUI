// MIT License
// Copyright (c) 2021 Chris Warin
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
using System;
using System.Collections.Generic;
using com.chwar.xrui.UIElements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace com.chwar.xrui
{
    [ExecuteAlways]
    public class XRUI : MonoBehaviour
    {
        // Singleton
        public static XRUI Instance;

        [HideInInspector] public XRUIGridController xruiGridController;
        // Used to define the nature of UIs.
        [SerializeField, Tooltip(
            "Defines the way UIs will be rendered. 2D UIs are fitted for screens (i.e., PC or Mobile AR) while 3D UIs are rendered within the virtual world (i.e., for MR and VR)")]
        internal XRUIFormat xruiFormat = XRUIFormat.TwoDimensional;

        [Tooltip("By default, the 2D XRUI format uses Landscape USS styles when in Play mode in the Unity Editor. This forces 2D Portrait USS styles.")]
        public bool setTwoDimensionalFormatToPortraitInEditor;

        [SerializeField]
        internal XRUIConfiguration xruiConfigurationAsset;
        
        // List of UI Elements
        [SerializeField]
        internal List<VisualTreeAsset> uiElements = new();

        // List of Modals
        [SerializeField]
        internal List<InspectorModal> modals = new();

        
        private void Awake()
        {
            if(Instance == null)
            {
                if (Application.isPlaying)
                {
                    Instance = this;
                    DontDestroyOnLoad(gameObject);
                }
                else
                    // For Editor mode
                    Instance = FindObjectOfType<XRUI>();

                // Set the format given in the inspector
                SetCurrentXRUIFormat(xruiFormat,setTwoDimensionalFormatToPortraitInEditor);
                InitializeElements();
            }
            else
            {
                Debug.LogWarning("Found another XRUI Instance, destroying this one.");
                DestroyImmediate(gameObject);
            }
        }

        #if UNITY_EDITOR
        public void OnValidate()
        {
            // This only runs in Editor mode
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            SetCurrentXRUIFormat(xruiFormat,setTwoDimensionalFormatToPortraitInEditor);
            InitializeElements();
        }
        #endif


        internal void Reset()
        {
            xruiConfigurationAsset = Resources.Load<XRUIConfiguration>("DefaultXRUIConfiguration");
        }
        
        /// <summary>
        /// Defines the nature of the UI in order to fit the desired XR device as best as possible
        /// </summary>
        public enum XRUIFormat
        {
            TwoDimensional,
            ThreeDimensional
    }
        
        /// <summary>
        /// Defines the different alert types
        /// </summary>
        public enum AlertType
        {
            Primary,
            Success,
            Warning,
            Danger,
            Info
        }

        /// <summary>
        /// Returns the current XRUI format based on the format defined in the inspector.
        /// </summary>
        /// <returns>The current reality.</returns>
        public static string GetCurrentXRUIFormat()
        {
            return PlayerPrefs.GetString("XRUIFormat");
        }

        /// <summary>
        /// Returns true if format matches the current XRUI format.
        /// </summary>
        /// <param name="format">The XRUI format to compare.</param>
        /// <returns></returns>
        public static bool IsCurrentXRUIFormat(XRUIFormat format)
        {
            return GetCurrentXRUIFormat().Equals(format.ToString().ToLower());
        }

        /// <summary>
        /// Set the current XRUI format.
        /// </summary>
        /// <param name="format">The XRUI Format to use.</param>
        /// <param name="setOrientationPortrait">Whether to use Portrait orientation mode.</param>
        public void SetCurrentXRUIFormat(XRUIFormat format, bool setOrientationPortrait = false)
        {
            // Update inspector value if called from API
            xruiFormat = format;
            PlayerPrefs.SetString("XRUIFormat", format.ToString().ToLower());
            PlayerPrefs.SetInt("XRUIFormatOrientationPortrait", Convert.ToInt32(setOrientationPortrait));
            PlayerPrefs.Save();
        }
        
        /// <summary>
        /// Returns a VisualTreeAsset of the given name from the templates list defined in the inspector. 
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns>VisualTreeAsset of the given name from the templates list defined in the inspector.</returns>
        public VisualTreeAsset GetUIElement(string elementName)
        {
            var asset = uiElements.Find(ui => ui.name.Equals(elementName));
            if (asset is null)
            {
                throw new ArgumentException($"No UI element template with the name \"{elementName}\" could be found." +
                                            $"Check its presence in the inspector list.");
            }

            return asset;
        }
        
        public XRUIAlert ShowAlert(AlertType type, string text)
        {
            return ShowAlert(type, null, text);     
        }
        
        public XRUIAlert ShowAlert(AlertType type, string title, string text)
        {
            return ShowAlert(null, type, title, text, 0, null);
        }
        
        public XRUIAlert ShowAlert(AlertType type, string title, string text, int countdown)
        {
            return ShowAlert(null, type, title, text, countdown, null);
        }               
        public XRUIAlert ShowAlert(AlertType type, string title, string text, Action onClick)
        {
            return ShowAlert(null, type, title, text, 0, onClick);
        }

        public XRUIAlert ShowAlert(VisualTreeAsset template, AlertType type, string title, string text, int countdown, Action onClick)
        {
            var container = GetXRUIFloatingElementContainer(type + "Alert", false);
            var uiDocument = container.GetComponent<UIDocument>();

            // Instantiate template
            VisualElement alertContainer = template == null ? xruiConfigurationAsset.defaultAlertTemplate.Instantiate() : template.Instantiate();
            AdaptFloatingTemplateContainer(ref alertContainer);
            // Let the pointer hover over the rest of the elements
            alertContainer.pickingMode = PickingMode.Ignore;
            uiDocument.rootVisualElement.Add(alertContainer);

            // Style the alert accordingly
            alertContainer.ElementAt(0).AddToClassList(GetCurrentXRUIFormat());
            alertContainer.ElementAt(0).AddToClassList(type.ToString().ToLower());

            var xrui = container.AddComponent<XRUIAlert>();
            if (title is null)
            {
                xrui.Title.style.display = DisplayStyle.None;
            }
            else
                xrui.Title.text = title;
            xrui.Content.text = text;
            xrui.countdown = countdown;
            
            // World UI parameters
            var camPos = Camera.main.transform.position;
            xrui.worldUIParameters.customPanelPosition = new Vector3(camPos.x,
                camPos.y - .2f, camPos.z + .3f);
            xrui.worldUIParameters.anchorPanelToCamera = true;
            xrui.worldUIParameters.bendPanel = false;
            xrui.worldUIParameters = xruiConfigurationAsset.defaultAlertWorldUIParameters;
            xrui.worldUIParameters.cameraFollowThreshold = .1f;
            
            if(countdown > 0)
                xrui.DisposeAlert(false,false);
            
            if (onClick != null)
                xrui.ClickCallback = onClick;

            return xrui;
        }

        /// <summary>
        /// Generates a modal using the provided XRUI Modal template name and appends it in the modal container.
        /// </summary>
        /// <param name="modalName">Name of the modal.</param>
        /// <param name="additionalScript">User script to attach to the modal for user-defined behaviour.</param>
        public XRUIModal ShowModal(string modalName, Type additionalScript)
        {
            InspectorModal m = XRUI.Instance.modals.Find(modal => modal.modalName.Equals(modalName));
            if (m.modalName is null)
            {
                throw new ArgumentException($"No modal with the name \"{modalName}\" was found. " +
                                            $"Check its presence in the inspector.");
            }
            
            var container = GetXRUIFloatingElementContainer("XRUIModal", true);
            var uiDocument = container.GetComponent<UIDocument>();

            // Instantiate main template
            VisualElement modalContainer = m.mainTemplateOverride is null ? xruiConfigurationAsset.defaultModalTemplate.Instantiate() : m.mainTemplateOverride.Instantiate();
            AdaptFloatingTemplateContainer(ref modalContainer);
            uiDocument.rootVisualElement.Add(modalContainer);
            
            var xruiModal = container.AddComponent<XRUIModal>();
            xruiModal.worldUIParameters = xruiConfigurationAsset.defaultModalWorldUIParameters;
            xruiModal.modalFlowList = m.modalFlowList;
            container.AddComponent(additionalScript);
            container.transform.SetParent(container.transform);

            return xruiModal;
        }
        
        /// <summary>
        /// Generates a contextual menu displayed with respect to the position of the clicked element.
        /// </summary>
        /// <param name="parentCoordinates">The coordinates of the parent element that triggered this menu.</param>
        /// <param name="showArrow">Displays an arrow pointing at the parent element.</param>
        public XRUIContextualMenu ShowContextualMenu(Vector2 parentCoordinates, bool showArrow)
        {
            return ShowContextualMenu(null, parentCoordinates, showArrow);
        }

        /// <summary>
        /// Generates a contextual menu displayed with respect to the position of the clicked element.
        /// </summary>
        /// <param name="template">Custom contextual menu template to use, set to null to use default.</param>
        /// <param name="parentCoordinates">The coordinates of the parent element that triggered this menu.</param>
        /// <param name="showArrow">Displays an arrow pointing at the parent element.</param>
        /// <param name="leftOffset">Adds an offset in pixels used when the contextual menu is positioned on the left of the parent coordinates.</param>
        /// <param name="rightOffset">Adds an offset in pixels used when the contextual menu is positioned on the right of the parent coordinates.</param>
        public XRUIContextualMenu ShowContextualMenu(VisualTreeAsset template, Vector2 parentCoordinates, bool showArrow, float leftOffset = Single.NaN, float rightOffset = Single.NaN)
        {
            var container = GetXRUIFloatingElementContainer("ContextualMenu", false);
            var uiDocument = container.GetComponent<UIDocument>();

            // Instantiate template
            VisualElement contextualMenuContainer = template == null ? xruiConfigurationAsset.defaultContextualMenuTemplate.Instantiate() : template.Instantiate();
            AdaptFloatingTemplateContainer(ref contextualMenuContainer);
            uiDocument.rootVisualElement.Add(contextualMenuContainer);

            // Style and position the contextual menu accordingly
            contextualMenuContainer.ElementAt(0).AddToClassList(GetCurrentXRUIFormat());
            var xrui = container.AddComponent<XRUIContextualMenu>();
            // Use default element template, can be overriden
            xrui.menuElementTemplate = Resources.Load<VisualTreeAsset>("DefaultContextualMenuElement");
            xrui.parentCoordinates = parentCoordinates;
            xrui.showArrow = showArrow;
            xrui.worldUIParameters = xruiConfigurationAsset.defaultContextualMenuWorldUIParameters;
            xrui.worldUIParameters.customPanelDimensions = parentCoordinates;
            if (!float.IsNaN(leftOffset)) xrui.positionOffsetLeft = leftOffset;
            if (!float.IsNaN(rightOffset)) xrui.positionOffsetRight = rightOffset;

            return xrui;
        }

        /// <summary>
        /// Gets a Floating Element container or creates it if not existing.
        /// </summary>
        /// <returns>The Floating Elements container game object.</returns>
        private GameObject GetXRUIFloatingElementContainer(string containerName, bool bDarkenBackground)
        {
            var containerGO = GameObject.Find(containerName);
            if (containerGO is null)
            {
                containerGO = new GameObject {name = containerName};
                var ui = containerGO.AddComponent<UIDocument>();
                ui.panelSettings = xruiConfigurationAsset.panelSettings;
                ui.sortingOrder = 1000;
                ui.rootVisualElement.style.position = new StyleEnum<Position>(Position.Absolute);
                ui.rootVisualElement.style.top = 0;
                ui.rootVisualElement.style.bottom = 0;
                ui.rootVisualElement.style.left = 0;
                ui.rootVisualElement.style.right = 0;
                ui.rootVisualElement.style.justifyContent = new StyleEnum<Justify>(Justify.Center);
                ui.rootVisualElement.style.alignItems = new StyleEnum<Align>(Align.Center);
                ui.rootVisualElement.style.width = new StyleLength(Length.Percent(100));
                ui.rootVisualElement.style.height = new StyleLength(Length.Percent(100));
                ui.rootVisualElement.EnableInClassList("xrui-background--dark", bDarkenBackground);
            }
            return containerGO;
        }

        /// <summary>
        /// Helper to format a Template Container so that it it is scaled to the entire screen
        /// </summary>
        /// <param name="templateContainer"></param>
        private void AdaptFloatingTemplateContainer(ref VisualElement templateContainer)
        {
            templateContainer.style.width = new StyleLength(Length.Percent(100));
            templateContainer.style.height = new StyleLength(Length.Percent(100));
            templateContainer.style.justifyContent = new StyleEnum<Justify>(Justify.Center);
            templateContainer.style.alignItems = new StyleEnum<Align>(Align.Center);
        }

        /// <summary>
        /// Generates a mesh on which a render texture is created. The render texture renders the XRUI element.
        /// </summary>
        /// <param name="uiDocument"></param>
        internal static void GetWorldUIPanel(GeometryChangedEvent evt, UIDocument uiDocument)
        {
            ((VisualElement) evt.target).UnregisterCallback<GeometryChangedEvent, UIDocument>(GetWorldUIPanel);
            var xrui = uiDocument.GetComponent<XRUIElement>();

            // Position the GO at the same height as the HMD / Camera
            var o = uiDocument.gameObject;
            var dimensions = xrui.RootElement.resolvedStyle;

            if (dimensions.width == 0 || dimensions.height == 0)
            {
                throw new ArgumentException($"The UI {uiDocument.name} has invalid dimensions. Make sure to add a corresponding Three-Dimensional USS rule.");
            }

            var ratio = GetGreatestCommonDivisor((int) dimensions.width, (int) dimensions.height);
            // Make the world UI panel dimensions tend towards one unity unit
            var scale = 1 / (dimensions.width / ratio);

            RenderTexture rt = new RenderTexture((int) dimensions.width, (int) dimensions.height, 24)
            {
                name = uiDocument.name,
                useDynamicScale = true
            };
            rt.Create();
            uiDocument.panelSettings.targetTexture = rt;

            o.AddComponent<XRUIWorldSpaceInteraction>();
            var plane = o.GetComponent<XRUIPanel>() ? o.GetComponent<XRUIPanel>() : o.AddComponent<XRUIPanel>();
            if (xrui.worldUIParameters.panelScale.Equals(0))
                xrui.worldUIParameters.panelScale = 1;
            plane.numSegments = 512;
            plane.height = xrui.worldUIParameters.customPanelDimensions.Equals(Vector2.zero) ? (scale * (dimensions.height / ratio)) * xrui.worldUIParameters.panelScale : xrui.worldUIParameters.customPanelDimensions.y;
            plane.radius = xrui.worldUIParameters.customPanelDimensions.Equals(Vector2.zero) ? (scale * (dimensions.width / ratio)) * xrui.worldUIParameters.panelScale : xrui.worldUIParameters.customPanelDimensions.x;
            plane.useArc = xrui.worldUIParameters.bendPanel;
            plane.curvatureDegrees = xrui.worldUIParameters.bendPanel ? 60 : 0;
            plane.Generate(rt);
            o.transform.position = xrui.worldUIParameters.customPanelPosition.Equals(Vector3.zero) ? Camera.main.transform.forward : xrui.worldUIParameters.customPanelPosition;

            var collider = o.GetComponent<MeshCollider>() ? o.GetComponent<MeshCollider>() : o.AddComponent<MeshCollider>();
            collider.sharedMesh = plane.mesh;
            // var meshRenderer =  o.GetComponent<MeshRenderer>();
            // meshRenderer.material.shader = Shader.Find("Unlit/Texture MMBias");
            
            // Add Physics Raycaster to enable XRI interactions
            o.AddComponent<TrackedDevicePhysicsRaycaster>();
        }
        

        private static int GetGreatestCommonDivisor(int a, int b) {
            return b == 0 ? Math.Abs(a) : GetGreatestCommonDivisor(b, a % b);
        }

        internal void InitializeElements()
        {
            xruiGridController = FindObjectOfType<XRUIGridController>();
            if(xruiGridController is not null) 
                xruiGridController.AdaptGrid();
            foreach (XRUIElement xruiElement in FindObjectsOfType<XRUIElement>())
            {
                xruiElement.Init();
                xruiElement.UpdateUI();
            }
        }
    }

    [Serializable]
    struct InspectorModal
    {
        [Tooltip("Name of the modal")]
        public string modalName;
        [Tooltip("Main template used as root content for the modal")]
        public VisualTreeAsset mainTemplateOverride;
        [Tooltip("List of contents that appear in this modal in order of navigation")]
        public List<VisualTreeAsset> modalFlowList;
    }
}
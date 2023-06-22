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
    /// <summary>
    /// Main controller of the XRUI Framework.
    /// </summary>
    [ExecuteAlways]
    public class XRUI : MonoBehaviour
    {
        #region Attributes
        
        /// <summary>
        /// The <see cref="XRUIConfiguration"/> to use for XRUI.
        /// </summary>
        [SerializeField]
        internal XRUIConfiguration xruiConfigurationAsset;
        /// <summary>
        /// List of UI Elements to be referenced in the Inspector.
        /// </summary>
        [SerializeField]
        internal List<VisualTreeAsset> uiElements = new();
        /// <summary>
        /// List of Modals to be referenced in the Inspector.
        /// </summary>
        [SerializeField]
        internal List<InspectorModal> modals = new();
        /// <summary>
        /// Defines the <see cref="XRUIFormat"/> which sets the UI to 2D or 3D.
        /// </summary>
        [SerializeField, Tooltip(
            "Defines the way UIs will be rendered. 2D UIs are fitted for screens (i.e., PC or Mobile AR) while 3D UIs are rendered within the virtual world (i.e., for MR and VR)")]
        internal XRUIFormat xruiFormat = XRUIFormat.TwoDimensional;
        /// <summary>
        /// The <see cref="XRUIGridController"/> used to organise the UI.
        /// </summary>
        [HideInInspector] public XRUIGridController xruiGridController;
        /// <summary>
        /// Forces 2D Portrait USS styles when in the Unity Editor.
        /// </summary>
        [Tooltip("By default, the 2D XRUI format uses Landscape USS styles when in the Unity Editor. This forces 2D Portrait USS styles.")]
        public bool forceTwoDimensionalFormatToPortrait;
        /// <summary>
        /// Instance of this class (singleton).
        /// </summary>
        public static XRUI Instance;
        /// <summary>
        /// Defines the nature of the UI in order to fit the desired XR device as best as possible.
        /// </summary>
        public enum XRUIFormat
        {
            /// <summary>
            /// Two Dimensional UI, for use on traditional screens, e.g. PC, smartphones, tablets, etc.
            /// </summary>
            TwoDimensional,
            /// <summary>
            /// Three Dimensional or World Space UI. Needed for displaying UI for MR/VR applications (can also be used for AR).
            /// </summary>
            ThreeDimensional
        }
        
        #endregion

        #region UnityMethods
        
        /// <summary>
        /// Unity method which instantiates the Singleton design pattern.
        /// </summary>
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
                SetGlobalXRUIFormat(xruiFormat,forceTwoDimensionalFormatToPortrait);
                InitializeElements();
            }
            else
            {
                Debug.LogWarning("Found another XRUI Instance, destroying this one.");
                DestroyImmediate(gameObject);
            }
        }

        /// <summary>
        /// Editor method that enables real time UI updating while in the Editor.
        /// </summary>
        #if UNITY_EDITOR
        public void OnValidate()
        {
            // This only runs in Editor mode
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            SetGlobalXRUIFormat(xruiFormat,forceTwoDimensionalFormatToPortrait);
            InitializeElements();
        }
        #endif


        /// <summary>
        /// Unity method that puts default values to the XRUI Instance in the Inspector.
        /// Reverts the <see cref="XRUIFormat"/> to <see cref="XRUIFormat.TwoDimensional"/> and <see cref="xruiConfigurationAsset"/> to the default 2D configuration.
        /// </summary>
        internal void Reset()
        {
            xruiFormat = XRUIFormat.TwoDimensional;
            xruiConfigurationAsset = Resources.Load<XRUIConfiguration>("DefaultXRUI2DConfiguration");
        }
        
        #endregion

        #region XRUIMethods
        
        /// <summary>
        /// Set the current XRUI format.
        /// </summary>
        /// <param name="format">The <see cref="XRUIFormat"/> to use.</param>
        /// <param name="setOrientationPortrait">Whether to use Portrait orientation mode.</param>
        internal void SetGlobalXRUIFormat(XRUIFormat format, bool setOrientationPortrait = false)
        {
            // Update inspector value if called from API
            xruiFormat = format;
            PlayerPrefs.SetString("XRUIFormat", format.ToString());
            PlayerPrefs.SetInt("XRUIFormatOrientationPortrait", Convert.ToInt32(setOrientationPortrait));
            PlayerPrefs.Save();
        }
        
        /// <summary>
        /// Returns the current <see cref="XRUIFormat"/> based on the format defined in the inspector.
        /// </summary>
        /// <returns>The current <see cref="XRUIFormat"/>.</returns>
        public static string GetGlobalXRUIFormat()
        {
            return PlayerPrefs.GetString("XRUIFormat");
        }

        /// <summary>
        /// Compares an <see cref="XRUIFormat"/> to the current one. .
        /// </summary>
        /// <param name="format">The <see cref="XRUIFormat"/> to compare.</param>
        /// <returns>True if <paramref name="format"/> matches the current <see cref="XRUIFormat"/></returns>
        public static bool IsGlobalXRUIFormat(XRUIFormat format)
        {
            return GetGlobalXRUIFormat().Equals(format.ToString());
        }

        /// <summary>
        /// Returns a <see cref="VisualTreeAsset"/> of the given name from the templates list defined in the inspector. 
        /// </summary>
        /// <param name="elementName">The <see cref="VisualTreeAsset"/> to find.</param>
        /// <returns><see cref="VisualTreeAsset"/> of the given name from the templates list defined in the inspector.</returns>
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
        
        /// <summary>
        /// When the XRUI Instance is initialised, all XRUI Elements are initialised by this method.
        /// </summary>
        internal void InitializeElements()
        {
            xruiGridController = FindObjectOfType<XRUIGridController>();
            if(xruiGridController is not null) 
                xruiGridController.RefreshGrid();
            foreach (XRUIElement xruiElement in FindObjectsOfType<XRUIElement>())
            {
                xruiElement.Init();
                xruiElement.UpdateUI();
            }
        }
        
        #endregion
        
        #region XRUIAlerts

        /// <summary>
        /// Shows an alert to the end-user.
        /// </summary>
        /// <param name="type">The <see cref="XRUIAlert.AlertType"/> to use.</param>
        /// <param name="text">The body of the alert.</param>
        ///         /// <returns>The created alert.</returns>
        public XRUIAlert ShowAlert(XRUIAlert.AlertType type, string text)
        {
            return ShowAlert(type, null, text);     
        }
        
        /// <summary>
        /// Shows an alert to the end-user.
        /// </summary>
        /// <param name="type">The <see cref="XRUIAlert.AlertType"/> to use.</param>
        /// <param name="title">The title of the alert.</param>
        /// <param name="text">The body of the alert.</param>
        /// <returns>The created alert.</returns>
        public XRUIAlert ShowAlert(XRUIAlert.AlertType type, string title, string text)
        {
            return ShowAlert(null, type, title, text, 0, null);
        }
        
        /// <summary>
        /// Shows an alert to the end-user.
        /// </summary>
        /// <param name="type">The <see cref="XRUIAlert.AlertType"/> to use.</param>
        /// <param name="title">The title of the alert.</param>
        /// <param name="text">The body of the alert.</param>
        /// <param name="countdown">Optional countdown after which the alert automatically disappears.</param>
        /// <returns>The created alert.</returns>
        public XRUIAlert ShowAlert(XRUIAlert.AlertType type, string title, string text, int countdown)
        {
            return ShowAlert(null, type, title, text, countdown, null);
        }               
        
        /// <summary>
        /// Shows an alert to the end-user.
        /// </summary>
        /// <param name="type">The <see cref="XRUIAlert.AlertType"/> to use.</param>
        /// <param name="title">The title of the alert.</param>
        /// <param name="text">The body of the alert.</param>
        /// <param name="onClick">Optional <see cref="Action"/> that is fired after a click on the alert.</param>
        /// <returns>The created alert.</returns>
        public XRUIAlert ShowAlert(XRUIAlert.AlertType type, string title, string text, Action onClick)
        {
            return ShowAlert(null, type, title, text, 0, onClick);
        }

        /// <summary>
        /// Shows an alert to the end-user.
        /// </summary>
        /// <param name="template">Custom alert template to use, set it to null to use the default.</param>
        /// <param name="type">The <see cref="XRUIAlert.AlertType"/> to use.</param>
        /// <param name="title">The title of the alert.</param>
        /// <param name="text">The body of the alert.</param>
        /// <param name="countdown">Optional countdown after which the alert automatically disappears.</param>
        /// <param name="onClick">Optional <see cref="Action"/> that is fired after a click on the alert.</param>
        /// <returns>The created alert.</returns>
        public XRUIAlert ShowAlert(VisualTreeAsset template, XRUIAlert.AlertType type, string title, string text, int countdown, Action onClick)
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
            alertContainer.ElementAt(0).AddToClassList(GetGlobalXRUIFormat());
            alertContainer.ElementAt(0).AddToClassList(type.ToString().ToLower());

            var xrui = container.AddComponent<XRUIAlert>();
            xrui.worldUIParameters = xruiConfigurationAsset.defaultAlertWorldUIParameters;
            if (title is null)
            {
                xrui.Title.style.display = DisplayStyle.None;
            }
            else
                xrui.Title.text = title;
            xrui.Content.text = text;
            xrui.countdown = countdown;
            
            // Alter camera position in World UI parameters
            // var camPos = Camera.main.transform.position;
            // xrui.worldUIParameters.customPanelPosition = new Vector3(camPos.x,
            //     camPos.y - .2f, camPos.z + .3f);
            if (onClick != null)
                xrui.clickCallback = onClick;

            if(countdown > 0)
                xrui.DisposeAlert(false,false);
            

            return xrui;
        }
        
        #endregion

        #region XRUIModals
        
        /// <summary>
        /// Generates a modal using the provided XRUI Modal template name and appends it in the modal container.
        /// </summary>
        /// <param name="modalName">Name of the modal.</param>
        /// <param name="additionalScript">User script to attach to the modal for user-defined behaviour.</param>
        /// <returns>The created modal.</returns>
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
        
        #endregion

        #region XRUIContextualMenus

        /// <summary>
        /// Generates a contextual menu displayed with respect to the position of the clicked element.
        /// </summary>
        /// <param name="parentCoordinates">The coordinates of the parent element that triggered this menu.</param>
        /// <param name="showArrow">Displays an arrow pointing at the parent element.</param>
        /// <returns>The created contextual menu.</returns>
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
        /// <returns>The created contextual menu.</returns>
        public XRUIContextualMenu ShowContextualMenu(VisualTreeAsset template, Vector2 parentCoordinates, bool showArrow, float leftOffset = Single.NaN, float rightOffset = Single.NaN)
        {
            var container = GetXRUIFloatingElementContainer("ContextualMenu", false);
            var uiDocument = container.GetComponent<UIDocument>();

            // Instantiate template
            VisualElement contextualMenuContainer = template == null ? xruiConfigurationAsset.defaultContextualMenuTemplate.Instantiate() : template.Instantiate();
            AdaptFloatingTemplateContainer(ref contextualMenuContainer);
            uiDocument.rootVisualElement.Add(contextualMenuContainer);

            // Style and position the contextual menu accordingly
            contextualMenuContainer.ElementAt(0).AddToClassList(GetGlobalXRUIFormat());
            var xrui = container.AddComponent<XRUIContextualMenu>();
            // Use default element template, can be overriden
            xrui.menuElementTemplate = Resources.Load<VisualTreeAsset>("DefaultContextualMenuElement");
            xrui.worldUIParameters = xruiConfigurationAsset.defaultContextualMenuWorldUIParameters;
            xrui.parentCoordinates = parentCoordinates;
            xrui.showArrow = showArrow && !IsGlobalXRUIFormat(XRUIFormat.ThreeDimensional);
 
            if (!float.IsNaN(leftOffset)) xrui.positionOffsetLeft = leftOffset;
            if (!float.IsNaN(rightOffset)) xrui.positionOffsetRight = rightOffset;

            return xrui;
        }
        
        #endregion

        #region XRUIRenderingHelpers
        
        /// <summary>
        /// Gets a Floating Element container or creates it if not existing.
        /// </summary>
        /// <returns>The Floating Element container game object.</returns>
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
        /// Helper to format a Template Container so that it it is scaled to the entire screen.
        /// </summary>
        /// <param name="templateContainer">The template container to format.</param>
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
        /// <param name="evt">The GeometryChangedEvent that triggered the layout pass.</param>
        /// <param name="uiDocument">The UI Document of the XRUI Element.</param>
        internal static void GetWorldUIPanel(GeometryChangedEvent evt, UIDocument uiDocument)
        {
            // ((VisualElement) evt.target).UnregisterCallback<GeometryChangedEvent, UIDocument>(GetWorldUIPanel);
            
            var xrui = uiDocument.GetComponent<XRUIElement>();
            // Do not process when UI is hidden
            if (xrui.RootElement.ClassListContains("xrui--hide")) return;

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

            var plane = o.GetComponent<XRUIPanel>() ? o.GetComponent<XRUIPanel>() : o.AddComponent<XRUIPanel>();
            if (xrui.worldUIParameters.panelScale.Equals(0))
                xrui.worldUIParameters.panelScale = 1;
            if (xrui.worldUIParameters.anchorPanelToCamera) 
                xrui.StartFollowingCamera();
            plane.numSegments = 128;
            plane.height = xrui.worldUIParameters.customPanelDimensions.Equals(Vector2.zero) ? (scale * (dimensions.height / ratio)) * xrui.worldUIParameters.panelScale : xrui.worldUIParameters.customPanelDimensions.y;
            plane.radius = xrui.worldUIParameters.customPanelDimensions.Equals(Vector2.zero) ? (scale * (dimensions.width / ratio)) * xrui.worldUIParameters.panelScale : xrui.worldUIParameters.customPanelDimensions.x;
            plane.useArc = xrui.worldUIParameters.bendPanel;
            plane.curvatureDegrees = xrui.worldUIParameters.bendPanel ? 60 : 0;
            plane.Generate(rt);

            if (!xrui.worldUIParameters.disableXRInteraction)
            {
                var collider = o.GetComponent<MeshCollider>() ? o.GetComponent<MeshCollider>() : o.AddComponent<MeshCollider>();
                collider.sharedMesh = plane.mesh;
                // Add Physics Raycaster to enable XRI interactions
                o.AddComponent<XRUIWorldSpaceInteraction>();
                o.AddComponent<TrackedDevicePhysicsRaycaster>();
            }
            // find the automatically generated PanelRaycasters for World Space XRUI panels and disable them, as they do not work properly
            foreach (PanelRaycaster panelRaycaster in FindObjectsOfType<PanelRaycaster>())
            {
                var ui = panelRaycaster.panel.visualTree.ElementAt(0);
                panelRaycaster.enabled = !(ui.ClassListContains("xrui") && ui.ClassListContains("threedimensional"));
            }
            
            // TODO Find a shader that can fade out (transparent) but that culls rays from MR/VR controllers
            // var meshRenderer =  o.GetComponent<MeshRenderer>();
            // meshRenderer.material.shader = Shader.Find("Unlit/Texture MMBias");
        }
        
        /// <summary>
        /// Helper that returns the greatest common divisor of two numbers. Used to calculate the ratio of a world UI panel.
        /// </summary>
        /// <param name="a">First number to compare</param>
        /// <param name="b">Second number to compare</param>
        /// <returns>The GCD between a and b.</returns>
        private static int GetGreatestCommonDivisor(int a, int b) {
            return b == 0 ? Math.Abs(a) : GetGreatestCommonDivisor(b, a % b);
        }
        
        #endregion
    }

    /// <summary>
    /// Lets users reference modals in the Unity Inspector for ease of access.
    /// </summary>
    [Serializable]
    struct InspectorModal
    {
        /// <summary>
        /// Name of the modal
        /// </summary>
        [Tooltip("Name of the modal")]
        public string modalName;
        /// <summary>
        /// Main template used as root content for the modal
        /// </summary>
        [Tooltip("Main template used as root content for the modal")]
        public VisualTreeAsset mainTemplateOverride;
        /// <summary>
        /// List of contents that appear in this modal
        /// </summary>
        [Tooltip("List of contents that appear in this modal")]
        public List<VisualTreeAsset> modalFlowList;
    }
}
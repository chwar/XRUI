// MIT License
// Copyright (c) 2021 Chris Warin
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
using System;
using System.Collections.Generic;
using com.chwar.xrui.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.chwar.xrui
{
    public class XRUI : MonoBehaviour
    {
        // Singleton
        public static XRUI Instance;

        [HideInInspector] public XRUIGridController xruiGridController;
        // Used to override global XRUI reality
        public RealityType realityType = RealityType.PC; 

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
                Instance = this;
                DontDestroyOnLoad(gameObject);
                
                // Set the reality given in the scene
                SetCurrentReality(realityType);
                this.InitializeElements();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void OnValidate()
        {
            SetCurrentReality(realityType);
        }


        internal void Reset()
        {
            xruiConfigurationAsset = Resources.Load<XRUIConfiguration>("DefaultXRUIConfiguration");
        }
        
        /// <summary>
        /// Defines the different XR realities
        /// </summary>
        public enum RealityType
        {
            PC,
            AR,
            VR
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
        /// Returns the current reality based on the running platform.
        /// </summary>
        /// <returns>The current reality.</returns>
        public static string GetCurrentReality()
        {
            // switch (Application.platform)
            // {
                // case RuntimePlatform.Android:
                // case RuntimePlatform.IPhonePlayer:
                //     if(Input.deviceOrientation == DeviceOrientation.Portrait || Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown)
                //         return RealityType.AR.ToString().ToLower();
                //     if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft ||
                //         Input.deviceOrientation == DeviceOrientation.LandscapeRight)
                //         return RealityType.PC.ToString().ToLower();
                //     break;
                // case RuntimePlatform.WindowsPlayer:
                // case RuntimePlatform.OSXPlayer:
                // case RuntimePlatform.LinuxPlayer:
                //     return RealityType.PC.ToString().ToLower();
                // default:
                //}
                // return null;
            return PlayerPrefs.GetString("reality");
        }

        public static bool IsCurrentReality(RealityType type)
        {
            return GetCurrentReality().Equals(type.ToString().ToLower());
        }
        
        /// <summary>
        /// Editor method to set the current reality.
        /// Since the runtime platform is set to Editor, this sets the correct reality in the PlayerPrefs.
        /// </summary>
        /// <param name="type"></param>
        public static void SetCurrentReality(RealityType type)
        {
            PlayerPrefs.SetString("reality", type.ToString().ToLower());
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
        
        public void ShowAlert(AlertType type, string text)
        {
            ShowAlert(type, null, text);     
        }
        
        public void ShowAlert(AlertType type, string title, string text)
        {
            ShowAlert(null, type, title, text, null);
        }
        
        public void ShowAlert(AlertType type, string title, string text, Action onClick)
        {
            ShowAlert(null, type, title, text, onClick);
        }
        
        public void ShowAlert(VisualTreeAsset template, AlertType type, string title, string text, Action onClick)
        {
            var container = GetXRUIFloatingElementContainer(type + "Alert", false);
            var uiDocument = container.GetComponent<UIDocument>();
            uiDocument.rootVisualElement.style.justifyContent = new StyleEnum<Justify>(Justify.Center);
            uiDocument.rootVisualElement.style.alignItems = new StyleEnum<Align>(Align.Center);
            uiDocument.rootVisualElement.style.width = new StyleLength(Length.Percent(100));
            uiDocument.rootVisualElement.style.height = new StyleLength(Length.Percent(100));
            
            // Instantiate template
            VisualElement alertContainer = template == null ? xruiConfigurationAsset.defaultAlertTemplate.Instantiate() : template.Instantiate();
            alertContainer.style.position = new StyleEnum<Position>(Position.Absolute);
            alertContainer.style.bottom = new StyleLength(0f);
            alertContainer.style.top = new StyleLength(0f);
            alertContainer.style.left = new StyleLength(0f);
            alertContainer.style.right = new StyleLength(0f);
            uiDocument.rootVisualElement.Add(alertContainer);

            // Style the alert accordingly
            alertContainer.ElementAt(0).AddToClassList(GetCurrentReality());
            alertContainer.ElementAt(0).AddToClassList(type.ToString().ToLower());

            var xrui = container.AddComponent<XRUIAlert>();
            if (title is null)
            {
                xrui.Title.style.display = DisplayStyle.None;
            }
            else
            {
                xrui.Title.text = title;
            }
            xrui.Content.text = text;
            if (onClick != null)
                xrui.ClickCallback = onClick;
        }

        /// <summary>
        /// Generates a modal using the provided XRUI Modal template name and appends it in the modal container.
        /// </summary>
        /// <param name="modalName">Name of the modal.</param>
        /// <param name="additionalScript">User script to attach to the modal for user-defined behaviour.</param>
        public void CreateModal(string modalName, Type additionalScript)
        {
            InspectorModal m = XRUI.Instance.modals.Find(modal => modal.modalName.Equals(modalName));
            if (m.modalName is null)
            {
                throw new ArgumentException($"No modal with the name \"{modalName}\" was found. " +
                                            $"Check its presence in the inspector.");
            }
            
            var container = GetXRUIFloatingElementContainer("XRUIModal", true);
            var uiDocument = container.GetComponent<UIDocument>();
            uiDocument.rootVisualElement.style.justifyContent = new StyleEnum<Justify>(Justify.Center);
            uiDocument.rootVisualElement.style.alignItems = new StyleEnum<Align>(Align.Center);
            uiDocument.rootVisualElement.style.width = new StyleLength(Length.Percent(100));
            uiDocument.rootVisualElement.style.height = new StyleLength(Length.Percent(100));
            
            // Instantiate main template
            VisualElement modalContainer = m.mainTemplateOverride is null ? xruiConfigurationAsset.defaultModalTemplate.Instantiate() : m.mainTemplateOverride.Instantiate();
            modalContainer.style.width = new StyleLength(Length.Percent(100));
            modalContainer.style.height = new StyleLength(Length.Percent(100));
            modalContainer.style.justifyContent = new StyleEnum<Justify>(Justify.Center);
            modalContainer.style.alignItems = new StyleEnum<Align>(Align.Center);
            uiDocument.rootVisualElement.Add(modalContainer);
            
            var xruiModal = container.AddComponent<XRUIModal>();
            xruiModal.modalFlowList = m.modalFlowList;
            container.AddComponent(additionalScript);
            container.transform.SetParent(container.transform);
        }
        
        /// <summary>
        /// Generates a contextual menu displayed with respect to the position of the clicked element.
        /// </summary>
        /// <param name="parentCoordinates">The coordinates of the parent element that triggered this menu.</param>
        /// <param name="showArrow">Displays an arrow pointing at the parent element.</param>
        public void ShowContextualMenu(Vector2 parentCoordinates, bool showArrow)
        {
            ShowContextualMenu(null, parentCoordinates, showArrow);
        }

        /// <summary>
        /// Generates a contextual menu displayed with respect to the position of the clicked element.
        /// </summary>
        /// <param name="template">Custom contextual menu template to use, set to null to use default.</param>
        /// <param name="parentCoordinates">The coordinates of the parent element that triggered this menu.</param>
        /// <param name="showArrow">Displays an arrow pointing at the parent element.</param>
        public void ShowContextualMenu(VisualTreeAsset template, Vector2 parentCoordinates, bool showArrow)
        {
            ShowContextualMenu(template, parentCoordinates, showArrow, Single.NaN, Single.NaN);
        }

        /// <summary>
        /// Generates a contextual menu displayed with respect to the position of the clicked element.
        /// </summary>
        /// <param name="template">Custom contextual menu template to use, set to null to use default.</param>
        /// <param name="parentCoordinates">The coordinates of the parent element that triggered this menu.</param>
        /// <param name="showArrow">Displays an arrow pointing at the parent element.</param>
        /// <param name="leftOffset">Adds an offset in pixels used when the contextual menu is positioned on the left of the parent coordinates.</param>
        /// <param name="rightOffset">Adds an offset in pixels used when the contextual menu is positioned on the right of the parent coordinates.</param>
        public XRUIContextualMenu ShowContextualMenu(VisualTreeAsset template, Vector2 parentCoordinates, bool showArrow, float leftOffset, float rightOffset)
        {
            var container = GetXRUIFloatingElementContainer("ContextualMenu", false);
            var uiDocument = container.GetComponent<UIDocument>();
            uiDocument.rootVisualElement.style.justifyContent = new StyleEnum<Justify>(Justify.Center);
            uiDocument.rootVisualElement.style.alignItems = new StyleEnum<Align>(Align.Center);
            uiDocument.rootVisualElement.style.width = new StyleLength(Length.Percent(100));
            uiDocument.rootVisualElement.style.height = new StyleLength(Length.Percent(100));
            uiDocument.rootVisualElement.pickingMode = PickingMode.Position;
            
            // Instantiate template
            VisualElement contextualMenuContainer = template == null ? xruiConfigurationAsset.defaultContextualMenuTemplate.Instantiate() : template.Instantiate();
            var contextualMenu = contextualMenuContainer.ElementAt(0);
            uiDocument.rootVisualElement.Add(contextualMenu);

            // Style and position the contextual menu accordingly
            contextualMenu.AddToClassList(GetCurrentReality());
            var xrui = container.AddComponent<XRUIContextualMenu>();
            // Use default element template, can be overriden
            xrui.menuElementTemplate = Resources.Load<VisualTreeAsset>("DefaultContextualMenuElement");
            xrui.parentCoordinates = parentCoordinates;
            xrui.showArrow = showArrow;
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
                ui.rootVisualElement.EnableInClassList("xrui__background--dark", bDarkenBackground);
            }
            return containerGO;
        }
        
        
        /// <summary>
        /// Generates a mesh on which a render texture is created. The render texture renders the XRUI element.
        /// </summary>
        /// <param name="uiDocument"></param>
        internal static void GetVRPanel(GeometryChangedEvent evt, UIDocument uiDocument)
        {
            ((VisualElement) evt.target).UnregisterCallback<GeometryChangedEvent, UIDocument>(GetVRPanel);
            var xrui = uiDocument.GetComponent<XRUIElement>();
            bool uiIsFloatingElement = xrui.GetType().IsSubclassOf(typeof(XRUIFloatingElement));

            // Position the GO at the same height as the HMD / Camera
            var o = uiDocument.gameObject;
            var dimensions = uiDocument.rootVisualElement.Q(null, "xrui").resolvedStyle;

            if (dimensions.width == 0 || dimensions.height == 0)
            {
                throw new ArgumentException($"The UI {uiDocument.name} has invalid dimensions. Make sure to add a corresponding VR USS rule.");
            }

            var ratio = GetGreatestCommonDivisor((int) dimensions.width, (int) dimensions.height);
            RenderTexture rt = new RenderTexture((int) dimensions.width, (int) dimensions.height, 24)
            {
                name = uiDocument.name
            };
            rt.vrUsage = VRTextureUsage.TwoEyes;
            rt.useDynamicScale = true;
            rt.Create();
            PanelSettings ps = uiDocument.panelSettings.targetTexture == null ? 
                Instantiate(Resources.Load<PanelSettings>("DefaultPanelSettings")) : uiDocument.panelSettings;
            ps.scaleMode = PanelScaleMode.ConstantPhysicalSize;
            ps.targetTexture = rt;
            
            try
            {
                uiDocument.panelSettings = ps;
            }
            catch (Exception)
            {
                // do nothing
            }
            
            o.AddComponent<XRUITextureInteraction>();
            var plane = o.GetComponent<CurvedPlane>() ? o.GetComponent<CurvedPlane>() : o.AddComponent<CurvedPlane>();
            plane.numSegments = 512;
            plane.height = xrui.vrParameters.customVRPanelDimensions.Equals(Vector2.zero) ? (dimensions.height / ratio) / 10 : xrui.vrParameters.customVRPanelDimensions.y;
            plane.radius = xrui.vrParameters.customVRPanelDimensions.Equals(Vector2.zero) ? (dimensions.width / ratio) / 10 : xrui.vrParameters.customVRPanelDimensions.x;
            plane.useArc = xrui.vrParameters.bendVRPanel || uiIsFloatingElement;
            plane.curvatureDegrees = xrui.vrParameters.bendVRPanel || uiIsFloatingElement ? 60 : 0;
            plane.Generate(rt);
            if (xrui.vrParameters.anchorVRPanelToCamera || uiIsFloatingElement)
            {
                o.transform.parent = Camera.main.transform;
                o.transform.localPosition = xrui.vrParameters.customVRPanelAnchorPosition.Equals(Vector3.zero) ? new Vector3(0, 0, .1f) : xrui.vrParameters.customVRPanelAnchorPosition;
                o.transform.localRotation = Quaternion.identity;
            } else
                o.transform.position = Camera.main.transform.position + Vector3.forward * 2;

            var collider = o.GetComponent<MeshCollider>() ? o.GetComponent<MeshCollider>() : o.AddComponent<MeshCollider>();
            collider.sharedMesh = plane.mesh;
        }

        private static int GetGreatestCommonDivisor(int a, int b) {
            return b == 0 ? Math.Abs(a) : GetGreatestCommonDivisor(b, a % b);
        }

        internal void InitializeElements()
        {
            foreach (XRUIElement xruiElement in FindObjectsOfType<XRUIElement>())
            {
                xruiElement.Init();
                xruiElement.UpdateUI();
            }
            xruiGridController = FindObjectOfType<XRUIGridController>();
            if(xruiGridController is not null) 
                xruiGridController.AdaptGrid();
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
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

        // Flag that other XRUI classes wait for
        public bool Ready { get; private set; }
        
        // Used to override global XRUI reality
        public RealityType realityType = RealityType.UseGlobal; 
        private RealityType _editorRealityType;
        
        
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

                if (!realityType.Equals(RealityType.UseGlobal))
                {
                    // Use the scene specific XRUI preference
                    Enum.TryParse(GetCurrentReality(), true, out _editorRealityType);
                    SetCurrentReality(realityType);
                    this.Ready = true;
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void OnApplicationQuit()
        {
            // Set back the global XRUI preference
            if (!realityType.Equals(RealityType.UseGlobal))
                SetCurrentReality(_editorRealityType);
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
            UseGlobal,
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
        /// Updates the USS class of the given UIDocument to the current reality.
        /// </summary>
        /// <param name="uiDocument">The UIDocument to update.</param>
        internal static void UpdateDocumentUI(UIDocument uiDocument)
        {
            if (uiDocument != null && uiDocument.rootVisualElement != null)
            {
                var xrui = uiDocument.rootVisualElement.Q(null, "xrui");
                xrui.EnableInClassList(GetCurrentReality(), true);
                if (IsCurrentReality(RealityType.VR) && Application.isPlaying)
                {
                    // Create a runtime VR panel after the layout pass
                    xrui.RegisterCallback<GeometryChangedEvent, UIDocument>(GetVRPanel, uiDocument);
                }
            }
        }
        
        /// <summary>
        /// Returns the current reality based on the running platform.
        /// </summary>
        /// <returns>The current reality.</returns>
        public static string GetCurrentReality()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                case RuntimePlatform.IPhonePlayer:
                    if(Input.deviceOrientation == DeviceOrientation.Portrait || Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown)
                        return RealityType.AR.ToString().ToLower();
                    if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft ||
                        Input.deviceOrientation == DeviceOrientation.LandscapeRight)
                        return RealityType.PC.ToString().ToLower();
                    break;
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.LinuxPlayer:
                    return RealityType.PC.ToString().ToLower();
                default:
                    return PlayerPrefs.GetString("reality");
            }
            return null;
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
            ShowAlert(null, type, title, text);
        }
        
        public void ShowAlert(VisualTreeAsset template, AlertType type, string title, string text)
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
            VisualElement modalContainer = m.mainTemplate is null ? xruiConfigurationAsset.defaultModalTemplate.Instantiate() : m.mainTemplate.Instantiate();
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
                ui.rootVisualElement.EnableInClassList("darken", bDarkenBackground);
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
            VRParameters parameters = uiDocument.GetComponent<XRUIElement>().VRParameters;

            // Position the GO at the same height as the HMD / Camera
            var o = uiDocument.gameObject;
            var dimensions = uiDocument.rootVisualElement.Q(null, "xrui").resolvedStyle;
            var ratio = GetGreatestCommonDivisor((int) dimensions.width, (int) dimensions.height);

            if (dimensions.width == 0 || dimensions.height == 0)
            {
                throw new ArgumentException($"The UI {uiDocument.name} has invalid dimensions. Make sure to add a corresponding VR USS rule.");
            }

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
            catch (Exception e)
            {
                // do nothing
            }
            
            var plane = o.GetComponent<CurvedPlane>() ? o.GetComponent<CurvedPlane>() : o.AddComponent<CurvedPlane>();
            plane.numSegments = 512;
            plane.height = parameters.customVRPanelDimensions.Equals(Vector2.zero) ? (dimensions.height / ratio) / 10 : parameters.customVRPanelDimensions.y;
            plane.radius = parameters.customVRPanelDimensions.Equals(Vector2.zero) ? (dimensions.width / ratio) / 10 : parameters.customVRPanelDimensions.x;
            plane.useArc = parameters.bendVRPanel;
            plane.curvatureDegrees = parameters.bendVRPanel ? 60 : 0;
            plane.Generate(rt);
            if (parameters.anchorVRPanelToCamera)
            {
                o.transform.parent = Camera.main.transform;
                o.transform.localPosition = parameters.customVRPanelAnchorPosition.Equals(Vector3.zero) ? new Vector3(0, 0, .1f) : parameters.customVRPanelAnchorPosition;
                o.transform.localRotation = Quaternion.identity;
            } else
                o.transform.position = Camera.main.transform.position + Vector3.forward * 2;

            var collider = o.GetComponent<MeshCollider>() ? o.GetComponent<MeshCollider>() : o.AddComponent<MeshCollider>();
            collider.sharedMesh = plane.mesh;
        }

        static int GetGreatestCommonDivisor(int a, int b) {
            return b == 0 ? Math.Abs(a) : GetGreatestCommonDivisor(b, a % b);
        }
    }

    [Serializable]
    struct InspectorModal
    {
        [Tooltip("Name of the modal")]
        public string modalName;
        [Tooltip("Main template used as root content for the modal")]
        public VisualTreeAsset mainTemplate;
        [Tooltip("List of contents that appear in this modal in order of navigation")]
        public List<VisualTreeAsset> modalFlowList;
    }
}

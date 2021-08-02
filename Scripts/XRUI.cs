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
        
        [SerializeField]
        internal XRUIConfiguration xruiConfigurationAsset;
        
        // List of UI Elements
        [InspectorName("UI Elements")]
        public List<VisualTreeAsset> uiElements;

        // List of Modals
        [SerializeField]
        public List<InspectorModal> modals;
        
        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
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

        private void Reset()
        {
            xruiConfigurationAsset = Resources.Load<XRUIConfiguration>("DefaultXRUIConfiguration");
        }

        /// <summary>
        /// Updates the USS class of the given UIDocument to the current reality.
        /// </summary>
        /// <param name="uiDocument">The UIDocument to update.</param>
        internal static void UpdateDocumentUI(UIDocument uiDocument)
        {
            if (uiDocument.rootVisualElement != null)
            {
                uiDocument.rootVisualElement.Q(null, "xrui").EnableInClassList(GetCurrentReality(), true);
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
                case RuntimePlatform.LinuxEditor:
                case RuntimePlatform.WindowsEditor:
                    return PlayerPrefs.GetString("reality");
                default:
                    return RealityType.PC.ToString().ToLower();
            }
            return null;
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
            var container = GetXRUIFloatingElementContainer(type.ToString(), false);
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
        /// Generates a modal using the provided XRUI Modal template and appends it in the modal container.
        /// </summary>
        /// <param name="modalName">Name of the modal.</param>
        /// <param name="mainTemplate">Template to use as a base.</param>
        /// <param name="modalFlowList">List of contents that can be navigated within the modal.</param>
        /// <param name="additionalScript">User script to attach to the modal for user-defined behaviour.</param>
        public void CreateModal(string modalName, VisualTreeAsset mainTemplate,
            List<VisualTreeAsset> modalFlowList, Type additionalScript)
        {
            var container = GetXRUIFloatingElementContainer("XRUIModal", true);
            var uiDocument = container.GetComponent<UIDocument>();
            uiDocument.rootVisualElement.style.justifyContent = new StyleEnum<Justify>(Justify.Center);
            uiDocument.rootVisualElement.style.alignItems = new StyleEnum<Align>(Align.Center);
            uiDocument.rootVisualElement.style.width = new StyleLength(Length.Percent(100));
            uiDocument.rootVisualElement.style.height = new StyleLength(Length.Percent(100));
            
            // Instantiate main template
            VisualElement modalContainer = mainTemplate is null ? xruiConfigurationAsset.defaultModalTemplate.Instantiate() : mainTemplate.Instantiate();
            modalContainer.style.width = new StyleLength(Length.Percent(100));
            modalContainer.style.height = new StyleLength(Length.Percent(100));
            modalContainer.style.justifyContent = new StyleEnum<Justify>(Justify.Center);
            modalContainer.style.alignItems = new StyleEnum<Align>(Align.Center);
            uiDocument.rootVisualElement.Add(modalContainer);
            
            var xruiModal = container.AddComponent<XRUIModal>();
            xruiModal.modalFlowList = modalFlowList;
            container.AddComponent(additionalScript);
            container.transform.SetParent(container.transform);
        }

        internal void DestroyXRUIFloatingElement(GameObject obj)
        {
            Destroy(obj);
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
    }
    
    [Serializable]
    public struct InspectorModal
    {
        [Tooltip("Name of the modal")]
        public string modalName;
        [Tooltip("Main template used as root content for the modal")]
        public VisualTreeAsset mainTemplate;
        [Tooltip("List of contents that appear in this modal in order of navigation")]
        public List<VisualTreeAsset> modalFlowList;
    }
}
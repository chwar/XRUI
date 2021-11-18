using System;
using com.chwar.xrui.UIElements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.chwar.xrui
{
    #if UNITY_EDITOR
    [InitializeOnLoad]
    public class XRUIEditor : MonoBehaviour
    {
        // Registers an event handler when the class is initialized
        static XRUIEditor()
        {
            // Allows Editor UI Update
            EditorApplication.projectChanged += AdaptXRUI;
        }
        
        /// <summary>
        /// Adds the XRUI Controller to the scene.
        /// </summary>
        [MenuItem("XRUI/Add XRUI Controller", false, 1)]
        public static void AddController()
        {
            GameObject xruiGo = new GameObject() {name = "XRUI"};
            var xrui = xruiGo.AddComponent<XRUI>();
            xrui.xruiConfigurationAsset = Resources.Load<XRUIConfiguration>("DefaultXRUIConfiguration");
        }
        
        /// <summary>
        /// Adds an XRUI Grid to the scene.
        /// </summary>
        [MenuItem("XRUI/Add XRUI Grid", false, 1)]
        public static void AddGrid()
        {
            GameObject xruiGo = new GameObject() {name = "XRUI Grid"};
            var ui = xruiGo.AddComponent<UIDocument>();
            ui.panelSettings = GetXRUIConfiguration().panelSettings;
            ui.visualTreeAsset = Resources.Load<VisualTreeAsset>("XRUIRoot");
            var xrui = xruiGo.AddComponent<XRUIGridController>();
        }        

        /// <summary>
        /// Adds a Card type XRUI Element.
        /// </summary>
        [MenuItem("XRUI/Add XRUI Element/Card", false, 1)]
        public static void AddCard()
        {
            GameObject card = AddXRUIElement("XRUI Card", GetXRUIConfiguration().defaultCardTemplate);
            card.AddComponent<XRUICard>();
        }
        
        /// <summary>
        /// Adds a List type XRUI Element.
        /// </summary>
        [MenuItem("XRUI/Add XRUI Element/List", false, 2)]
        public static void AddList()
        {
            GameObject list = AddXRUIElement("XRUI List", GetXRUIConfiguration().defaultListTemplate);
            list.AddComponent<XRUIList>();
        }
        
        /// <summary>
        /// Adds a Navbar type XRUI Element.
        /// </summary>
        [MenuItem("XRUI/Add XRUI Element/Navbar", false, 3)]
        public static void AddNavbar()
        {
            GameObject navbar = AddXRUIElement("XRUI Navbar", GetXRUIConfiguration().defaultNavbarTemplate);
            navbar.AddComponent<XRUINavbar>();
        }
        
        /// <summary>
        /// Adds a Menu type XRUI Element.
        /// </summary>
        [MenuItem("XRUI/Add XRUI Element/Menu", false, 4)]
        public static void AddMenu()
        {
            GameObject menu = AddXRUIElement("XRUI Menu", GetXRUIConfiguration().defaultMenuTemplate);
            menu.AddComponent<XRUIMenu>();
        }
        
        /// <summary>
        /// Adds a custom XRUI Element.
        /// </summary>
        [MenuItem("XRUI/Add XRUI Element/Custom UI Element", false, 15)]
        private static void AddCustomElement()
        {
            GameObject element = AddXRUIElement("XRUI Element");
            element.AddComponent<XRUIElement>();
        }
        
        [MenuItem("XRUI/Switch Reality.../PC")]
        internal static void SwitchToPC()
        {
            // Switch to Windows/Linux/Mac standalone build.
            if(Application.platform == RuntimePlatform.WindowsEditor)
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
            else if(Application.platform == RuntimePlatform.LinuxEditor)
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneLinux64);
            XRUI.SetCurrentReality(XRUI.RealityType.PC);
        }
        
        [MenuItem("XRUI/Switch Reality.../AR (Android)")]
        internal static void SwitchToARAndroid()
        {
            // Switch to Android build.
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            XRUI.SetCurrentReality(XRUI.RealityType.AR);
        }
        
        [MenuItem("XRUI/Switch Reality.../AR (iOS)")]
        internal static void SwitchToARiOS()
        {
            // Switch to iOS build.
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS); 
            XRUI.SetCurrentReality(XRUI.RealityType.AR);
        }
        
        [MenuItem("XRUI/Switch Reality.../VR")]
        internal static void SwitchToVR()
        {
            // Switch to Windows VR build.
            SwitchToPC();
            XRUI.SetCurrentReality(XRUI.RealityType.VR);
        }
        
        /// <summary>
        /// Adds a XRUI Element.
        /// <param name="name">The name of the created Game Object</param>
        /// <param name="template">The UI template to use</param>
        /// </summary>
        private static GameObject AddXRUIElement(string name, VisualTreeAsset template = null)
        {
            // TODO Check if root XRUI GO exists and add the element there
            GameObject uiElement = new GameObject {name = name};
            var uiDocument = uiElement.AddComponent<UIDocument>();
            uiDocument.visualTreeAsset = template;
            uiDocument.panelSettings = GetXRUIConfiguration().panelSettings;
            return uiElement;
        }

        private static XRUIConfiguration GetXRUIConfiguration()
        {
            return FindObjectOfType<XRUI>() ? FindObjectOfType<XRUI>().xruiConfigurationAsset : Resources.Load<XRUIConfiguration>("DefaultXRUIConfiguration");
        }

        private static void AdaptXRUI()
        {
            foreach (var uiDocument in FindObjectsOfType<XRUIElement>())
            {
                // Update USS
                //XRUI.UpdateDocumentUI(uiDocument.GetComponent<UIDocument>());
                // Update Editor values
                uiDocument.UpdateUI();
            }
        }
    }
    #endif
}
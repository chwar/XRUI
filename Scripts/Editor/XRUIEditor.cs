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

        private static void AdaptXRUI()
        {
            foreach (var uiDocument in FindObjectsOfType<XRUIElement>())
            {
                // Update USS
                XRUI.UpdateDocumentUI(uiDocument.GetComponent<UIDocument>());
                // Update Editor values
                uiDocument.UpdateUI();
            }
        }

        /// <summary>
        /// Adds a Card type XRUI Element.
        /// </summary>
        /// <param name="cardTemplate">The template to use to create the Card</param>
        public static void AddCard(VisualTreeAsset cardTemplate)
        {
            GameObject card = AddXRUIElement("XR UI Card", cardTemplate);
            card.AddComponent<XRUICard>();
        }
        
        /// <summary>
        /// Adds a List type XRUI Element.
        /// </summary>
        /// <param name="listTemplate">The template to use to create the List</param>
        public static void AddList(VisualTreeAsset listTemplate)
        {
            GameObject list = AddXRUIElement("XR UI List", listTemplate);
            list.AddComponent<XRUIList>();
        }
        
        /// <summary>
        /// Adds a Navbar type XRUI Element.
        /// </summary>
        /// <param name="navbarTemplate">The template to use to create the Navbar</param>
        public static void AddNavbar(VisualTreeAsset navbarTemplate)
        {
            GameObject navbar = AddXRUIElement("XR UI Navbar", navbarTemplate);
            navbar.AddComponent<XRUINavbar>();
        }
        
        /// <summary>
        /// Adds a Menu type XRUI Element.
        /// </summary>
        /// <param name="menuTemplate">The template to use to create the Menu</param>
        public static void AddMenu(VisualTreeAsset menuTemplate)
        {
            GameObject menu = AddXRUIElement("XR UI Menu", menuTemplate);
            menu.AddComponent<XRUIMenu>();
        }
        
        /// <summary>
        /// Adds a XRUI Element.
        /// <param name="name">The name of the created Game Object</param>
        /// <param name="template">The UI template to use</param>
        /// </summary>
        private static GameObject AddXRUIElement(string name, VisualTreeAsset template)
        {
            // TODO Check if root XRUI GO exists and add the element there
            GameObject uiElement = new GameObject {name = name};
            var uiDocument = uiElement.AddComponent<UIDocument>();
            uiDocument.visualTreeAsset = template;
            uiDocument.panelSettings = Resources.Load<PanelSettings>("PanelSettings");
            return uiElement;
        }

        /// <summary>
        /// Editor method to set the current reality.
        /// Since the runtime platform is set to Editor, this sets the correct reality in the PlayerPrefs.
        /// </summary>
        /// <param name="type"></param>
        public static void SetCurrentReality(XRUI.RealityType type)
        {
            PlayerPrefs.SetString("reality", type.ToString().ToLower());
            PlayerPrefs.Save();
        }
    }
    #endif
}
// MIT License
// Copyright (c) 2021 Chris Warin
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
using com.chwar.xrui.UIElements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.chwar.xrui
{
    [InitializeOnLoad]
    public class XRUIEditor : MonoBehaviour
    {
        #if UNITY_EDITOR
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
            if (FindObjectOfType<XRUI>() is not null) return;
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
        public static void AddCustomElement()
        {
            GameObject element = AddXRUIElement("XRUI Element");
            element.AddComponent<XRUIElement>();
        }
        #endif

        /// <summary>
        /// Adds a XRUI Element.
        /// <param name="name">The name of the created Game Object</param>
        /// <param name="template">The UI template to use</param>
        /// </summary>
        internal static GameObject AddXRUIElement(string name, VisualTreeAsset template = null)
        {
            // TODO Check if root XRUI GO exists and add the element there
            GameObject uiElement = new GameObject {name = name};
            var uiDocument = uiElement.AddComponent<UIDocument>();
            uiDocument.visualTreeAsset = template;
            uiDocument.panelSettings = GetXRUIConfiguration().panelSettings;
            return uiElement;
        }

        internal static XRUIConfiguration GetXRUIConfiguration()
        {
            return FindObjectOfType<XRUI>() ? FindObjectOfType<XRUI>().xruiConfigurationAsset : Resources.Load<XRUIConfiguration>("DefaultXRUIConfiguration");
        }

        private static void AdaptXRUI()
        {
            foreach (var uiDocument in FindObjectsOfType<XRUIElement>())
            {
                uiDocument.UpdateUI();
            }
        }
    }
}
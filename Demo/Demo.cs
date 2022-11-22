// MIT License
// Copyright (c) 2021 Chris Warin
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
using System;
using System.Collections;
using com.chwar.xrui.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.chwar.xrui
{
    public class Demo : MonoBehaviour
    {
        private XRUIMenu _menu;
        private XRUICard _card;
        private XRUIList _list;
        
        IEnumerator Start()
        { 
            /* Find XRUI Elements in the scene */
            _menu = FindObjectOfType<XRUIMenu>();
            _card = FindObjectOfType<XRUICard>();
            _list = FindObjectOfType<XRUIList>();

            /* ========== XRUI Menu ========== */
            
            // Find the default menu template's main button and add a callback
            var mainBtn = _menu.GetXRUIVisualElement<Button>("xrui-menu__main-btn");
            mainBtn.clicked += () => ShowEntryContextualMenu(mainBtn);
            
            // Add lots of entries to trigger scrollview
            for (int i = 1; i <= 15; i++)
            {
                // AddElement returns the instantiated VisualElement template, which contains a UXML node called "MenuEntry"
                // Configure each button by changing its text and giving it a callback
                var menuEntry = _menu.AddElement().Q<Button>("MenuEntry");
                menuEntry.text = $"Menu entry {i}";
                // When clicking on the menu entry, the UI Element referenced in the inspector of the XRUI controller will be appended to the card
                menuEntry.clicked += () => _card.AddUIElement(XRUI.Instance.GetUIElement("TestUIElement").Instantiate(), "MainContainer");
            }

            /* ========== XRUI Alerts ========== */
            
            // Here are various examples for showing alerts with title, text, callbacks, and countdowns
            XRUI.Instance.ShowAlert(XRUI.AlertType.Primary, "Primary message.");
            yield return new WaitForSeconds(1);
            XRUI.Instance.ShowAlert(XRUI.AlertType.Success, "Success message.");
            yield return new WaitForSeconds(1);
            XRUI.Instance.ShowAlert(XRUI.AlertType.Warning, "Warning","Clicking this turns the card green!", () =>
            {
                // Find the default card template's header and make it green
                _card.GetXRUIVisualElement("xrui-card__header").AddToClassList("xrui-background--success");
            });
            yield return new WaitForSeconds(1);
            XRUI.Instance.ShowAlert(XRUI.AlertType.Danger, 	"Error", "Error with a title and message.");
            yield return new WaitForSeconds(1);
            XRUI.Instance.ShowAlert(XRUI.AlertType.Info, "Info", "This message will disappear automatically", 5);

            /* ========== XRUI List ========== */
            
            // Find the default list template's add button and add a callback
            var addBtn = _list.GetXRUIVisualElement<Button>("xrui-list__add-btn");
            addBtn.clicked += () => _list.AddElement(true).Q<Label>("Text").text = $"List element {_list.GetListCount()}";
            
            // Add a few entries to the list
            _list.AddElement(false).Q<Label>("Text").text = "List element";
            _list.AddElement(true).Q<Label>("Text").text = "Selected element";
            _list.AddElement(false).Q<Label>("Text").text = "List element";
            
            /* ========== XRUI Card ========== */
            
            // Find the default card template's close button and add a callback
            var closeBtn = _card.GetXRUIVisualElement<Button>("xrui-card__close-btn");
            closeBtn.clicked += () => _card.Show(false);
        }
        
        private void ShowEntryContextualMenu(VisualElement parentElement)
        {
            // Get the coordinates of the menu button and add an offset to its coordinates so that the contextual menu is displayed at the correct height
            var contextualMenu = XRUI.Instance.ShowContextualMenu(null, parentElement.worldBound.position + new Vector2(25,25),
                true, 50, 100);
        
            // Add class to identify currently selected visual element
            // TODO this could be done internally
            parentElement.panel.visualTree.Q(null, "xrui-menu-item--selected")?.ToggleInClassList("xrui-menu-item--selected");
            parentElement.parent.parent.parent.AddToClassList("xrui-menu-item--selected");

            // Add a few entries to the contextual menu
            
            var el = contextualMenu.AddMenuElement();
            el.Q<Label>("Text").text = "Open Modal";
            // Clicking on this entry will open a modal that is referenced in the inspector of the XRUI controller
            // The behaviour of the modal is defined by an additional script whose type we give to the API since it is outside of the XRUI package
            el.RegisterCallback<PointerDownEvent>((e) => XRUI.Instance.CreateModal("MyModal", Type.GetType("com.chwar.xrui.MyModalContent")));
            
            var el2 = contextualMenu.AddMenuElement();
            el2.Q<Label>("Text").text = "Close Menu";
            // Clicking on this entry will hide the menu
            el2.RegisterCallback<PointerDownEvent>((e) => _menu.Show(false));
        }
    }
}

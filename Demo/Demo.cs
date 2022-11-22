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
        
        IEnumerator Start()
        { 
            _menu = FindObjectOfType<XRUIMenu>();
            _card = FindObjectOfType<XRUICard>();

            /* XRUI Menu */
            var btn = _menu.GetXRUIVisualElement<Button>("xrui-menu__main-btn");
            btn.clicked += () => ShowEntryContextualMenu(btn);
            for (int i = 0; i < 10; i++)
            {
                _menu.AddElement();
            }

            /* XRUI Alerts */
            XRUI.Instance.ShowAlert(XRUI.AlertType.Primary, "Primary message.");
            yield return new WaitForSeconds(1);
            XRUI.Instance.ShowAlert(XRUI.AlertType.Success, "Success message.");
            yield return new WaitForSeconds(1);
            XRUI.Instance.ShowAlert(XRUI.AlertType.Warning, "Warning","Clicking this turns the card green!", () =>
            {
                _card.GetXRUIVisualElement("xrui-card__header").AddToClassList("xrui-background--success");
            });
            yield return new WaitForSeconds(1);
            XRUI.Instance.ShowAlert(XRUI.AlertType.Danger, 	"Error", "Error with a title and message.");
            yield return new WaitForSeconds(1);
            XRUI.Instance.ShowAlert(XRUI.AlertType.Info, "This will disappear in 3 seconds", "Info message.", 5);

            /* XRUI List */
            var list = FindObjectOfType<XRUIList>();
            var addBtn = list.GetXRUIVisualElement<Button>("xrui-list__add-btn");
            addBtn.clicked += () => 
                list.AddElement(true).Q<Label>("Text").text = $"List element {list.GetListCount()}";
            list.AddElement(false).Q<Label>("Text").text = "List element";
            list.AddElement(true).Q<Label>("Text").text = "Selected element";
            list.AddElement(false).Q<Label>("Text").text = "List element";
        }
        
        private void ShowEntryContextualMenu(VisualElement parentElement)
        {
            // Get the coordinates of the menu button and add an offset to its coordinates so that the contextual menu is displayed at the correct height
            var contextualMenu = XRUI.Instance.ShowContextualMenu(null, parentElement.worldBound.position + new Vector2(25,25),
                true, 50, 100);
        
            // Add class to identify currently selected visual element
            parentElement.panel.visualTree.Q(null, "xrui-menu-item--selected")?.ToggleInClassList("xrui-menu-item--selected");
            parentElement.parent.parent.parent.AddToClassList("xrui-menu-item--selected");

            var el = contextualMenu.AddMenuElement();
            el.Q<Label>("Text").text = "Open Modal";
            el.RegisterCallback<PointerDownEvent>((e) => XRUI.Instance.CreateModal("MyModal", Type.GetType("com.chwar.xrui.MyModalContent")));
            
            var el2 = contextualMenu.AddMenuElement();
            el2.Q<Label>("Text").text = "Close Menu";
            el2.RegisterCallback<PointerDownEvent>((e) => _menu.Show(false));
        }
    }
}

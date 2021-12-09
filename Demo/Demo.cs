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
        IEnumerator Start()
        {
            _menu = FindObjectOfType<XRUIMenu>();
            var btn = _menu.MainButton;
            btn.clicked += () => ShowEntryContextualMenu(btn);
            _menu.AddElement();
            _menu.AddElement();
            _menu.AddElement();
            _menu.AddElement();
            _menu.AddElement();
            _menu.AddElement();
            
            yield return new WaitForSeconds(1);
            // XRUI.Instance.ShowAlert(XRUI.AlertType.Primary, "Primary message.");
            // yield return new WaitForSeconds(1);
            // XRUI.Instance.ShowAlert(XRUI.AlertType.Success, "Success message.");
            // yield return new WaitForSeconds(1);
            // XRUI.Instance.ShowAlert(XRUI.AlertType.Warning, "Warning", "Warning with a title and message.");
            // yield return new WaitForSeconds(1);
            // XRUI.Instance.ShowAlert(XRUI.AlertType.Danger, 	"Error message.");
            // yield return new WaitForSeconds(1);
            // XRUI.Instance.ShowAlert(XRUI.AlertType.Info, 	"Info message.");

            var list = FindObjectOfType<XRUIList>();
            list.AddElement(false).Q<Label>("Text").text = "List element";
            list.AddElement(true).Q<Label>("Text").text = "Selected element";
            list.AddElement(false).Q<Label>("Text").text = "List element";

        }
        
        private void ShowEntryContextualMenu(VisualElement parentElement)
        {
            var contextualMenu = XRUI.Instance.ShowContextualMenu(null, new Vector2(parentElement.worldBound.x, parentElement.worldBound.y),
                true, _menu.Menu.resolvedStyle.width, Single.NaN);
        
            // Add class to identify currently selected visual element
            parentElement.panel.visualTree.Q(null, "xrui__menu__item--selected")?.ToggleInClassList("xrui__menu__item--selected");
            parentElement.parent.parent.parent.AddToClassList("xrui__menu__item--selected");

            var el = contextualMenu.AddMenuElement();
            el.Q<Label>("Text").text = "Contextual action 1";
            el.RegisterCallback<PointerDownEvent>((e) => XRUI.Instance.CreateModal("MyModal", Type.GetType("com.chwar.xrui.MyModalContent")));
            
            var el2 = contextualMenu.AddMenuElement();
            el2.Q<Label>("Text").text = "Contextual action 2";
        }
    }
}

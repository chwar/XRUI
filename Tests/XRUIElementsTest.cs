// MIT License
// Copyright (c) 2021 Chris Warin
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
using System;
using System.Collections;
using com.chwar.xrui.UIElements;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace com.chwar.xrui.Tests
{
    [TestFixture]
    public class XRUIElementsTest
    {
        private bool _clicked;
        private XRUI _xrui;

        [SetUp]
        public void Init()
        {
            var go = new GameObject() {name = "XRUI"};
            _xrui = go.AddComponent<XRUI>();
            _xrui.xruiConfigurationAsset = Resources.Load<XRUIConfiguration>("DefaultXRUIConfiguration");
            go.AddComponent<Camera>();
            _clicked = false;
            Debug.Log("XRUI Initialized");
        }

        private void XRUIElementClicked()
        {
            _clicked = true;
        }

        [Test]
        public void XRUIElementTestMenu()
        {
            XRUIEditor.AddMenu();
            _xrui.InitializeElements();
            var menu = GameObject.FindObjectOfType<XRUIMenu>();
            menu.menuElementTemplate = Resources.Load<VisualTreeAsset>("TestUIElement");
            var e = menu.AddElement();
            Assert.NotNull(e);
        }
        
        [Test]
        public void XRUIElementTestList()
        {
            XRUIEditor.AddList();
            _xrui.InitializeElements();
            var list = GameObject.FindObjectOfType<XRUIList>();
            list.listElementTemplate = Resources.Load<VisualTreeAsset>("TestUIElement");
            var e = list.AddElement(true);
            Assert.NotNull(e);
            Assert.True(e.ElementAt(0).ClassListContains("xrui__list__item--selected"));
        }
        
        [Test]
        public void XRUIElementTestCardDimensions()
        {
            XRUIEditor.AddCard();
            _xrui.InitializeElements();
            var card = GameObject.FindObjectOfType<XRUICard>();
            card.UpdateDimensions(new Vector2(0, 500));
            Assert.True(card.GetComponent<UIDocument>().rootVisualElement.Q("Card")
                .style.height.Equals(new StyleLength(500)));
        }
        
        [UnityTest]
        public IEnumerator XRUIElementTestCardCloseButtonAction()
        {
            XRUIEditor.AddCard();
            var card = GameObject.FindObjectOfType<XRUICard>();
            _xrui.InitializeElements();
            yield return new WaitForSeconds(1);
            card.UpdateTitle("Click on the close button");
            card.SetCloseButtonAction(XRUIElementClicked);
            yield return new WaitUntil(() => _clicked);
            Assert.True(_clicked);
        }
        
                
        [Test]
        public void XRUIElementTestCardShow()
        {
            XRUIEditor.AddCard();
            _xrui.InitializeElements();
            var card = GameObject.FindObjectOfType<XRUICard>();
            card.Show(false);
            Assert.True(card.GetComponent<UIDocument>().rootVisualElement.style.display.value.Equals(DisplayStyle.None));
        }
        
        [Test]
        public void XRUIElementTestXRUIElementAddUIElement()
        {
            XRUIEditor.AddCard();
            _xrui.InitializeElements();
            var card = GameObject.FindObjectOfType<XRUICard>();
            card.AddUIElement(Resources.Load<VisualTreeAsset>("TestUIElement").Instantiate(), "MainContainer");
            Assert.NotNull(card.GetComponent<UIDocument>().rootVisualElement.Q("MainContainer").ElementAt(0));
        }
        
        [Test]
        public void XRUIElementTestXRUIElementAddUIElementInNonExistingContainer()
        {
            XRUIEditor.AddCard();
            _xrui.InitializeElements();
            var card = GameObject.FindObjectOfType<XRUICard>();
            Assert.Throws<ArgumentException>(() => card.AddUIElement(Resources.Load<VisualTreeAsset>("TestUIElement").Instantiate(), "NonExistingContainer"));
        }
        
        [Test]
        public void XRUIElementTestXRUIElementRemoveUIElement()
        {
            XRUIEditor.AddCard();
            _xrui.InitializeElements();
            var card = GameObject.FindObjectOfType<XRUICard>();
            var element = Resources.Load<VisualTreeAsset>("TestUIElement").Instantiate();
            card.AddUIElement(element, "MainContainer");
            card.RemoveUIElement(element);
            Assert.True(card.GetComponent<UIDocument>().rootVisualElement.Q("MainContainer").childCount == 0);
        }
        
        [Test]
        public void XRUIElementTestXRUIElementEnableEmptyElement()
        {
            XRUIEditor.AddCard();
            _xrui.InitializeElements();
            var card = GameObject.FindObjectOfType<XRUICard>();
            var ui = card.GetComponent<UIDocument>().rootVisualElement;

            card.enabled = false;
            card.RemoveUIElement(ui.ElementAt(0));
            try
            {
                card.enabled = true;
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e);
                throw;
            }
            // Fires OnEnable which throws a NullReferenceException, but can't be tested
            Assert.Pass();
        }
    }
}
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

        [OneTimeSetUp]
        public void Init()
        {
            var go = new GameObject() { name = "XRUI" };
            go.AddComponent<XRUI>();
            go.AddComponent<Camera>().tag = "MainCamera";
            XRUI.Instance.xruiConfigurationAsset = Resources.Load<XRUIConfiguration>("DefaultXRUI2DConfiguration");
            Debug.Log("XRUI Initialized");
        }

        private void XRUIElementClicked()
        {
            _clicked = true;
        }

        [Test]
        public void XRUIElementTestMenu()
        {
            GameObject menuGo = XRUIEditor.AddXRUIElement("XRUI Menu", XRUIEditor.GetXRUIConfiguration().defaultMenuTemplate);
            var menu = menuGo.AddComponent<XRUIMenu>();
            XRUI.Instance.InitializeElements();
            menu.menuElementTemplate = Resources.Load<VisualTreeAsset>("TestUIElement");
            var e = menu.AddElement();
            Assert.NotNull(e);
            menu.RemoveAllElements();
            Assert.Catch<ArgumentOutOfRangeException>(() => 
                menu.RootElement.Q(null, "xrui-menu__container").ElementAt(0));
        }
        
        [Test]
        public void XRUIElementTestList()
        {
            XRUIEditor.AddList();
            XRUI.Instance.InitializeElements();
            var list = GameObject.FindObjectOfType<XRUIList>();
            list.listElementTemplate = Resources.Load<VisualTreeAsset>("TestUIElement");
            var e = list.AddElement(true);
            Assert.NotNull(e);
            Assert.True(e.ElementAt(0).ClassListContains("xrui-list-item--selected"));
        }
        
        [Test]
        public void XRUIElementTestListCount()
        {
            XRUIEditor.AddList();
            XRUI.Instance.InitializeElements();
            var list = GameObject.FindObjectOfType<XRUIList>();
            list.listElementTemplate = Resources.Load<VisualTreeAsset>("TestUIElement");
            list.AddElement(true);
            list.AddElement(false);
            Assert.True(list.GetListCount().Equals(2));
            list.RemoveAllElements();
            Assert.True(list.GetListCount().Equals(0));
        }
        
        [UnityTest]
        public IEnumerator XRUIElementTestListWithCallback()
        {
            _clicked = false;
            XRUIEditor.AddList();
            XRUI.Instance.InitializeElements();
            var list = GameObject.FindObjectOfType<XRUIList>();
            list.listElementTemplate = Resources.Load<VisualTreeAsset>("TestUIElement");
            list.UpdateTitle("Click inside the blue zone");
            var e = list.AddElement(true, (_) => XRUIElementClicked());
            yield return new WaitUntil(() => _clicked);
        }
        
        // [UnityTest]
        // public IEnumerator XRUIElementTestCardDimensions()
        // {
        //     XRUIEditor.AddCard();
        //     XRUI.Instance.InitializeElements();
        //     var card = GameObject.FindObjectOfType<XRUICard>();
        //     card.UpdateDimensions(new Vector2(0, 500));
        //     yield return new WaitForEndOfFrame();
        //     Assert.True(card.RootElement.style.height.Equals(new StyleLength(500)));
        // }
        
        [UnityTest]
        public IEnumerator XRUIElementTestCardCloseButtonAction()
        {
            XRUIEditor.AddCard();
            var card = GameObject.FindObjectOfType<XRUICard>();
            XRUI.Instance.InitializeElements();
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
            XRUI.Instance.InitializeElements();
            var card = GameObject.FindObjectOfType<XRUICard>();
            card.Show(false);
            Assert.True(card.RootElement.ClassListContains("xrui--hide"));
        }
        
        [UnityTest]
        public IEnumerator XRUIElementTestCardShowWorldUI()
        {
            XRUIEditor.AddCard();
            var card = GameObject.FindObjectOfType<XRUICard>();
            card.xruiFormatOverride = XRUIFormatOverride.ThreeDimensional;
            XRUI.Instance.InitializeElements();
            yield return new WaitForEndOfFrame();

            card.Show(false);
            Assert.True(card.RootElement.ClassListContains("xrui--hide"));
            Assert.False(card.GetComponent<MeshRenderer>().enabled);
            Assert.False(card.GetComponent<MeshCollider>().enabled);
        }
        
        [Test]
        public void XRUIElementTestXRUIElementAddUIElement()
        {
            XRUIEditor.AddCard();
            XRUI.Instance.InitializeElements();
            var card = GameObject.FindObjectOfType<XRUICard>();
            Assert.Catch<ArgumentOutOfRangeException>(()=> card.RootElement.Q(null, "xrui-card__container").ElementAt(0));
            card.AddUIElement(Resources.Load<VisualTreeAsset>("TestUIElement").Instantiate(), "xrui-card__container");
            Assert.NotNull(card.RootElement.Q(null, "xrui-card__container").ElementAt(0));
        }
        
        [Test]
        public void XRUIElementTestXRUIElementAddUIElementInNonExistingContainer()
        {
            XRUIEditor.AddCard();
            XRUI.Instance.InitializeElements();
            var card = GameObject.FindObjectOfType<XRUICard>();
            Assert.Throws<ArgumentException>(() => card.AddUIElement(Resources.Load<VisualTreeAsset>("TestUIElement").Instantiate(), "NonExistingContainer"));
        }
        
        [Test]
        public void XRUIElementTestXRUIElementRemoveUIElement()
        {
            XRUIEditor.AddCard();
            XRUI.Instance.InitializeElements();
            var card = GameObject.FindObjectOfType<XRUICard>();
            var element = Resources.Load<VisualTreeAsset>("TestUIElement").Instantiate();
            card.AddUIElement(element, "xrui-card__container");
            card.RemoveUIElement(element);
            Assert.True(card.GetXRUIVisualElement("xrui-card__container").childCount == 0);
        }
        
        [Test]
        public void XRUIElementTestXRUIElementEnableEmptyElement()
        {
            XRUIEditor.AddCard();
            XRUI.Instance.InitializeElements();
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
        
        [Test]
        public void XRUIElementTestFormatUseGlobal()
        {
            XRUIEditor.AddCard();
            XRUI.Instance.InitializeElements();
            var card = GameObject.FindObjectOfType<XRUICard>();
            
            XRUI.Instance.SetGlobalXRUIFormat(XRUI.XRUIFormat.ThreeDimensional);
            
            card.xruiFormatOverride = XRUIFormatOverride.UseGlobal;
            Assert.True(card.IsXRUIFormat(XRUI.XRUIFormat.ThreeDimensional));
            Assert.False(card.IsXRUIFormat(XRUI.XRUIFormat.TwoDimensional));
        }
        
        [Test]
        public void XRUIElementTestFormatTwoDimensional()
        {
            XRUIEditor.AddCard();
            XRUI.Instance.InitializeElements();
            var card = GameObject.FindObjectOfType<XRUICard>();
            card.xruiFormatOverride = XRUIFormatOverride.TwoDimensional;
            Assert.True(card.IsXRUIFormat(XRUI.XRUIFormat.TwoDimensional));
            Assert.False(card.IsXRUIFormat(XRUI.XRUIFormat.ThreeDimensional));
        }
        
        [Test]
        public void XRUIElementTestFormatThreeDimensional()
        {
            XRUIEditor.AddCard();
            XRUI.Instance.InitializeElements();
            var card = GameObject.FindObjectOfType<XRUICard>();
            card.xruiFormatOverride = XRUIFormatOverride.ThreeDimensional;
            Assert.True(card.IsXRUIFormat(XRUI.XRUIFormat.ThreeDimensional));
            Assert.False(card.IsXRUIFormat(XRUI.XRUIFormat.TwoDimensional));
        }

        [UnityTest]
        public IEnumerator XRUIElementTestFollowCamera()
        {
            var cam = Camera.main;
            XRUIEditor.AddCard();
            var card = GameObject.FindObjectOfType<XRUICard>();
            
            // Position far outside view frustum
            card.transform.position = new Vector3(-5, -5, -5);
            card.worldUIParameters.anchorPanelToCamera = true;
            card.xruiFormatOverride = XRUIFormatOverride.ThreeDimensional;
            XRUI.Instance.InitializeElements();

            yield return new WaitForEndOfFrame();

            yield return new WaitForSeconds(2);
           
            // Assert that card is in front of camera
            Assert.True(card.transform.position.Equals(cam.transform.position + Vector3.forward));
        }
    }
}
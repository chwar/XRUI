// MIT License
// Copyright (c) 2021 Chris Warin
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace com.chwar.xrui.Tests
{
    [TestFixture]
    public class XRUITest
    {
        private XRUI _xrui;
        
        [OneTimeSetUp]
        public void Init()
        {
            if (GameObject.FindObjectOfType<XRUI>() is not null) return;
            _xrui = new GameObject() {name = "XRUI"}.AddComponent<XRUI>();
            _xrui.gameObject.AddComponent<Camera>().tag = "MainCamera";
            _xrui.xruiConfigurationAsset = Resources.Load<XRUIConfiguration>("DefaultXRUIConfiguration");
            Debug.Log("XRUI Initialized");
        }
        
        [Test]
        public void XRUITestCreateConfiguration()
        {
            XRUIConfiguration c = ScriptableObject.CreateInstance<XRUIConfiguration>();
            c.Reset();
            Assert.NotNull(c.panelSettings);
            Assert.NotNull(c.defaultAlertTemplate);
            Assert.NotNull(c.defaultModalTemplate);
            Assert.NotNull(c.defaultCardTemplate);
            Assert.NotNull(c.defaultMenuTemplate);
            Assert.NotNull(c.defaultListTemplate);
            Assert.NotNull(c.defaultNavbarTemplate);
            Assert.NotNull(c.defaultContextualMenuTemplate);
        }

        [Test]
        public void XRUITestGetNonExistingUIElement()
        {
            Assert.Throws<ArgumentException>(() => XRUI.Instance.GetUIElement("NonExistingTemplate"));
        }

        [Test]
        public void XRUITestGetExistingUIElement()
        {
            XRUI.Instance.uiElements.Add(Resources.Load<VisualTreeAsset>("TestUIElement"));
            Assert.NotNull(XRUI.Instance.GetUIElement("TestUIElement"));
        }

        [UnityTest]
        public IEnumerator XRUITestWorldUIPanel()
        {
            XRUI.Instance.SetCurrentXRUIFormat(XRUI.XRUIFormat.ThreeDimensional);
            XRUIEditor.AddCard();
            XRUI.Instance.InitializeElements();

            yield return new WaitForSeconds(1);
            var panel = GameObject.FindObjectOfType<CurvedPlane>();
            Assert.NotNull(panel);
        }

        [Test]
        public void XRUITestFormat()
        {
            XRUI.Instance.SetCurrentXRUIFormat(XRUI.XRUIFormat.ThreeDimensional);
            Assert.True(XRUI.Instance.GetCurrentXRUIFormat().Equals(XRUI.Instance.xruiFormat.ToString().ToLower()));
            Assert.True(XRUI.Instance.IsCurrentXRUIFormat(XRUI.XRUIFormat.ThreeDimensional));
        }
    }
}
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
    public class XRUITest
    {
        [SetUp]
        public void Init()
        {
            var go = new GameObject() {name = "Modal"};
            var xrui = go.AddComponent<XRUI>();
            go.AddComponent<Camera>().tag = "MainCamera";
            xrui.xruiConfigurationAsset = Resources.Load<XRUIConfiguration>("DefaultXRUIConfiguration");
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

        [Test]
        public void XRUITestRealityOnApplicationQuit()
        {
            var xrui = GameObject.FindObjectOfType<XRUI>();
            xrui.realityType = XRUI.RealityType.AR;
            xrui.OnApplicationQuit();
            Assert.True(XRUI.IsCurrentReality(XRUI.RealityType.UseGlobal));
            XRUI.SetCurrentReality(XRUI.RealityType.PC);
        }

        [UnityTest]
        public IEnumerator XRUITestVRPanel()
        {
            // Create Card
            GameObject uiElement = new GameObject();
            var uiDocument = uiElement.AddComponent<UIDocument>();
            XRUIConfiguration c = ScriptableObject.CreateInstance<XRUIConfiguration>();
            c.Reset();
            uiDocument.panelSettings = c.panelSettings;
            uiDocument.visualTreeAsset = c.defaultCardTemplate;
            uiElement.AddComponent<XRUICard>();

            // Set Reality to VR to trigger VRPanel method
            var xrui = uiDocument.rootVisualElement.Q(null, "xrui");
            GeometryChangedEvent evt = new GeometryChangedEvent();
            evt.target = xrui;
            yield return new WaitForSeconds(1);
            try
            {
                XRUI.GetVRPanel(evt, uiDocument);
            }
            catch (Exception)
            {
                // Avoid internal UI Toolkit error when applying Panel Settings
            }

            var panel = GameObject.FindObjectOfType<CurvedPlane>();
            Assert.NotNull(panel);
        }
    }
}
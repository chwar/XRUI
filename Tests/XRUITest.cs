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
            catch (Exception e)
            {
                // Avoid internal UI Toolkit error when applying Panel Settings
            }

            var panel = GameObject.FindObjectOfType<CurvedPlane>();
            Assert.NotNull(panel);
        }
    }
}
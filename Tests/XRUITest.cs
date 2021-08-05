using System;
using NUnit.Framework;
using UnityEngine;
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
    }
}
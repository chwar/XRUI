using System;
using com.chwar.xrui.UIElements;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.chwar.xrui.Tests.Editor
{
    [TestFixture]
    public class XRUIEditorTest
    {
        [SetUp]
        public void Init()
        {
            XRUIEditor.AddController();
        }
        
        [Test]
        public void XRUIEditorTestAddController()
        {
            var xrui = GameObject.FindObjectOfType<XRUI>();
            Assert.NotNull(xrui);
        }

        [Test]
        public void XRUIEditorTestAddGrid()
        {
            XRUIEditor.AddGrid();
            var xrui = GameObject.FindObjectOfType<XRUIGridController>();
            Assert.NotNull(xrui);
        }
        
        [Test]
        public void XRUIEditorTestAddCard()
        {
            XRUIEditor.AddCard();
            var xrui = GameObject.FindObjectOfType<XRUICard>();
            Assert.NotNull(xrui);
        }
        
        [Test]
        public void XRUIEditorTestAddList()
        {
            XRUIEditor.AddList();
            var xrui = GameObject.FindObjectOfType<XRUIList>();
            Assert.NotNull(xrui);
        }
        
        [Test]
        public void XRUIEditorTestAddNavbar()
        {
            XRUIEditor.AddNavbar();
            var xrui = GameObject.FindObjectOfType<XRUINavbar>();
            Assert.NotNull(xrui);
        }
        
        [Test]
        public void XRUIEditorTestAddMenu()
        {
            XRUIEditor.AddMenu();
            var xrui = GameObject.FindObjectOfType<XRUIMenu>();
            Assert.NotNull(xrui);
        }

        [Test]
        public void XRUIEditorTestAddCustomElement()
        {
            XRUIEditor.AddCustomElement();
            var xrui = GameObject.FindObjectOfType<XRUIElement>();
            var uid = xrui.gameObject.GetComponent<UIDocument>();
            uid.visualTreeAsset = Resources.Load<VisualTreeAsset>("TestUIElement");
            Assert.NotNull(xrui);
        }

        [Test]
        public void XRUIEditorTestSwitchToPC()
        {
            XRUIEditor.SwitchToPC();
            Assert.True(XRUI.IsCurrentReality(XRUI.RealityType.PC));
        }
        
        [Test]
        public void XRUIEditorTestSwitchToARAndroid()
        {
            XRUIEditor.SwitchToARAndroid();
            Assert.True(XRUI.IsCurrentReality(XRUI.RealityType.AR));
        }
        
        [Test]
        public void XRUIEditorTestSwitchToVR()
        {
            XRUIEditor.SwitchToVR();
            Assert.True(XRUI.IsCurrentReality(XRUI.RealityType.VR));
        }
    }
}
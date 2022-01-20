// MIT License
// Copyright (c) 2021 Chris Warin
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
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

        /*[Test]
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
        }*/
    }
}
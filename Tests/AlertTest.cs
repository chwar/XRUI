// MIT License
// Copyright (c) 2021 Chris Warin
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
using System.Collections;
using com.chwar.xrui.UIElements;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace com.chwar.xrui.Tests
{
    [TestFixture]
    public class AlertTest
    {
        private GameObject _go;
        private bool _clicked;
        
        [SetUp]
        public void Init()
        {
            _go = new GameObject() {name = "XRUI"};
            var xrui = _go.AddComponent<XRUI>();
            _go.AddComponent<Camera>().tag = "MainCamera";
            _clicked = false;
            xrui.xruiConfigurationAsset = Resources.Load<XRUIConfiguration>("DefaultXRUI2DConfiguration");
            Debug.Log("XRUI Initialized");
        }

        [TearDown]
        public void Cleanup()
        {
            GameObject.DestroyImmediate(_go);
            _clicked = false;
            var alert = GameObject.FindObjectOfType<XRUIAlert>();
            // Already deleted when clicked upon
            if(alert is not null)
                GameObject.DestroyImmediate(alert.gameObject);
        }

        [Test]
        public void AlertTestCreatePrimaryAlert()
        {
            XRUI.Instance.ShowAlert(XRUIAlert.AlertType.Primary, "Test");
            var alert = Object.FindObjectOfType<XRUIAlert>();
            Assert.NotNull(alert);
            Assert.True(alert.RootElement.ClassListContains("primary"));
        }
        
        [Test]
        public void AlertTestCreateWarningAlert()
        {
            XRUI.Instance.ShowAlert(XRUIAlert.AlertType.Warning, "Test");
            var alert = Object.FindObjectOfType<XRUIAlert>();
            Assert.NotNull(alert);
            Assert.True(alert.RootElement.ClassListContains("warning"));
        }
        
        [Test]
        public void AlertTestCreateSuccessAlert()
        {
            XRUI.Instance.ShowAlert(XRUIAlert.AlertType.Success, "Test");
            var alert = Object.FindObjectOfType<XRUIAlert>();
            Assert.NotNull(alert);
            Assert.True(alert.RootElement.ClassListContains("success"));
        }
        
        [Test]
        public void AlertTestCreateInfoAlert()
        {
            XRUI.Instance.ShowAlert(XRUIAlert.AlertType.Info, "Test");
            var alert = Object.FindObjectOfType<XRUIAlert>();
            Assert.NotNull(alert);
            Assert.True(alert.RootElement.ClassListContains("info"));
        }
        
        [Test]
        public void AlertTestCreateDangerAlert()
        {
            XRUI.Instance.ShowAlert(XRUIAlert.AlertType.Danger, "Test");
            var alert = Object.FindObjectOfType<XRUIAlert>();
            Assert.NotNull(alert);
            Assert.True(alert.RootElement.ClassListContains("danger"));
        }

        [Test]
        public void AlertTestCreateAlertWithTitle()
        {
            XRUI.Instance.ShowAlert(XRUIAlert.AlertType.Primary, "Title","Test");
            var alert = Object.FindObjectOfType<XRUIAlert>();
            Assert.NotNull(alert);
            Assert.NotNull(alert.Title);
        }

        [UnityTest]
        public IEnumerator AlertTestWithCallback()
        {
            XRUI.Instance.ShowAlert(XRUIAlert.AlertType.Primary, "Click me!", "Click to trigger callback", ()=> AlertClick());
            yield return new WaitUntil(() => _clicked);
            // Wait for animation
            yield return new WaitForSeconds(1.5f);
            Assert.Null(GameObject.Find("PrimaryAlert"));
        }

        [UnityTest]
        public IEnumerator AlertTestWithCountdown()
        {
            XRUI.Instance.ShowAlert(XRUIAlert.AlertType.Primary, null, "This will disappear in one second", 1);
            yield return new WaitForSeconds(1.5f);
            Assert.Null(GameObject.Find("PrimaryAlert"));
        }
        
        [UnityTest]
        public IEnumerator WorldAlertTestWithCountdown()
        {
            XRUI.Instance.SetCurrentXRUIFormat(XRUI.XRUIFormat.ThreeDimensional);
            XRUI.Instance.ShowAlert(XRUIAlert.AlertType.Primary, null, "This will disappear in one second", 1);
            yield return new WaitForSeconds(2f);
            Assert.Null(GameObject.Find("PrimaryAlert"));
        }
        
        private void AlertClick(PointerDownEvent evt = null)
        {
            _clicked = true;
        }
    }
}
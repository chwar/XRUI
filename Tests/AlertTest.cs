using System.Collections;
using com.chwar.xrui.UIElements;
using NUnit.Framework;
using UnityEditor;
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
            _clicked = false;
            xrui.xruiConfigurationAsset = Resources.Load<XRUIConfiguration>("DefaultXRUIConfiguration");
            Debug.Log("XRUI Initialized");
        }

        [TearDown]
        public void Cleanup()
        {
            GameObject.DestroyImmediate(_go);
            var alert = GameObject.FindObjectOfType<XRUIAlert>();
            // Already deleted when clicked upon
            if(alert is not null)
                GameObject.DestroyImmediate(alert.gameObject);
        }

        [Test]
        public void AlertTestCreatePrimaryAlert()
        {
            XRUI.Instance.ShowAlert(XRUI.AlertType.Primary, "Test");
            var alert = Object.FindObjectOfType<XRUIAlert>();
            Assert.NotNull(alert);
            Assert.True(alert.Alert.ClassListContains("primary"));
        }
        
        [Test]
        public void AlertTestCreateWarningAlert()
        {
            XRUI.Instance.ShowAlert(XRUI.AlertType.Warning, "Test");
            var alert = Object.FindObjectOfType<XRUIAlert>();
            Assert.NotNull(alert);
            Assert.True(alert.Alert.ClassListContains("warning"));
        }
        
        [Test]
        public void AlertTestCreateSuccessAlert()
        {
            XRUI.Instance.ShowAlert(XRUI.AlertType.Success, "Test");
            var alert = Object.FindObjectOfType<XRUIAlert>();
            Assert.NotNull(alert);
            Assert.True(alert.Alert.ClassListContains("success"));
        }
        
        [Test]
        public void AlertTestCreateInfoAlert()
        {
            XRUI.Instance.ShowAlert(XRUI.AlertType.Info, "Test");
            var alert = Object.FindObjectOfType<XRUIAlert>();
            Assert.NotNull(alert);
            Assert.True(alert.Alert.ClassListContains("info"));
        }
        
        [Test]
        public void AlertTestCreateDangerAlert()
        {
            XRUI.Instance.ShowAlert(XRUI.AlertType.Danger, "Test");
            var alert = Object.FindObjectOfType<XRUIAlert>();
            Assert.NotNull(alert);
            Assert.True(alert.Alert.ClassListContains("danger"));
        }

        [Test]
        public void AlertTestCreateAlertWithTitle()
        {
            XRUI.Instance.ShowAlert(XRUI.AlertType.Primary, "Title","Test");
            var alert = Object.FindObjectOfType<XRUIAlert>();
            Assert.NotNull(alert);
            Assert.NotNull(alert.Title);
        }

        [UnityTest]
        public IEnumerator AlertTestDestroyAlert()
        {
            XRUI.Instance.ShowAlert(XRUI.AlertType.Primary, "Click me!");
            var alert = Object.FindObjectOfType<XRUIAlert>();
            alert.Alert.RegisterCallback<PointerDownEvent>(AlertClick);
            yield return new WaitUntil(() => _clicked);
            // Wait for animation
            yield return new WaitForSeconds(1.5f);
            Assert.Null(GameObject.Find("PrimaryAlert"));
        }

        private void AlertClick(PointerDownEvent evt)
        {
            _clicked = true;
        }
    }
}
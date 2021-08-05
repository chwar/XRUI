using com.chwar.xrui.UIElements;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace com.chwar.xrui.Tests
{
    [TestFixture]
    public class AlertTest
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
    }
}
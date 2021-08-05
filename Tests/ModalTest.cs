using System;
using System.Collections;
using System.Collections.Generic;
using com.chwar.xrui.UIElements;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace com.chwar.xrui.Tests
{
    [TestFixture]
    public class ModalTest
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
        public void ModalTestAddModalWithExistingTemplate()
        {
            InspectorModal m = new InspectorModal();
            m.modalName = "TestModal";
            XRUI.Instance.modals = new List<InspectorModal> {m};
            XRUI.Instance.CreateModal("TestModal", null);
            var modal = Object.FindObjectOfType<XRUIModal>();
            Assert.NotNull(modal);
        }
        
        [Test]
        public void ModalTestAddModalWithNonExistingTemplate()
        {
            Assert.Throws<ArgumentException>(() => XRUI.Instance.CreateModal("NonExistingModal", null));
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator ModalTestWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}

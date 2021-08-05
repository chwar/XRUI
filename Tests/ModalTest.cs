using System;
using System.Collections;
using System.Collections.Generic;
using com.chwar.xrui.UIElements;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace com.chwar.xrui.Tests
{
    [TestFixture]
    public class ModalTest
    {
        private XRUIModal _modal;
        private bool _btnClicked;
        
        [OneTimeSetUp]
        public void Init()
        {
            var go = new GameObject() {name = "Modal"};
            var xrui = go.AddComponent<XRUI>();
            xrui.xruiConfigurationAsset = Resources.Load<XRUIConfiguration>("DefaultXRUIConfiguration");
            Debug.Log("XRUI Initialized");

            InspectorModal m = new InspectorModal
            {
                modalName = "TestModal", 
                modalFlowList = new List<VisualTreeAsset>(){Resources.Load<VisualTreeAsset>("TestUIElement")}
            };
            XRUI.Instance.modals.Add(m);
            XRUI.Instance.CreateModal("TestModal", null);
            _modal = Object.FindObjectOfType<XRUIModal>();
        }
        
        [Test]
        public void ModalTestAddModalWithExistingTemplate()
        {
            Assert.NotNull(_modal);
        }
        
        [Test]
        public void ModalTestAddModalWithNonExistingTemplate()
        {
            Assert.Throws<ArgumentException>(() => XRUI.Instance.CreateModal("NonExistingModal", null));
        }

        [Test]
        public void ModalTestSetButtonsPlacement()
        {
            _modal.SetButtonsPlacement(Justify.Center);
            Assert.True(_modal.ValidateButton.parent.style.justifyContent.value.Equals(Justify.Center));
            Assert.True(_modal.CancelButton.parent.style.justifyContent.value.Equals(Justify.Center));
        }

        [UnityTest]
        public IEnumerator ModalTestSetCancelButtonAction()
        {
            _modal.ModalTitle.text = "Click on the Cancel button.";
            _modal.SetCancelButtonAction(ModalButtonClicked);
            yield return new WaitUntil(() => this._btnClicked);
            Assert.True(this._btnClicked);
            _btnClicked = false;
        }
        
        [UnityTest]
        public IEnumerator ModalTestSetValidateButtonAction()
        {
            _modal.ModalTitle.text = "Click on the Validate button.";
            _modal.SetValidateButtonAction(ModalButtonClicked);
            yield return new WaitUntil(() => this._btnClicked);
            Assert.True(this._btnClicked);
            _btnClicked = false;
        }

        public void ModalButtonClicked()
        {
            this._btnClicked = true;
        }
    }
}

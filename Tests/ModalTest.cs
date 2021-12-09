// MIT License
// Copyright (c) 2021 Chris Warin
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
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
        private GameObject _go;
        private XRUIModal _modal;
        private UIDocument _ui;
        private bool _clicked;
        
        [SetUp]
        public void Init()
        {
            _go = new GameObject() {name = "XRUI"};
            var xrui = _go.AddComponent<XRUI>();
            _go.AddComponent<Camera>();
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
            _ui = _modal.GetComponent<UIDocument>();
            _clicked = false;
        }

        [TearDown]
        public void Cleanup()
        {
            GameObject.DestroyImmediate(_go);
            GameObject.DestroyImmediate(_modal.gameObject);
        }
        
        
        public void ModalClicked()
        {
            this._clicked = true;
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
            _modal.SetCancelButtonAction(ModalClicked);
            yield return new WaitUntil(() => this._clicked);
            Assert.True(this._clicked);
            _clicked = false;
        }
        
        [UnityTest]
        public IEnumerator ModalTestSetValidateButtonAction()
        {
            _modal.ModalTitle.text = "Click on the Validate button.";
            _modal.SetValidateButtonAction(ModalClicked);
            yield return new WaitUntil(() => this._clicked);
            Assert.True(this._clicked);
            _clicked = false;
        }

        [Test]
        public void ModalTestUpdateModalFlow()
        {
            _modal.UpdateModalFlow("TestUIElement", "MainContainer", () =>
            {
                
            });
            Assert.True(_ui.rootVisualElement.Q("MainContainer")
                .ElementAt(0).name.Equals(_modal.modalFlowList[0].name));
        }
        
        [Test]
        public void ModalTestUpdateModalFlowTwice()
        {
            _modal.UpdateModalFlow("TestUIElement", "MainContainer", () =>
            {
                _modal.ModalTitle.text = "Page One";
            });
            _modal.UpdateModalFlow("TestUIElement", "MainContainer", () =>
            {
                _modal.ModalTitle.text = "Page Two";
            });
        }
        
        [Test]
        public void ModalTestUpdateModalFlowWithNonExistingContent()
        {
            Assert.Throws<ArgumentException>(() => _modal.UpdateModalFlow("NonExistingPage", "MainContainer", null));
        }
        
        [Test]
        public void ModalTestUpdateModalFlowWithNonExistingContainer()
        {
            Assert.Throws<ArgumentException>(() => _modal.UpdateModalFlow("TestUIElement", "NonExistingContainer", null));
        }
        
        [Test]
        public void ModalTestSetRequiredFields()
        {
            _modal.UpdateModalFlow("TestUIElement", "MainContainer", () =>
            {
                var field = _ui.rootVisualElement.Q<TextField>();
                field.value = "";
                _modal.SetRequiredFields(field);
            });
            Assert.False(_modal.ValidateButton.enabledSelf);
        }
        
        [UnityTest]
        public IEnumerator ModalTestSetRequiredFieldsWithUserCheck()
        {
            _modal.UpdateModalFlow("TestUIElement", "MainContainer", () =>
            {
                _modal.ModalTitle.text = "Change the value of the required field.";
                var field = _ui.rootVisualElement.Q<TextField>();
                field.value = "";
                _modal.SetRequiredFields(field);
                field.RegisterCallback<InputEvent>(_ => ModalClicked());
            });
            yield return new WaitUntil(() => _clicked);
            Assert.True(_modal.ValidateButton.enabledSelf);
        }
        
        [Test]
        public void ModalTestSetFieldError()
        {
            TextField field = null;
            _modal.UpdateModalFlow("TestUIElement", "MainContainer", () =>
            {
                field = _ui.rootVisualElement.Q<TextField>();
                _modal.SetFieldError(field);
            });
            Assert.True(field.ClassListContains("error"));
        }
        
        [UnityTest]
        public IEnumerator ModalTestClickOnErrorField()
        {
            TextField field = null;
            _modal.UpdateModalFlow("TestUIElement", "MainContainer", () =>
            {
                _modal.ModalTitle.text = "Change the value on the field marked as error.";
                field = _ui.rootVisualElement.Q<TextField>();
                _modal.SetFieldError(field);
            });
            yield return new WaitUntil(() => !field.ClassListContains("error"));
            Assert.False(field.ClassListContains("error"));
        }
    }
}

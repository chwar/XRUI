using System.Collections;
using System.Collections.Generic;
using com.chwar.xrui.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.chwar.xrui
{
    public class MyModalContent : MonoBehaviour
    {
        private XRUIModal _xruiModal;
        private UIDocument _uiDocument;
        void Start()
        {
            _xruiModal = GetComponent<XRUIModal>();
            _uiDocument = GetComponent<UIDocument>();

            StartPage();
        }

        private void StartPage()
        {
            _xruiModal.UpdateModalFlow("MyModalContent", "MainContainer", () =>
            {
                _xruiModal.SetCancelButtonAction(() => Destroy(_xruiModal.gameObject));
                _xruiModal.SetValidateButtonAction(Validate);
            });
        }

        private void Validate()
        {
            var field = _uiDocument.rootVisualElement.Q<TextField>("text");
            _xruiModal.SetFieldError(field); 
        }
    }
}
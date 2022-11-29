// MIT License
// Copyright (c) 2021 Chris Warin
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
using com.chwar.xrui.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.chwar.xrui
{
    public class MyModalContent : MonoBehaviour
    {
        private XRUIModal _xruiModal;
        
        void Start()
        {
            _xruiModal = GetComponent<XRUIModal>();
            StartPage();
        }

        private void StartPage()
        {
            _xruiModal.UpdateModalFlow("TestUIElement", "xrui-modal__container", () =>
            {
                _xruiModal.SetCancelButtonAction(() => Destroy(_xruiModal.gameObject));
                _xruiModal.SetValidateButtonAction(Validate);
            });
        }

        private void Validate()
        {
            var field = _xruiModal.RootElement.Q<TextField>(null, "unity-text-field");
            _xruiModal.SetFieldError(field); 
        }
    }
}
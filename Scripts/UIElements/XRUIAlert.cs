// MIT License
// Copyright (c) 2021 Chris Warin
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.chwar.xrui.UIElements
{
    public class XRUIAlert : XRUIFloatingElement
    {
        public Label Title { get; private set; }
        public Label Content { get; private set; }

        public Action ClickCallback;

        internal VisualElement Alert;
        
        /// <summary>
        /// Initializes the UI Elements of the Alert.
        /// </summary>
        protected internal override void Init()
        {
            base.Init();
            Title = UIDocument.rootVisualElement.Q<Label>("AlertTitle");
            Content = UIDocument.rootVisualElement.Q<Label>("AlertContent");
            
            // Set handler on click to dispose of the alert
            UIDocument.rootVisualElement.RegisterCallback<PointerDownEvent>(_ => DisposeAlert());
            Alert = UIDocument.rootVisualElement.Q(null, "xrui__alert");
            StartCoroutine(Animate());
        }

        internal void DisposeAlert()
        {
            if (PointerOverUI)
            {
                StartCoroutine(Animate());
                StartCoroutine(Dispose());
                ClickCallback?.Invoke();
            }
        }

        private IEnumerator Dispose()
        {
            yield return new WaitForSeconds(1);
            Destroy(this.gameObject);
        }
        
        private IEnumerator Animate()
        {
            yield return new WaitForFixedUpdate();
            Alert.ToggleInClassList("animate");
        }
    }
}
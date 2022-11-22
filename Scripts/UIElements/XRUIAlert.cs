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
    public class XRUIAlert : XRUIElement
    {
        public Label Title { get; private set; }
        public Label Content { get; private set; }

        public Action ClickCallback;
        
        public int countdown = 0;
        
        /// <summary>
        /// Initializes the UI Elements of the Alert.
        /// </summary>
        protected internal override void Init()
        {
            base.Init();
            Title = GetXRUIVisualElement<Label>("xrui-alert__title");
            Content = GetXRUIVisualElement<Label>("xrui-alert__content");
            
            // Set handler on click to dispose of the alert
            RootElement.RegisterCallback<PointerDownEvent>(_ => DisposeAlert(true));
            StartCoroutine(FollowCamera());
            StartCoroutine(Animate());
        }

        public void DisposeAlert(bool requirePointerOverUI = false, bool destroyImmediate = true)
        {
            if ((requirePointerOverUI && PointerOverUI) || !requirePointerOverUI)
            {
                StartCoroutine(Animate(destroyImmediate));
                StartCoroutine(Dispose(destroyImmediate));
                ClickCallback?.Invoke();
            }
        }

        private IEnumerator Dispose(bool destroyImmediate = true)
        {
            yield return new WaitForSeconds(destroyImmediate ? 1 : countdown == 0 ? 1 : countdown);
            Destroy(this.gameObject);
        }
        
        private IEnumerator Animate(bool animateImmediate = true)
        {
            yield return new WaitForSeconds(animateImmediate ? 0 : countdown -1 < 0 ? 0 : countdown -1);
            RootElement.ToggleInClassList("animate");
            if (XRUI.IsCurrentXRUIFormat(XRUI.XRUIFormat.ThreeDimensional))
                StartCoroutine(FadeWorldPanel(RootElement.ClassListContains("animate")));
        }
    }
}
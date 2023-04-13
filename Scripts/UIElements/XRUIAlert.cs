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
    /// <summary>
    /// XRUI Alert class.
    /// </summary>
    public class XRUIAlert : XRUIElement
    {
        /// <summary>
        /// The title UXML node of the alert.
        /// </summary>
        public Label Title { get; private set; }
        /// <summary>
        /// The content (body text) UXML node of the alert.
        /// </summary>
        public Label Content { get; private set; }

        /// <summary>
        /// The optional callback to trigger when the alert is clicked.
        /// </summary>
        public Action clickCallback;
        
        /// <summary>
        /// The optional countdown after which the alert is destroyed.
        /// </summary>
        public int countdown = 0;
        
        /// <summary>
        /// Initializes the UI Element.
        /// </summary>
        protected internal override void Init()
        {
            base.Init();
            Title = GetXRUIVisualElement<Label>("xrui-alert__title");
            Content = GetXRUIVisualElement<Label>("xrui-alert__content");
            
            // Set handler on click to dispose of the alert
            RootElement.RegisterCallback<PointerDownEvent>(_ => DisposeAlert(true));
            StartCoroutine(Animate());
        }

        /// <summary>
        /// Destroys the Alert with optional requirements.
        /// </summary>
        /// <param name="requirePointerOverUI">If true, will only destroy the alert when the pointer is on it.</param>
        /// <param name="destroyImmediate">If false, waits for the <see cref="countdown"> before destroying.</see></param>
        public void DisposeAlert(bool requirePointerOverUI = false, bool destroyImmediate = true)
        {
            if ((requirePointerOverUI && PointerOverUI) || !requirePointerOverUI)
            {
                StartCoroutine(Animate(destroyImmediate));
                StartCoroutine(Dispose(destroyImmediate));
            }
        }

        /// <summary>
        /// Destroys the Alert Game Object.
        /// </summary>
        /// <param name="destroyImmediate">If false, waits for the <see cref="countdown"/> before destroying.</param>
        /// <returns></returns>
        private IEnumerator Dispose(bool destroyImmediate = true)
        {
            yield return new WaitForSeconds(destroyImmediate ? 1 : countdown == 0 ? 1 : countdown);
            clickCallback?.Invoke();
            Destroy(this.gameObject);
        }
        
        /// <summary>
        /// Animates the alert by triggering an USS class.
        /// </summary>
        /// <param name="animateImmediate">If false, waits for the <see cref="countdown"/> before animating.</param>
        /// <returns></returns>
        private IEnumerator Animate(bool animateImmediate = true)
        {
            yield return new WaitForSeconds(animateImmediate ? 0 : countdown -1 < 0 ? 0 : countdown -1);
            RootElement.ToggleInClassList("animate");
            if (XRUI.IsGlobalXRUIFormat(XRUI.XRUIFormat.ThreeDimensional))
                StartCoroutine(FadeWorldPanel(RootElement.ClassListContains("animate")));
        }
        
        /// <summary>
        /// Defines the different alert types. Default styles are inspired by Bootstrap.
        /// </summary>
        public enum AlertType
        {
            /// <summary>
            /// Primary alert type, by default in blue
            /// </summary>
            Primary,
            /// <summary>
            /// Success alert type, by default in green
            /// </summary>
            Success,
            /// <summary>
            /// Warning alert type, by default in yellow
            /// </summary>
            Warning,
            /// <summary>
            /// Danger alert type, by default in red
            /// </summary>
            Danger,
            /// <summary>
            /// Info alert type, by default in light blue
            /// </summary>
            Info
        }
    }
}
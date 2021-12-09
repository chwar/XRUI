// MIT License
// Copyright (c) 2021 Chris Warin
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
using UnityEngine;
using UnityEngine.UIElements;

namespace com.chwar.xrui
{
    [CreateAssetMenu(fileName = "XRUIConfiguration", menuName = "XRUI/Create XRUI Configuration Asset", order = 1)]
    public class XRUIConfiguration : ScriptableObject
    {
        public PanelSettings panelSettings;
        public VisualTreeAsset defaultModalTemplate;
        public VisualTreeAsset defaultAlertTemplate;
        public VisualTreeAsset defaultCardTemplate;
        public VisualTreeAsset defaultListTemplate;
        public VisualTreeAsset defaultMenuTemplate;
        public VisualTreeAsset defaultNavbarTemplate;
        public VisualTreeAsset defaultContextualMenuTemplate;

        internal void Reset()
        {
            panelSettings = Resources.Load<PanelSettings>("DefaultPanelSettings");
            defaultModalTemplate = Resources.Load<VisualTreeAsset>("DefaultModal");
            defaultAlertTemplate = Resources.Load<VisualTreeAsset>("DefaultAlert");
            defaultCardTemplate = Resources.Load<VisualTreeAsset>("DefaultCard");
            defaultListTemplate = Resources.Load<VisualTreeAsset>("DefaultList");
            defaultMenuTemplate = Resources.Load<VisualTreeAsset>("DefaultMenu");
            defaultNavbarTemplate = Resources.Load<VisualTreeAsset>("DefaultNavbar");
            defaultContextualMenuTemplate = Resources.Load<VisualTreeAsset>("DefaultContextualMenu");
        }
    }
}
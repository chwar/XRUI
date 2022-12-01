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
    /// <summary>
    /// Configuration assets are created from this class. The generated assets can be used as a configuration for XRUI.
    /// A configuration asset needs to be referenced in the <see cref="XRUI"/> controller.
    /// </summary>
    [CreateAssetMenu(fileName = "XRUIConfiguration", menuName = "XRUI/Create XRUI Configuration Asset", order = 1)]
    public class XRUIConfiguration : ScriptableObject
    {
        /// <summary>
        /// The <see cref="PanelSettings"/> used by this configuration.
        /// </summary>
        public PanelSettings panelSettings;
        /// <summary>
        /// The default Modal template used by this configuration. 
        /// </summary>
        public VisualTreeAsset defaultModalTemplate;
        /// <summary>
        /// The default Alert template used by this configuration. 
        /// </summary>
        public VisualTreeAsset defaultAlertTemplate;
        /// <summary>
        /// The default Card template used by this configuration. 
        /// </summary>
        public VisualTreeAsset defaultCardTemplate;
        /// <summary>
        /// The default List template used by this configuration. 
        /// </summary>
        public VisualTreeAsset defaultListTemplate;
        /// <summary>
        /// The default Menu template used by this configuration. 
        /// </summary>
        public VisualTreeAsset defaultMenuTemplate;
        /// <summary>
        /// The default Navbar template used by this configuration. 
        /// </summary>
        public VisualTreeAsset defaultNavbarTemplate;
        /// <summary>
        /// The default Contextual Menu template used by this configuration. 
        /// </summary>
        public VisualTreeAsset defaultContextualMenuTemplate;
        /// <summary>
        /// The default World UI configuration for alerts.
        /// </summary>
        public WorldUIParameters defaultAlertWorldUIParameters;
        /// <summary>
        /// The default World UI configuration for contextual menus.
        /// </summary>
        public WorldUIParameters defaultContextualMenuWorldUIParameters;
        /// <summary>
        /// The default World UI configuration for modals.
        /// </summary>
        public WorldUIParameters defaultModalWorldUIParameters;

        /// <summary>
        /// Unity method which resets the configuration asset to default values.
        /// </summary>
        internal void Reset()
        {
            panelSettings = Resources.Load<PanelSettings>("Default2DPanelSettings");
            defaultModalTemplate = Resources.Load<VisualTreeAsset>("DefaultModal");
            defaultAlertTemplate = Resources.Load<VisualTreeAsset>("DefaultAlert");
            defaultCardTemplate = Resources.Load<VisualTreeAsset>("DefaultCard");
            defaultListTemplate = Resources.Load<VisualTreeAsset>("DefaultList");
            defaultMenuTemplate = Resources.Load<VisualTreeAsset>("DefaultMenu");
            defaultNavbarTemplate = Resources.Load<VisualTreeAsset>("DefaultNavbar");
            defaultContextualMenuTemplate = Resources.Load<VisualTreeAsset>("DefaultContextualMenu");
            defaultAlertWorldUIParameters = new WorldUIParameters();
            defaultContextualMenuWorldUIParameters = new WorldUIParameters();
            defaultModalWorldUIParameters = new WorldUIParameters();
        }
    }
}
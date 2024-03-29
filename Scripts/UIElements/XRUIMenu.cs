// MIT License
// Copyright (c) 2021 Chris Warin
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
using UnityEngine;
using UnityEngine.UIElements;

namespace com.chwar.xrui.UIElements
{
    /// <summary>
    /// XRUI Menu class.
    /// </summary>
    public class XRUIMenu : XRUIElement
    {
        /// <summary>
        /// The title UXML node of the menu.
        /// </summary>
        private Label _title;
        /// <summary>
        /// The subtitle UXML node of the menu.
        /// </summary>
        private Label _subtitle;
        /// <summary>
        /// The container UXML node of the menu.
        /// </summary>
        private ScrollView _container;

        /// <summary>
        /// The title property in the Inspector.
        /// </summary>
        [Tooltip("Title of the menu")]
        [SerializeField]
        private string titleText;
        /// <summary>
        /// The subtitle property in the Inspector.
        /// </summary>
        [Tooltip("Subtitle of the menu")]
        [SerializeField]
        private string subtitleText;
        /// <summary>
        /// The template to add elements to the menu.
        /// </summary>
        [Tooltip("Template used to add elements to the menu")]
        public VisualTreeAsset menuElementTemplate;


        /// <summary>
        /// Initializes the UI Element.
        /// </summary>
        protected internal override void Init()
        {
            base.Init();
            _title = GetXRUIVisualElement<Label>("xrui-menu__title");
            _subtitle = GetXRUIVisualElement<Label>("xrui-menu__subtitle");
            _container = GetXRUIVisualElement<ScrollView>("xrui-menu__container");
        }
        
        /// <summary>
        /// Updates UXML UI with the values inserted in the inspector.
        /// </summary>
        internal override void UpdateUI()
        {
            base.UpdateUI();
            UpdateTitle(titleText);
            UpdateSubtitle(subtitleText);
        }
        
        /*Update Methods*/

        /// <summary>
        /// Updates the title.
        /// </summary>
        /// <param name="text">The new text to replace the title with.</param>
        public void UpdateTitle(string text)
        {
            if(_title != null && _title.text != text) {
                _title.text = text;
                // Also update inspector value to keep only one value in case the field is updated from code 
                titleText = text;
            }
        }
        
        /// <summary>
        /// Updates the subtitle.
        /// </summary>
        /// <param name="text">The new text to replace the subtitle with.</param>
        public void UpdateSubtitle(string text)
        {
            if(_subtitle != null && _subtitle.text != text) {
                _subtitle.text = text;
                // Also update inspector value to keep only one value in case the field is updated from code 
                subtitleText = text;
            }
        }
        
        /// <summary>
        /// Adds template element to the menu
        /// </summary>
        /// <returns>The added element</returns>
        public VisualElement AddElement()
        {
            if (menuElementTemplate is null)
            {
                throw new MissingReferenceException($"The menu element template of {this.gameObject.name} is missing!");
            }
            VisualElement el = menuElementTemplate.Instantiate();
            el.ElementAt(0).AddToClassList("xrui-menu-item");
            _container.Add(el);
            return el;
        }
        
        /// <summary>
        /// Deletes all elements from the menu
        /// </summary>
        public void RemoveAllElements()
        {
            _container.Query(null, "xrui-menu-item").ForEach(i => i.parent.RemoveFromHierarchy());
        }
    }
}
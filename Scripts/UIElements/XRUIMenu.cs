// MIT License
// Copyright (c) 2021 Chris Warin
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
using UnityEngine;
using UnityEngine.UIElements;

namespace com.chwar.xrui.UIElements
{
    public class XRUIMenu : XRUIElement
    {
        // UXML Attributes
        public Button MainButton;
        public VisualElement Menu;
        public Button CloseButton;
        
        private Label _appTitleLabel;
        private Label _subtextLabel;
        private ScrollView _container;

        [Tooltip("Title of the menu")] 
        [SerializeField]
        private string titleText;
        public string TitleText
        { 
            get => titleText;
            set => UpdateTitle(value);
        }

        [Tooltip("Subtitle of the menu")] 
        [SerializeField]
        private string subText;
        public string SubText 
        { 
            get => subText;
            set => UpdateSubtext(value);
        }
        [Tooltip("Template used to add elements to the menu")]
        public VisualTreeAsset menuElementTemplate;
        [Tooltip("Texture used for the close button")]
        public Texture2D closeButtonTexture;
        [Tooltip("Texture used for the main button")]
        public Texture2D mainButtonTexture;

        /// <summary>
        /// Initializes the UI Elements of the List.
        /// </summary>
        protected internal override void Init()
        {
            base.Init();
            _appTitleLabel = UIDocument.rootVisualElement.Q<Label>("AppName");
            _subtextLabel = UIDocument.rootVisualElement.Q<Label>("Subtext");
            _container = UIDocument.rootVisualElement.Q<ScrollView>("MainContainer");
            Menu = UIDocument.rootVisualElement.Q("Menu");
            MainButton = UIDocument.rootVisualElement.Q<Button>("MainButton");
            MainButton.style.backgroundImage = mainButtonTexture;
            CloseButton = UIDocument.rootVisualElement.Q<Button>("Close");
            CloseButton.style.backgroundImage = closeButtonTexture;
        }
        
        /// <summary>
        /// Updates UXML UI with the values inserted in the inspector.
        /// </summary>
        internal override void UpdateUI()
        {
            base.UpdateUI();
            UpdateTitle(TitleText);
            UpdateSubtext(SubText);
        }
        
        /*Update Methods*/

        public void UpdateTitle(string text)
        {
            if(_appTitleLabel != null && _appTitleLabel.text != text)
                _appTitleLabel.text = text;
        }
        
        public void UpdateSubtext(string text)
        {
            if(_subtextLabel != null && _subtextLabel.text != text)
                _subtextLabel.text = text;
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
            _container.Add(el);
            return el;
        }
        
        /// <summary>
        /// Deletes all elements from the list
        /// </summary>
        public void RemoveAllElements()
        {
            _container.Query(null, "xrui__menu__item").ForEach(i => i.RemoveFromHierarchy());
        }
    }
}
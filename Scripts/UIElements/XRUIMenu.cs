using System;
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
        
        private Label _appTitle;
        private Label _subtext;
        private ScrollView _container;

        [Tooltip("Title of the menu")]
        public string titleText;
        [Tooltip("Subtitle of the menu")]
        public string subText;
        [Tooltip("Template used to add elements to the menu")]
        public VisualTreeAsset listElementTemplate;
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
            _appTitle = UIDocument.rootVisualElement.Q<Label>("AppName");
            _subtext = UIDocument.rootVisualElement.Q<Label>("Subtext");
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
            UpdateTitle(titleText);
            UpdateSubtext(subText);
        }
        
        /*Update Methods*/

        public void UpdateTitle(string text)
        {
            if(_appTitle != null && _appTitle.text != text)
                _appTitle.text = text;
        }
        
        public void UpdateSubtext(string text)
        {
            if(_subtext != null && _subtext.text != text)
                _subtext.text = text;
        }
        
        /// <summary>
        /// Adds template element to the menu
        /// </summary>
        /// <returns>The added element</returns>
        public VisualElement AddElement()
        {
            VisualElement el = listElementTemplate.Instantiate();
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
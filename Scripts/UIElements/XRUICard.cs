using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.chwar.xrui.UIElements
{
    public class XRUICard : XRUIElement
    {
        // UXML Attributes
        private Label _title;
        private Label _subtitle;
        private Button _closeButton;
        
        [Tooltip("Dimensions of the card")]
        public Vector2 cardDimensions;
        [Tooltip("Title of the card")]
        public string titleText;
        [Tooltip("Subtitle of the card")]
        public string subtitleText;
        
        protected override void Init()
        {
            _title = UIDocument.rootVisualElement.Q<Label>("Title");
            _subtitle = UIDocument.rootVisualElement.Q<Label>("Subtitle");
            _closeButton = UIDocument.rootVisualElement.Q<Button>("Close");
            base.Init();
        }

        /// <summary>
        /// Updates UXML UI with the values inserted in the inspector.
        /// </summary>
        internal override void UpdateUI()
        {
            base.UpdateUI();
            UpdateTitle(titleText);
            UpdateSubtitle(subtitleText);
            UpdateDimensions(cardDimensions);
        }
        
        /*Update Methods*/

        public void UpdateTitle(string text)
        {
            if(_title != null && _title.text != text)
                _title.text = text;
        }
        
        public void UpdateSubtitle(string text)
        {
            if(_subtitle != null && _subtitle.text != text)
                _subtitle.text = text;
        }

        public void UpdateDimensions(Vector2 dimensions)
        {
            if (UIDocument != null && cardDimensions != dimensions)
            {
                UIDocument.rootVisualElement.ElementAt(0).style.width = dimensions.x;
                UIDocument.rootVisualElement.ElementAt(0).style.height = dimensions.y;
            }
        }

        public void SetCloseButtonAction(Action closeButtonAction)
        {
            _closeButton.clicked += closeButtonAction;
        }
    }
}

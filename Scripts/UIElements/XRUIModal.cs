// MIT License
// Copyright (c) 2021 Chris Warin
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using Label = UnityEngine.UIElements.Label;

namespace com.chwar.xrui.UIElements
{
    public class XRUIModal : XRUIFloatingElement
    {
        public List<VisualTreeAsset> modalFlowList;
        public Label ModalTitle { get; private set; }
        public Button ValidateButton { get; private set; }
        public Button CancelButton { get; private set; }

        public Texture2D closeButtonTexture;
        
        private Button _closeButton;
        private VisualElement _buttonsContainer;
        private Action _cancelButtonAction;
        private Action _validateButtonAction;
        private readonly Dictionary<string, List<TextField>> _requiredFields = new();

        protected internal override void Init()
        {
            base.Init();
            ModalTitle = UIDocument.rootVisualElement.Q<Label>("ModalTitle");
            ValidateButton = UIDocument.rootVisualElement.Q<Button>("Validate");
            CancelButton = UIDocument.rootVisualElement.Q<Button>("Cancel");
            _closeButton = UIDocument.rootVisualElement.Q<Button>("CloseButton");
            _closeButton.style.backgroundImage = closeButtonTexture;
            _closeButton.clicked += () => Destroy(this.gameObject);
            _buttonsContainer = UIDocument.rootVisualElement.Q<VisualElement>("ButtonsContainer");
        }

        /// <summary>
        /// Determines the placement of the main buttons.
        /// </summary>
        /// <param name="placement">USS property to define placement.</param>
        public void SetButtonsPlacement(Justify placement)
        {
            _buttonsContainer.style.justifyContent = new StyleEnum<Justify>(placement);
        }

        /// <summary>
        /// Subscribes an action to the cancel button and replaces any other previously subscribed action.
        /// </summary>
        /// <param name="action"></param>
        public void SetCancelButtonAction(Action action)
        {
            CancelButton.clicked -= _cancelButtonAction;
            _cancelButtonAction = action;
            CancelButton.clicked += _cancelButtonAction;
        }
        
        /// <summary>
        /// Subscribes an action to the validate button and replaces any other previously subscribed action.
        /// </summary>
        /// <param name="action"></param>
        public void SetValidateButtonAction(Action action)
        {
            ValidateButton.clicked -= _validateButtonAction;
            _validateButtonAction = action;
            ValidateButton.clicked += _validateButtonAction;
        }

        /// <summary>
        /// Updates the content of the modal with the desired content. Creates content if it is non existing, otherwise makes it visible.
        /// </summary>
        /// <param name="contentAssetName">The name of the Visual Tree Asset to instantiate or use. Must be a part of the modal flow list.</param>
        /// <param name="parent">Name of the container in which to put the content.</param>
        /// <param name="onCreate">Callback that is triggered only once, upon the content's instantiation.</param>
        /// <exception cref="ArgumentException">Content asset name or parent not found.</exception>
        public void UpdateModalFlow(string contentAssetName, string parent, Action onCreate)
        {
            // Fetch content UXML from modal flow list
            var content = modalFlowList.Find(c => c.name.Equals(contentAssetName));
            if (content is null)
            {
                throw new ArgumentException($"Modal does not have a reference to {contentAssetName} in its flow list");
            }
            
            // Find the container
            var main = UIDocument.rootVisualElement.Q(parent);
            if (main is null)
            {
                throw new ArgumentException($"There is no Visual Element called \"{parent}\" in the Modal. " +
                                            "Please add one in order to append content inside");
            }
            
            // Hide visible (current) content and add new content
            var current = main.childCount > 0 ? main.Children().First(ve => 
                ve.style.display.value.Equals(DisplayStyle.Flex)) : null;
            if(current is not null)
                current.style.display = DisplayStyle.None;
            
            // Check if content to add is already existing and hidden
            var existingContent = UIDocument.rootVisualElement.Q<VisualElement>(contentAssetName);
            if (existingContent is not null)
            {
                existingContent.style.display = DisplayStyle.Flex;
            }
            else
            {
                var ui = content.Instantiate();
                // Make content take all container space
                ui.style.flexGrow = 1;
                ui.name = contentAssetName;
                AddUIElement(ui, parent);

                // Fire callback for user-defined behaviour on content creation
                onCreate();
            }
            CheckFormValidity();
            UpdateUI();
        }

        /// <summary>
        /// Sets the given fields as required to validate the current page.
        /// </summary>
        /// <param name="fields">Fields to set as required for the page they are contained in.</param>
        public void SetRequiredFields(params TextField[] fields)
        {
            var page = UIDocument.rootVisualElement.Query<TemplateContainer>().Where(ve => 
                ve.style.display.value.Equals(DisplayStyle.Flex)).Last().name;
            if (!_requiredFields.ContainsKey(page))
            {
                _requiredFields.Add(page, new List<TextField>());
            }
            foreach (var el in fields)
            {
                el.RegisterCallback<ChangeEvent<string>>(CheckFormValidity);
                _requiredFields[page].Add(el);
            }
            CheckFormValidity();
        }

        /// <summary>
        /// Flags a field with an error.
        /// </summary>
        /// <param name="field">The field to flag.</param>
        public void SetFieldError(TextField field)
        {
            field.EnableInClassList("error", true);
            field.RegisterValueChangedCallback(_ => ClickOnError(field));
        }

        private void ClickOnError(TextField field)
        {
            field.RemoveFromClassList("error");
            field.UnregisterValueChangedCallback(_ => ClickOnError(field));
        }

        /// <summary>
        /// Activates the validate button if all required fields from a page are not empty.
        /// </summary>
        private void CheckFormValidity()
        {
            var currentPage = UIDocument.rootVisualElement.Query<TemplateContainer>().Where(ve => 
                ve.style.display.value.Equals(DisplayStyle.Flex)).Last().name;
            if (_requiredFields.ContainsKey(currentPage))
            {
                bool validity = _requiredFields[currentPage].All(tf => !tf.value.Equals(string.Empty));
                ValidateButton.SetEnabled(validity);
            }
            else
            {
                ValidateButton.SetEnabled(true);
            }
        }
        
        private void CheckFormValidity<T>(ChangeEvent<T> evt)
        {
            CheckFormValidity();
        }
    }
}
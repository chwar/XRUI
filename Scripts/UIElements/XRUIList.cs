// MIT License
// Copyright (c) 2021 Chris Warin
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.chwar.xrui.UIElements
{
    public class XRUIList : XRUIElement
    {
        // UXML Attributes
        public Button AddButton;
        public VisualElement List;
        
        private Label _title;
        private ScrollView _container;

        [Tooltip("Title of the list")]
        [SerializeField]
        private string titleText;
        [Tooltip("Template used to add elements to the list")]
        public VisualTreeAsset listElementTemplate;
        [Tooltip("Texture used for the Add button")]
        public Texture2D addButtonTexture;
        

        /// <summary>
        /// Initializes the UI Elements of the List.
        /// </summary>
        protected internal override void Init()
        {
            base.Init();
            _title = UIDocument.rootVisualElement.Q<Label>("Title");
            _container = UIDocument.rootVisualElement.Q<ScrollView>("MainContainer");
            List = UIDocument.rootVisualElement.Q("List");
            AddButton = UIDocument.rootVisualElement.Q<Button>("AddItem");
            AddButton.style.backgroundImage = addButtonTexture;
        }
        
        /// <summary>
        /// Updates UXML UI with the values inserted in the inspector.
        /// </summary>
        internal override void UpdateUI()
        {
            base.UpdateUI();
            UpdateTitle(titleText);
        }
        
        /*Update Methods*/

        public void UpdateTitle(string text)
        {
            if (_title != null && _title.text != text)
            {
                _title.text = text;
                // Also update inspector value to keep only one value in case the field is updated from code 
                titleText = text;
            }
        }
        
        /// <summary>
        /// Adds template element to the list
        /// </summary>
        /// <param name="bSelect">Selects the added element in the list</param>
        /// <returns>The added element</returns>
        public VisualElement AddElement(bool bSelect)
        {
            if (listElementTemplate is null)
            {
                throw new MissingReferenceException($"The list element template of {this.gameObject.name} is missing!");
            }
            
            VisualElement el = listElementTemplate.Instantiate();
            if (bSelect)
                SelectElement(el.ElementAt(0));
            
            el.ElementAt(0).AddToClassList("xrui__list__item");
            _container.Add(el);
            return el;
        }

        /// <summary>
        /// Adds template element to the list
        /// </summary>
        /// <param name="bSelect">Selects the added element in the list</param>
        /// <param name="itemSelectedCallback">The callback to trigger when the element is selected</param>
        /// <returns>The added element</returns>
        public VisualElement AddElement(bool bSelect, Action<PointerDownEvent> itemSelectedCallback)
        {
            var el = AddElement(bSelect);
            el.RegisterCallback<PointerDownEvent>(e =>
            {
                SelectElement(el.ElementAt(0));
                itemSelectedCallback?.Invoke(e);
            });
            return el;
        }

        /// <summary>
        /// Deletes all elements from the list
        /// </summary>
        public void RemoveAllElements()
        {
            _container.Query(null, "xrui__list__item").ForEach(i => i.RemoveFromHierarchy());
        }

        public int GetListCount()
        {
            return  _container.Query(null, "xrui__list__item").ToList().Count;
        }

        /// <summary>
        /// Visually selects an element of the list
        /// </summary>
        /// <param name="el"></param>
        public void SelectElement(VisualElement el)
        {
            var previousSelection = _container.Q(null, "xrui__list__item--selected");
            previousSelection?.ToggleInClassList("xrui__list__item--selected");
            el.ToggleInClassList("xrui__list__item--selected");
        }
    }
}
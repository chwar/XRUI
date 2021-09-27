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
        public string titleText;
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
            if(_title != null && _title.text != text)
                _title.text = text;
        }
        
        /// <summary>
        /// Adds template element to the list
        /// </summary>
        /// <param name="bSelect">Selects the added element in the list</param>
        /// <returns>The added element</returns>
        public VisualElement AddElement(bool bSelect)
        {
            VisualElement el = listElementTemplate.Instantiate();
            if(bSelect)
                SelectElement(el.ElementAt(0));
            el.ElementAt(0).AddToClassList("xrui__list__item");
            el.RegisterCallback<PointerDownEvent>(_ => SelectElement(el.ElementAt(0))); 
            _container.Add(el);
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
        private void SelectElement(VisualElement el)
        {
            var previousSelection = _container.Q(null, "xrui__list__item--selected");
            previousSelection?.ToggleInClassList("xrui__list__item--selected");
            el.ToggleInClassList("xrui__list__item--selected");
        }
    }
}
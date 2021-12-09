// MIT License
// Copyright (c) 2021 Chris Warin
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.chwar.xrui.UIElements
{
    public class XRUIContextualMenu : XRUIFloatingElement
    {
        /// <summary>
        /// Template used to generate entries.
        /// </summary>
        public VisualTreeAsset menuElementTemplate;
        /// <summary>
        /// The coordinates of the clicked element that triggered this contextual menu.
        /// </summary>
        public Vector2 parentCoordinates;
        /// <summary>
        /// Adds an offset in pixels used when the contextual menu is positioned on the left of the parent coordinates.
        /// </summary>
        public float positionOffsetLeft = 50;
        /// <summary>
        /// Adds an offset in pixels used when the contextual menu is positioned on the right of the parent coordinates.
        /// </summary>
        public float positionOffsetRight = 50;
        /// <summary>
        /// Display an arrow pointing at the parent coordinates (clicked element).
        /// </summary>
        public bool showArrow;
        /// <summary>
        /// UXML representation of the contextual menu.
        /// </summary>
        private VisualElement _menu;
        /// <summary>
        /// UXML representation of the contextual menu's arrow.
        /// </summary>
        private VisualElement _contextualArrow;
        
        /// <summary>
        /// Initializes the UI Elements of the List.
        /// </summary>
        protected internal override void Init()
        {
            base.Init();
            _menu = UIDocument.rootVisualElement.Q(null, "xrui__contextual-menu");
            _contextualArrow = _menu.Q(null, "xrui__contextual-menu__arrow");
            
            // Set handler on click to dispose of the contextual menu
            UIDocument.rootVisualElement.RegisterCallback<PointerDownEvent>(_ => DisposeMenu());
            _menu.RegisterCallback<GeometryChangedEvent>(PositionRelativeToParent);
        }

        /// <summary>
        /// Positions the contextual menu with respect to the parent coordinates.
        /// </summary>
        /// <param name="evt"></param>
        internal void PositionRelativeToParent(GeometryChangedEvent evt)
        {
            _menu.style.position = new StyleEnum<Position>(Position.Absolute);
            _menu.style.top = parentCoordinates.y;
            // If there's not enough space to display the contextual menu on the right of the target, display on the left instead
            if (_menu.parent.worldBound.width - parentCoordinates.x < _menu.resolvedStyle.width)
            {
                _menu.style.left = StyleKeyword.Auto;
                _menu.style.right = _menu.parent.worldBound.width - parentCoordinates.x + positionOffsetLeft;
                _contextualArrow.style.left =  StyleKeyword.Auto;
                _contextualArrow.style.right = -5;
            } 
            else
                _menu.style.left = parentCoordinates.x + positionOffsetRight;

            if (!showArrow)
            {
                _contextualArrow.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            }
        }

        /// <summary>
        /// Destroys the contextual menu.
        /// </summary>
        internal void DisposeMenu()
        {
            if (!PointerOverUI)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Adds an element to the contextual menu using the provided menuElementTemplate.
        /// </summary>
        /// <returns>The created element.</returns>
        public VisualElement AddMenuElement()
        {
            if (menuElementTemplate is null)
            {
                throw new ArgumentNullException(
                    $"There is no template to create an entry from! " +
                    $"Please provide a valid template like so: xruiContextualMenu.menuElementTemplate = myTemplate;");
            }
            var el = menuElementTemplate.Instantiate();
            el.style.flexShrink = 0;
            el.style.flexGrow = 1;
            el.ElementAt(0).AddToClassList("xrui__contextual-menu__item");
            _menu.Q("MainContainer").Add(el);
            
            // Destroy the menu when the element is clicked
            el.RegisterCallback<PointerDownEvent>(_ => Destroy(this.gameObject));
            
            return el.ElementAt(0);
        }
    }
}
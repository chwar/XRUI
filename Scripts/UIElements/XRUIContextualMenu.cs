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
    /// <summary>
    /// XRUI Contextual Menu class.
    /// </summary>
    public class XRUIContextualMenu : XRUIElement
    {
        /// <summary>
        /// The  arrow UXML node of the contextual menu.
        /// </summary>
        private VisualElement _contextualArrow;
        /// <summary>
        /// Template used to generate entries.
        /// </summary>
        public VisualTreeAsset menuElementTemplate;
        /// <summary>
        /// The coordinates of the clicked element that triggered this contextual menu.
        /// </summary>
        public Vector3 parentCoordinates;
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
        /// Initializes the UI Element.
        /// </summary>
        protected internal override void Init()
        {
            base.Init();
            _contextualArrow = RootElement.Q(null, "xrui-contextual-menu__arrow");
            
            // Set handler on click to dispose of the contextual menu
            RootElement.parent.RegisterCallback<PointerDownEvent>(_ => DisposeMenu());
            RootElement.RegisterCallback<GeometryChangedEvent>(PositionRelativeToParent);
        }

        /// <summary>
        /// Positions the contextual menu with respect to the parent coordinates.
        /// </summary>
        /// <param name="evt"></param>
        internal void PositionRelativeToParent(GeometryChangedEvent evt)
        {
            if (XRUI.IsGlobalXRUIFormat(XRUI.XRUIFormat.TwoDimensional))
            {
                RootElement.style.position = new StyleEnum<Position>(Position.Absolute);
                RootElement.style.top = parentCoordinates.y;
                // If there's not enough space to display the contextual menu on the right of the target, display on the left instead
                if (RootElement.parent.worldBound.width - parentCoordinates.x < RootElement.resolvedStyle.width)
                {
                    RootElement.style.left = StyleKeyword.Auto;
                    RootElement.style.right = RootElement.parent.worldBound.width - parentCoordinates.x + positionOffsetLeft;
                    _contextualArrow.style.left =  StyleKeyword.Auto;
                    _contextualArrow.style.right = -5;
                } 
                else
                    RootElement.style.left = parentCoordinates.x + positionOffsetRight;
            }

            _contextualArrow.style.display = new StyleEnum<DisplayStyle>(showArrow ? DisplayStyle.Flex: DisplayStyle.None);
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
            el.ElementAt(0).AddToClassList("xrui-contextual-menu__item");
            RootElement.Q("MainContainer").Add(el);
            
            // Destroy the menu when the element is clicked
            el.RegisterCallback<PointerDownEvent>(_ => Destroy(this.gameObject));
            
            return el.ElementAt(0);
        }
    }
}
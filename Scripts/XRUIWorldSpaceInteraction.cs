// MIT License
// Copyright (c) 2021 Chris Warin
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace com.chwar.xrui
{
    /// <summary>
    /// This class interfaces with Unity <see cref="EventHandler"/> and makes XRUI react to world space interactions (e.g., MR/VR pointers). 
    /// </summary>
    public class XRUIWorldSpaceInteraction : MonoBehaviour, IPointerMoveHandler, IPointerUpHandler, IPointerDownHandler,
    ISubmitHandler, ICancelHandler, IMoveHandler, IScrollHandler, ISelectHandler, IDeselectHandler, IDragHandler
    {
        /// <summary>
        /// The <see cref="PanelSettings"/> of the UI that is pointed at.
        /// </summary>
        public PanelSettings targetPanel;
        /// <summary>
        /// <see cref="PanelEventHandler"/> of the targeted panel.
        /// </summary>
        private PanelEventHandler _panelEventHandler; 
        /// <summary>
        /// Method that translates 3D coordinates of the world space raycast to 2D texture coordinates.
        /// </summary>
        private Func<Vector2, Vector2> _renderTextureScreenTranslation;

        /// <summary>
        /// Unity method.
        /// </summary>
        void OnEnable()
        {
            targetPanel = GetComponent<UIDocument>().panelSettings;
            if (targetPanel != null)
            {
                if (_renderTextureScreenTranslation == null)
                {
                    _renderTextureScreenTranslation = ScreenCoordinatesToRenderTexture;
                }
                targetPanel.SetScreenToPanelSpaceFunction(_renderTextureScreenTranslation);
            }
            
            // find the automatically generated PanelEventHandler and PanelRaycaster for this panel and disable the raycaster
            PanelEventHandler[] handlers = FindObjectsOfType<PanelEventHandler>();
            var uiDocument = GetComponent<UIDocument>();
            foreach (PanelEventHandler handler in handlers)
            {
                if (handler.panel == uiDocument.rootVisualElement.panel)
                {
                    _panelEventHandler = handler;
                    PanelRaycaster panelRaycaster = _panelEventHandler.GetComponent<PanelRaycaster>();
                    if (panelRaycaster != null)
                        panelRaycaster.enabled = false;
                    
                    break;
                }
            }
        }

        /// <summary>
        /// Unity method.
        /// </summary>
        void OnDisable()
        {
            //we reset it back to the default behavior
            if (targetPanel != null)
            {
                targetPanel.SetScreenToPanelSpaceFunction(null);
            }
        }

        /// <summary>
        /// Transforms a screen position to a position relative to render texture used by a MeshRenderer.
        /// </summary>
        /// <param name="screenPosition">The position in screen coordinates.</param>
        /// <returns>Returns the coordinates in texel space, or a position containing NaN values if no hit was recorded or if the hit mesh's material is not using the render texture as their mainTexture</returns>
        internal Vector2 ScreenCoordinatesToRenderTexture(Vector2 screenPosition)
        {
            var invalidPosition = new Vector2(float.NaN, float.NaN);
            screenPosition.y = Screen.height - screenPosition.y;
            Ray cameraRay = Camera.main.ScreenPointToRay(screenPosition);
            
            RaycastHit hit;
            if (!Physics.Raycast(cameraRay, out hit))
            {
                return invalidPosition;
            }
            
            var targetTexture = targetPanel.targetTexture;
            Vector2 pixelUV = hit.textureCoord;

            pixelUV.y = 1 - pixelUV.y;
            pixelUV.x *= targetTexture.width;
            pixelUV.y *= targetTexture.height;
            return pixelUV;
        }

        public void OnPointerMove (PointerEventData eventData)
        {
            _panelEventHandler?.OnPointerMove(eventData);
        }

        public void OnPointerDown (PointerEventData eventData)
        {
            _panelEventHandler?.OnPointerDown(eventData);
        }

        public void OnPointerUp (PointerEventData eventData)
        {
            _panelEventHandler?.OnPointerUp(eventData);
        }

        public void OnSubmit (BaseEventData eventData)
        {
            _panelEventHandler?.OnSubmit(eventData);
        }

        public void OnCancel (BaseEventData eventData)
        {
            _panelEventHandler?.OnCancel(eventData);
        }

        public void OnMove (AxisEventData eventData)
        {
            _panelEventHandler?.OnMove(eventData);
        }

        public void OnScroll (PointerEventData eventData)
        {
            _panelEventHandler?.OnScroll(eventData);
        }

        public void OnSelect (BaseEventData eventData)
        {
            _panelEventHandler?.OnSelect(eventData);
        }

        public void OnDeselect (BaseEventData eventData)
        {
            _panelEventHandler?.OnDeselect(eventData);
        }

        public void OnDrag (PointerEventData eventData)
        {
            OnPointerMove(eventData);
        }

    }
}
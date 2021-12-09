// MIT License
// Copyright (c) 2021 Chris Warin
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.chwar.xrui
{
    public class XRUITextureInteraction : MonoBehaviour
    {
        public PanelSettings targetPanel;
        private Func<Vector2, Vector2> _renderTextureScreenTranslation;

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
        }

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
            //Debug.Log(screenPosition);
            screenPosition.y = Screen.height - screenPosition.y;
            Ray cameraRay = Camera.main.ScreenPointToRay(screenPosition);
            
            // var cameraRay = Camera.main.ScreenPointToRay(screenPosition);

            RaycastHit hit;
            if (!Physics.Raycast(cameraRay, out hit))
            {
                Debug.DrawLine(cameraRay.origin, cameraRay.direction * 10, Color.red);
                return invalidPosition;
            }
            Debug.Log(hit.collider.name);
            Debug.DrawLine(cameraRay.origin, cameraRay.direction * 10, Color.green);
            var targetTexture = targetPanel.targetTexture;
            MeshRenderer rend = hit.transform.GetComponent<MeshRenderer>();

            if (rend == null || rend.sharedMaterial.mainTexture != targetTexture)
            {
                return invalidPosition;
            }

            Vector2 pixelUV = hit.textureCoord;

            //since y screen coordinates are usually inverted, we need to flip them
            pixelUV.y = 1 - pixelUV.y;
            pixelUV.x *= targetTexture.width;
            pixelUV.y *= targetTexture.height;
            return pixelUV;
        }
    }
}
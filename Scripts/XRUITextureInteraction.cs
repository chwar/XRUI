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
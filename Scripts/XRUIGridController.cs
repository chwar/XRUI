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

namespace com.chwar.xrui
{
    /// <summary>
    /// Controller which creates a grid to organise UI elements on screen.
    /// </summary>
    [ExecuteAlways]
    public class XRUIGridController : MonoBehaviour
    {
        /// <summary>
        /// List of <see cref="XRUIGrid"/> that are in the grid.
        /// </summary>
        public List<XRUIGrid> gridElementsList = new();
        /// <summary>
        /// List of the children <see cref="Transform"/>.
        /// </summary>
        private List<Transform> _listGridElements;
        /// <summary>
        /// Checks if the grid has been initialised yet at runtime.
        /// </summary>
        private bool _isInitialized;
        
        /// <summary>
        /// Unity method. 
        /// </summary>
        private void Awake()
        {
            _listGridElements = new List<Transform>();
        }
        
        /// <summary>
        /// Unity method. Refreshes the grid in the Editor.
        /// </summary>
        private void OnValidate()
        {
            if(_isInitialized || !Application.isPlaying)
                RefreshGrid();
        }

        /// <summary>
        /// Unity method. Enables all children of the grid. 
        /// </summary>
        private void OnEnable()
        {
            if(_listGridElements != null)
                _listGridElements.ForEach(x => x.gameObject.SetActive(true));
            
            // This does not run during the initial run, only during the app's lifetime if the XRUIGrid has been re-enabled.
            // During the initial run, the XRIGrid is initialized by the XRUI Instance to make sure that the instance is running first.
            if (_isInitialized || !Application.isPlaying)
            {
                RefreshGrid();
            }
        }

        /// <summary>
        /// Unity method. Disables all children of the grid.
        /// </summary>
        private void OnDisable()
        {
            if(Application.isPlaying)
                _listGridElements.ForEach(x => x.gameObject.SetActive(false));
            //_isInitialized = false;
        }

        /// <summary>
        /// Refreshes the grid according to the settings defined in the Inspector. 
        /// </summary>
        /// <exception cref="MissingComponentException">Fired if a <see cref="UIDocument"/> is missing on a row.</exception>
        public void RefreshGrid()
        {
            var worldUI = XRUI.IsCurrentXRUIFormat(XRUI.XRUIFormat.ThreeDimensional) && Application.isPlaying;
            
            // TODO Custom Editor that fills the list of elements automatically from the hierarchy
            var i = 0;
            foreach (var gridElement in gridElementsList)
            {
                var ui = gridElement.row.GetComponent<UIDocument>();
                if (!worldUI && ui == null)
                {
                    throw new MissingComponentException(
                        $"There is no UIDocument attached on the following XRUI row: {gridElement.row.name}");
                }

                if (!worldUI)
                {
                    var row = ui.rootVisualElement;
                    if(row is null) return;
                    
                    ui.sortingOrder = i;
                    row.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
                    row.style.height = new StyleLength(StyleKeyword.Initial);
                    row.style.flexGrow = gridElement.weight;
                    row.style.minHeight = gridElement.minHeight;

                    foreach (var child in row.Children())
                    {
                        child.style.position = new StyleEnum<Position>(Position.Absolute);
                        // child.pickingMode = PickingMode.Position;
                        child.style.top = 0;
                        child.style.bottom = 0;
                        child.style.left = 0;
                        child.style.right = 0;
                    }

                    i++;
                }
                else
                {
                    // Remove the UI documents of each row in VR to break the UIDocument hierarchy
                    // Each XRUI Element needs its own PanelSettings to have its own render texture to be displayed within the world
                    if (ui != null)
                    {
                        gridElement.row.transform.GetComponentsInChildren<Transform>().ToList().ForEach(t => t.parent = null);
                        this.gameObject.SetActive(false);
                        // _listGridElements.AddRange(gridElement.row.transform.GetComponentsInChildren<Transform>()
                        //     .ToList()
                        //     .GetRange(1,gridElement.row.transform.childCount));
                        // DestroyImmediate(ui);
                    }
                }
            }
            _isInitialized = true;
        }

        /// <summary>
        /// Used to reference rows in the Inspector.
        /// </summary>
        [Serializable]
        public struct XRUIGrid
        {
            /// <summary>
            /// The row in which <see cref="UIElements.XRUIElement"/> are contained.
            /// </summary>
            public GameObject row;
            /// <summary>
            /// The weight that this row has in comparison to other rows. This defines the space that this row is allowed to take.
            /// </summary>
            public float weight;
            /// <summary>
            /// The minimal height of this row in pixels. Useful when the elements inside are using absolute positioning.
            /// </summary>
            public float minHeight;
        }
    }
}

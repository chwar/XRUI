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
    [ExecuteAlways]
    public class XRUIGridController : MonoBehaviour
    {
        public VisualElement XruiRoot;  
        public List<XRUIGrid> gridElementsList = new();
        private List<Transform> _listGridElements;
        private bool _isInitialized;
        
        private void Awake()
        {
            _listGridElements = new List<Transform>();
        }
        
        private void OnValidate()
        {
            if(_isInitialized || !Application.isPlaying)
                AdaptGrid();
        }

        private void OnEnable()
        {
            if(_listGridElements != null)
                _listGridElements.ForEach(x => x.gameObject.SetActive(true));
            
            // This does not run during the initial run, only during the app's lifetime if the XRUIGrid has been re-enabled.
            // During the initial run, the XRIGrid is initialized by the XRUI Instance to make sure that the instance is running first.
            if (_isInitialized || !Application.isPlaying)
            {
                XruiRoot = GetComponent<UIDocument>().rootVisualElement?.panel.visualTree;
                AdaptGrid();
            }
        }

        private void OnDisable()
        {
            if(Application.isPlaying)
                _listGridElements.ForEach(x => x.gameObject.SetActive(false));
            //_isInitialized = false;
        }

        // Start is called before the first frame update
        public void AdaptGrid()
        {
            var vr = XRUI.IsCurrentReality(XRUI.RealityType.VR) && Application.isPlaying;
            
            // TODO Custom Editor that fills the list of elements automatically from the hierarchy
            var i = 0;
            foreach (var gridElement in gridElementsList)
            {
                var ui = gridElement.row.GetComponent<UIDocument>();
                if (!vr && ui == null)
                {
                    throw new MissingComponentException(
                        $"There is no UIDocument attached on the following XRUI row: {gridElement.row.name}");
                }

                if (!vr)
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
                        _listGridElements.AddRange(gridElement.row.transform.GetComponentsInChildren<Transform>()
                            .ToList()
                            .GetRange(1,gridElement.row.transform.childCount));
                        DestroyImmediate(ui);
                    }
                }
            }
            _isInitialized = true;
        }

        [Serializable]
        public struct XRUIGrid
        {
            public GameObject row;
            public float weight;
            public float minHeight;
        }
    }
}

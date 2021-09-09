using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.chwar.xrui
{
    [ExecuteAlways]
    public class XRUIGridController : MonoBehaviour
    {
        public List<XRUIGrid> gridElementsList = new();
        private XRUI _xrui;
        private UIDocument _rootUI;
        private List<Transform> _listGridElements;
        
        private void Start()
        {
            _listGridElements = new List<Transform>();
            //yield return new WaitUntil(() => _xrui.Ready);
            AdaptGrid();
        }

        private void OnEnable()
        {
            if(_listGridElements != null)
                _listGridElements.ForEach(x => x.gameObject.SetActive(true));
            _xrui = FindObjectOfType<XRUI>();
            if (_xrui.Ready)
                AdaptGrid();
        }

        private void OnDisable()
        {
            if(Application.isPlaying)
                _listGridElements.ForEach(x => x.gameObject.SetActive(false));
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
                    ui.sortingOrder = i;
                    row.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
                    row.style.height = new StyleLength(StyleKeyword.Initial);
                    row.style.flexGrow = gridElement.weight;
                    row.style.minHeight = gridElement.minHeight;

                    foreach (var child in row.Children())
                    {
                        child.style.position = new StyleEnum<Position>(Position.Absolute);
                        child.pickingMode = PickingMode.Position;
                        child.style.top = 0;
                        child.style.bottom = 0;
                        child.style.left = 0;
                        child.style.right = 0;
                    }

                    i++;
                }
                else
                {
                    // Remove the UI documents of each row in VR
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
        }

        private void OnValidate()
        {
            _xrui = FindObjectOfType<XRUI>();
            if(_xrui.Ready)
                AdaptGrid();
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

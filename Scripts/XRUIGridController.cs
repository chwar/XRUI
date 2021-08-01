using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.chwar.xrui
{
    [ExecuteAlways]
    public class XRUIGridController : MonoBehaviour
    {
        public List<XRUIGrid> gridElementsList;

        private void Start()
        {
            AdaptGrid();
        }

        private void OnEnable()
        {
            AdaptGrid();
        }

        // Start is called before the first frame update
        public void AdaptGrid()
        {
            // TODO Custom Editor that fills the list of elements automatically from the hierarchy
            foreach (var gridElement in gridElementsList)
            {
                var row = gridElement.row.GetComponent<UIDocument>().rootVisualElement;
                if (row == null) return;
                
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
            }
        }

        private void OnValidate()
        {
            AdaptGrid();
        }

        // Update is called once per frame
        void Update()
        {
        
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

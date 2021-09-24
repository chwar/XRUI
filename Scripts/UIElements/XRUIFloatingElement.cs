using System;
using UnityEngine.UIElements;

namespace com.chwar.xrui.UIElements
{
    public class XRUIFloatingElement : XRUIElement
    {
        public bool PointerOverFloatingElement { get; private set; }
        private void Awake()
        {
            Init();
            UIDocument.rootVisualElement.Q(null, "xrui").RegisterCallback<PointerEnterEvent>(OnPointerEnterFloating);
            UIDocument.rootVisualElement.Q(null, "xrui").RegisterCallback<PointerLeaveEvent>(OnPointerLeaveFloating);
        }

        private void OnPointerLeaveFloating(PointerLeaveEvent evt)
        {
            PointerOverFloatingElement = false;
        }

        private void OnPointerEnterFloating(PointerEnterEvent evt)
        {
            PointerOverFloatingElement = true;
        }
    }
}
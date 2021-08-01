using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.chwar.xrui.UIElements
{
    public class XRUIAlert : XRUIFloatingElement
    {
        public Label Title { get; private set; }
        public Label Content { get; private set; }

        private VisualElement _alert;
        
        /// <summary>
        /// Initializes the UI Elements of the List.
        /// </summary>
        protected override void Init()
        {
            Title = UIDocument.rootVisualElement.Q<Label>("AlertTitle");
            Content = UIDocument.rootVisualElement.Q<Label>("AlertContent");
            
            // Set handler on click to dispose of the alert
            UIDocument.rootVisualElement.RegisterCallback<PointerDownEvent>(DisposeAlert);
            _alert = UIDocument.rootVisualElement.Q(null, "xrui__alert");
            StartCoroutine(Animate());
            base.Init();
        }

        private void DisposeAlert(PointerDownEvent evt)
        {
            if (PointerOverUI)
            {
                StartCoroutine(Animate());
                StartCoroutine(Dispose());
            }
        }

        private IEnumerator Dispose()
        {
            yield return new WaitForSeconds(1);
                Destroy();
        }
        
        private IEnumerator Animate()
        {
            yield return new WaitForFixedUpdate();
            _alert.ToggleInClassList("animate");
        }
    }
}
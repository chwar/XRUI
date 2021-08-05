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

        internal VisualElement Alert;
        
        /// <summary>
        /// Initializes the UI Elements of the List.
        /// </summary>
        protected override void Init()
        {
            Title = UIDocument.rootVisualElement.Q<Label>("AlertTitle");
            Content = UIDocument.rootVisualElement.Q<Label>("AlertContent");
            
            // Set handler on click to dispose of the alert
            UIDocument.rootVisualElement.RegisterCallback<PointerDownEvent>(_ => DisposeAlert());
            Alert = UIDocument.rootVisualElement.Q(null, "xrui__alert");
            StartCoroutine(Animate());
            base.Init();
        }

        internal void DisposeAlert()
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
            Destroy(this.gameObject);
        }
        
        private IEnumerator Animate()
        {
            yield return new WaitForFixedUpdate();
            Alert.ToggleInClassList("animate");
        }
    }
}
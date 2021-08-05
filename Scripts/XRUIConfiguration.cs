using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.chwar.xrui
{
    [CreateAssetMenu(fileName = "XRUIConfiguration", menuName = "XRUI/Create XRUI Configuration Asset", order = 1)]
    public class XRUIConfiguration : ScriptableObject
    {
        public PanelSettings panelSettings;
        public VisualTreeAsset defaultModalTemplate;
        public VisualTreeAsset defaultAlertTemplate;
        public VisualTreeAsset defaultCardTemplate;
        public VisualTreeAsset defaultListTemplate;
        public VisualTreeAsset defaultMenuTemplate;
        public VisualTreeAsset defaultNavbarTemplate;

        internal void Reset()
        {
            panelSettings = Resources.Load<PanelSettings>("DefaultPanelSettings");
            defaultModalTemplate = Resources.Load<VisualTreeAsset>("DefaultModal");
            defaultAlertTemplate = Resources.Load<VisualTreeAsset>("DefaultAlert");
            defaultCardTemplate = Resources.Load<VisualTreeAsset>("DefaultCard");
            defaultListTemplate = Resources.Load<VisualTreeAsset>("DefaultList");
            defaultMenuTemplate = Resources.Load<VisualTreeAsset>("DefaultMenu");
            defaultNavbarTemplate = Resources.Load<VisualTreeAsset>("DefaultNavbar");
        }
    }
}
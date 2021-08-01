using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

#if  UNITY_EDITOR
namespace com.chwar.xrui
{
    public class MenuActions : EditorWindow
    {
        private bool bShow = true;
        
        [MenuItem("XR UI/Add XR UI Element/Card", false, 1)]
        static void AddCard()
        {
            VisualTreeAsset card = Resources.Load<VisualTreeAsset>("DefaultCard");
            XRUIEditor.AddCard(card);
        }
        
        [MenuItem("XR UI/Add XR UI Element/List", false, 2)]
        static void AddList()
        {
            VisualTreeAsset list = Resources.Load<VisualTreeAsset>("DefaultList");
            XRUIEditor.AddList(list);
        }
        
        [MenuItem("XR UI/Add XR UI Element/Navbar", false, 3)]
        static void AddNavbar()
        {
            VisualTreeAsset navbar = Resources.Load<VisualTreeAsset>("DefaultNavBar");
            XRUIEditor.AddNavbar(navbar);
        }

        [MenuItem("XR UI/Add XR UI Element/Menu", false, 4)]
        static void AddMenu()
        {
            VisualTreeAsset menu = Resources.Load<VisualTreeAsset>("DefaultMenu");
            XRUIEditor.AddMenu(menu);
        }
        
        [MenuItem("XR UI/Add XR UI Element/Custom UI Element", false, 15)]
        static void AddCustomElement()
        {
            GetWindowWithRect<MenuActions>(new Rect(0,0,1,1), false, "Choose Custom UI Template");
        }

        [MenuItem("XR UI/Switch Reality.../PC")]
        static void SwitchToPC()
        {
            // Switch to Windows/Linux/Mac standalone build.
            if(Application.platform == RuntimePlatform.WindowsEditor)
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);
            else if(Application.platform == RuntimePlatform.LinuxEditor)
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneLinux64);
            XRUIEditor.SetCurrentReality(XRUI.RealityType.PC);
        }
        
        [MenuItem("XR UI/Switch Reality.../AR (Android)")]
        static void SwitchToARAndroid()
        {
            // Switch to Android build.
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            XRUIEditor.SetCurrentReality(XRUI.RealityType.AR);
        }
        
        [MenuItem("XR UI/Switch Reality.../AR (iOS)")]
        static void SwitchToARiOS()
        {
            // Switch to iOS build.
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS); 
            XRUIEditor.SetCurrentReality(XRUI.RealityType.AR);
        }
        
        [MenuItem("XR UI/Switch Reality.../VR")]
        static void SwitchToVR()
        {
            // Switch to Windows VR build.
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);
            XRUIEditor.SetCurrentReality(XRUI.RealityType.VR);
        }

        private void OnGUI()
        {
            if (bShow)
            {
                bShow = false;
                EditorGUIUtility.ShowObjectPicker<VisualTreeAsset>(null, false, "", 0);
            }

            if (Event.current.commandName == "ObjectSelectorSelectionDone")
            {
                VisualTreeAsset element = (VisualTreeAsset) EditorGUIUtility.GetObjectPickerObject();
                //if(element != null)
                    //EditorController.AddXRUIElement(element);
            }

            if (Event.current.commandName == "ObjectSelectorClosed")
            {

            }
        }
    }
}
#endif
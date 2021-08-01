using UnityEngine;

namespace com.chwar.xrui.Editor.Inspector
{
    //[CustomEditor(typeof(XRUICard))]
    //[CanEditMultipleObjects]
    public class XRUICardInspector : MonoBehaviour
    {
        //public PropertyField TitleText;
        private void OnEnable()
        {
            //TitleText = new PropertyField(serializedObject.FindProperty("TitleText"));
        }

        /*public void OnInspectorGUI()
        {
            /*serializedObject.Update();
            TitleText.stringValue = EditorGUILayout.TextField("Test", TitleText.stringValue);
            Debug.Log("bruh");
            serializedObject.ApplyModifiedProperties();#1#
            //TitleText.RegisterCallback<ChangeEvent<string>>(x => { ((XRUICard)serializedObject.targetObject).UpdateUI(); });
        }*/
        
    }
}

using System.Collections;
using com.chwar.xrui.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.chwar.xrui
{
    public class Demo : MonoBehaviour
    {
        IEnumerator Start()
        {
            var menu = FindObjectOfType<XRUIMenu>().gameObject;
            menu.SetActive(false);
            
            yield return new WaitForSeconds(1);
            XRUI.Instance.ShowAlert(XRUI.AlertType.Primary, "Primary message.");
            yield return new WaitForSeconds(1);
            XRUI.Instance.ShowAlert(XRUI.AlertType.Success, "Success message.");
            yield return new WaitForSeconds(1);
            XRUI.Instance.ShowAlert(XRUI.AlertType.Warning, "Warning message.");
            yield return new WaitForSeconds(1);
            XRUI.Instance.ShowAlert(XRUI.AlertType.Danger, 	"Error message.");
            yield return new WaitForSeconds(1);
            XRUI.Instance.ShowAlert(XRUI.AlertType.Info, 	"Info message.");

            var list = FindObjectOfType<XRUIList>();
            list.AddElement(false).Q<Label>("Text").text = "List element";
            list.AddElement(true).Q<Label>("Text").text = "Selected list element";
            list.AddElement(false).Q<Label>("Text").text = "List element";
            
            yield return new WaitForSeconds(2);
            menu.SetActive(true);
            yield return new WaitForSeconds(1);

            var inspectorModal = XRUI.Instance.modals[0];
            XRUI.Instance.CreateModal(inspectorModal.modalName, inspectorModal.mainTemplate, null, null);
        }
    }
}

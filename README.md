# XRUI Framework
[![Unity 2021.2+](https://flat.badgen.net/badge/unity/2021.2+)](https://unity3d.com/get-unity/download)
[![MIT](https://flat.badgen.net/badge/license/MIT/green)](./LICENSE)
[![Coverage](https://flat.badgen.net/badge/coverage/85%25/green)](./Tests)

XRUI is a responsive UI framework for making cross-platform XR applications with the Unity 3D editor. Its purpose is to assist users in creating efficient and adaptive UIs that can automatically adjust depending on the deployed platform and the environment's reality, supporting regular PC environment as well as AR and VR environments. This way, users only need to design and develop their UI once for all platforms, resulting in a great saving of time.   

XRUI is based on Unity's new UI system, [UI Toolkit](https://docs.unity3d.com/Manual/UIElements.html). Internally, it uses UXML and USS, so a basic knowledge and understanding of these technologies are required to use this framework. 

## Installation

1. In the package manager, click on `Add package from git URL` and insert the repository's URL: [https://github.com/chwar/XRUI.git](https://github.com/chwar/XRUI.git)
  * Alternatively, you can unzip and import the package manually.

2. Add the XRUI controller to your scene by navigating to `XRUI > Add XRUI Controller`. You can also create an empty game object and attach the main XRUI script (`XRUI.cs`). This script is a singleton flagged as `DontDestroyOnLoad`. It contains the main API that can be easily accessed through the instance:
```csharp
using com.chwar.xrui;

void Start(){
    // XRUI.Instance...
}
```
3. The package uses a default configuration that references the default UXML templates for UI elements. You can create your own by navigating to `Assets > Create > XRUI > Create XRUI Configuration asset`. You can then override the default templates for UI elements with your own (see [Custom UI Elements](#custom-ui-elements)). Don't forget to reference your own XRUI configuration asset to the XRUI controller.

## UI Elements

![Screenshot from 2021-08-03 18-43-15](https://user-images.githubusercontent.com/25299178/128054028-87a27934-1dad-4377-9b35-4ded3e8855d2.png)


XRUI provides a few UI Elements. The style is minimalistic and inspired from [Bootstrap](https://getbootstrap.com). You can add them in your project by navigating from the Unity menu to `XRUI > Add XRUI Element`. This creates a game object containing a `UIDocument` (which contains the UXML template and USS styles) and an XRUI script that matches the element. Add your own scripts to this object with a reference to the XRUI script to define the behaviour of the UI. 

XRUI elements are thought as basic containers for user content. Given the hierarchic nature of UXML, it is easy to append content within the UI elements at runtime. To easily access your UXML contents and append them into various XRUI elements, reference them in the intended UI Elements list within the XRUI controller:

![image](https://user-images.githubusercontent.com/25299178/150392915-51de8296-3522-4635-b419-bf2fed487808.png)


The list of UI elements is accessible within the XRUI controller's instance. Use the `GetUIElement` method for easy access:
```csharp
// Use the name of the VisualTreeAsset you put in the inspector list 
VisualTreeAsset myElement = XRUI.Instance.GetUIElement("MyElement");
```

### XRUI Element
<details>
<summary>Click to expand!</summary>

When adding UI Elements through the XRUI menu in Unity, the system uses the template referenced in the XRUI Configuration asset (see [Installation](#installation)). Each XRUI element script inherits from the `XRUIElement` class, which comes with some useful generic methods.

To add or remove visual elements from the UI element, call these methods:
```csharp
VisualElement myElement = someVisualTreeAsset.Instantiate();
XRUICard card = GetComponent<XRUICard>();

card.AddUIElement(myElement, "MyCardContainer");
card.RemoveUIElement(myElement); 
```

You can also show or hide XRUI elements at any time:
```csharp
card.Show(true);    // Display.Flex
card.Show(false);   // Display.None

card.Show(myElement, false); // Hides some of the content  
``` 

> Note: Keep in mind that hidden elements will not be found with a regular QML query, as they are hidden. You can still find them by either keeping a reference to the visual element in your code, or by querying it like this: 
> 
> ```csharp
> card.Query<TemplateContainer>().Where(ve => 
>	ve.style.display.value.Equals(DisplayStyle.None)).First();
> ```
</details>
	
### XRUI Menu
<details>
<summary>Click to expand!</summary>
	
<table>
	<tr>	
		<td><img src="https://user-images.githubusercontent.com/25299178/150380152-620c7abc-c9ed-4f59-a5c9-1c290cb188da.png"  alt="pc" width = 360px></td>
		<td><img src="https://user-images.githubusercontent.com/25299178/150380261-88113fc8-5787-45be-891e-c3c63884a5f4.png" alt="ar" width = 360px></td>
		<td><img src="https://user-images.githubusercontent.com/25299178/150380451-dcdd3686-2f91-40c5-99da-9114d3119784.png" alt="vr" width = 360px></td>
	</tr> 
	<tr>
		<td align="center">PC</td>
		<td align="center">AR</td>
		<td align="center">VR</td>
	</tr>
</table>

The provided XRUI Menu template is designed as a side menu that collapses out of the view frustum. It can be configured in the inspector (see screenshot above).
	
The list element template is the UXML template that is used to create entries. You can provide a template with a simple button, or more complex compositions with images, text, buttons, etc. to suit your needs.
	
Add entries to your menu:
```csharp
var menu = GetComponent<XRUIMenu>();
// The menu returns the created entry to be configured
var element = menu.AddElement();
element.Q<Label>("MyElementLabel").text = "myLabelTitle";
```
</details>

### XRUI List
<details>
<summary>Click to expand!</summary>

<table>
	<tr>	
		<td><img src="https://user-images.githubusercontent.com/25299178/150381681-5008c5e6-8656-436d-ad37-5673c88eaf9f.png"  alt="pc" width = 360px></td>
		<td><img src="https://user-images.githubusercontent.com/25299178/150381664-4cf1bd87-ec84-429e-9d75-035f361b3aed.png" alt="ar" width = 360px></td>
		<td><img src="https://user-images.githubusercontent.com/25299178/150381684-7843e2c5-fd60-4149-b9ad-2e21ea66c5db.png" alt="vr" width = 360px></td>
	</tr> 
	<tr>
		<td align="center">PC</td>
		<td align="center">AR</td>
		<td align="center">VR</td>
	</tr>
</table>

The XRUI List works in the same way as the menu:
```csharp
var list = GetComponent<XRUIList>();
// The list returns the created entry to be configured
var element = list.AddElement();
element.Q<Label>("MyElementLabel").text = "myLabelTitle";
```
</details>

### XRUI Navbar
<details>
<summary>Click to expand!</summary>
	
<table>
	<tr>	
		<td><img src="https://user-images.githubusercontent.com/25299178/150387236-07cf2ba4-d59a-47fa-b10d-1d7f2270b4aa.png"  alt="pc" width = 360px></td>
		<td><img src="https://user-images.githubusercontent.com/25299178/150387230-8ae337ba-cc53-4e56-aba8-61972b07cef9.png" alt="ar" width = 360px></td>
		<td><img src="https://user-images.githubusercontent.com/25299178/150387239-d4f12053-72e1-444c-8666-074dc7cda6ad.png" alt="vr" width = 360px></td>
	</tr> 
	<tr>
		<td align="center">PC</td>
		<td align="center">AR</td>
		<td align="center">VR</td>
	</tr>
</table>
	
The provided navbar is a very simple dark top bar. Since XRUI does not provide any third-party assets, it is provided empty. However, the default template contains a row of buttons (three justified on the left side, one justified on the right side) to get you started. Since navbar designs can be very different, the adopted solution was to propose a very generic template to fit the most users. You could use the template as a base to add your own elements (buttons, dropdowns, labels) to tailor the navbar to your needs.
</details>

### XRUI Card
<details>
<summary>Click to expand!</summary>

<table>
	<tr>	
		<td><img src="https://user-images.githubusercontent.com/25299178/150382706-dfcca9f6-28d4-49a6-b104-1f13476e86b3.png"  alt="pc" width = 360px></td>
		<td><img src="https://user-images.githubusercontent.com/25299178/150382701-ea5b0e55-4bbe-4cac-9db5-7ed07fb47afa.png" alt="ar" width = 360px></td>
		<td><img src="https://user-images.githubusercontent.com/25299178/150382708-57b80239-35e1-483d-a11d-77f1f93865ab.png" alt="vr" width = 360px></td>
	</tr> 	
	<tr>
		<td align="center">PC</td>
		<td align="center">AR</td>
		<td align="center">VR</td>
	</tr>
</table>

The XRUI Card is floating on the right corner by default, and sticks to the bottom of the screen in AR mode. You can specify custom dimensions from the inspector. Use the `AddUIElement` method (see [XRUI Element](#xrui-element)) to fill the card with content. The template's default container is named `"MainContainer"`. 
</details>
	
### XRUI Modals
<details>
<summary>Click to expand!</summary>

<table>
	<tr>	
		<td><img src="https://user-images.githubusercontent.com/25299178/150383323-b82a2a4d-3565-4d58-8695-9fef29920ffb.png"  alt="pc" width = 360px></td>
		<td><img src="https://user-images.githubusercontent.com/25299178/150383320-c52537d2-c74a-4c9d-8dd8-bc7a5955f92d.png" alt="ar" width = 360px></td>
		<td><img src="https://user-images.githubusercontent.com/25299178/150383326-c13b93d2-bbfc-4286-83c7-2361fbfea257.png" alt="vr" width = 360px></td>
	</tr> 	
	<tr>
		<td align="center">PC</td>
		<td align="center">AR</td>
		<td align="center">VR</td>
	</tr>
</table>

XRUI creates modals at runtime rather than requiring you to create all of them in the editor in order to save resources.

Given the hierarchic nature of UXML, modals are easy to reproduce. XRUI provides one XR Modal template, which consists of a title, empty container, two buttons (main and secondary) sticking at the bottom, and a closing button in the top right corner. You can use this template and fill its container dynamically at runtime.

In Unity, you can reference your modals in the intended list:

![Screenshot from 2021-08-03 17-35-39](https://user-images.githubusercontent.com/25299178/128054683-9a34f51f-f440-40a1-b6e5-f213e6a204dd.png)

The name given to each modal entry can be used to find the matching template and create a modal from it, with the `CreateModal` method:

```csharp
// Adapt the namespace to your own
Type t = Type.GetType("myModalScript");
XRUI.Instance.CreateModal("DemoModal", t);
```
> Note: The user script type has to be passed outside of the XRUI package, because Unity packages can't access the Assembly-CSharp assembly, i.e. can't find user namespaces, and hence, can't find user scripts located in the Assets automatically. It's also not possible to reference it through the inspector, as it only accepts instances of a script and not the script itself.

This creates a modal game object on which the `XRUIModal` script is attached, as well as a `UIDocument` script that contains the main template. You can access the modal system's API through the `XRUIModal` script.
The user script type is used to create an instance of said script when the modal is created. This lets you define the behaviour of your elements. One approach is to create one method per page, and to setup event handlers on your buttons to navigate them. To create modal pages, use the `UpdateModalFlow` method. Its last parameter is a callback function that is fired once upon the page's creation.

```csharp
private XRUIModal _xruiModal;
private UIDocument _uiDocument;

void Start() {
    _xruiModal = GetComponent<XRUIModal>();
    _uiDocument = GetComponent<UIDocument>();
    StartPage();
}

void StartPage() {
    _xruiModal.UpdateModalFlow("MyModalPage", "MainContainer", () =>
    {
        // This callback is only fired once, when the page is created for the first time
        // Put here initialization code, event subscriptions, etc. 

        Button myButton = UIDocument.rootVisualElement.Q<Button>("myButton");
        _myButton.clicked += MyPage;
    });

    // Content to execute every time this page is opened.
}

void MyPage() {
    // ...
}
```

### Using the default modal template
You can use the default modal template that comes with the package and fill it with your own content. It consists of a title, empty container, two buttons (main and secondary) sticking at the bottom, and a closing button in the top right corner. You can add your content to the container by referencing it by name (`MainContainer`) to the `UpdateModalFlow` method, as per the example above. You can manipulate the buttons and change the title through the `XRUIModal` API.

Access the modal's public fields to change the title of the modal, the text of the buttons, or to set the icon of the top right close button:
```csharp
_xruiModal.ModalTitle.text = "Create a new project";
_xruiModal.CancelButton.text = "Cancel";
_xruiModal.ValidateButton.text = "Finish";
_xruiModal.ValidateButton.visible = true;
```

Change the justification of the bottom buttons with the `SetButtonsPlacement` method:
```csharp
// Supported options: FlexStart, FlexEnd, Center, Space Between, Space Around
_xruiModal.SetButtonsPlacement(Justify.Center);
```

Set the action of the cancel and validation buttons:
```csharp
// These methods take an action as parameter, but you can provide a function call

_xruiModal.SetCancelButtonAction(XRUIModal.Destroy);
_xruiModal.SetValidateButtonAction(CreateProject);
```
> Note: prefer these methods to the direct access to `ValidationButton.clicked` or `CancelButton.clicked`, as the methods replace any other event subscription with the provided action. This means that a click on either button can have only one action on a given page. 

Destroy the modal:
```csharp
_xruiModal.Destroy();
```


#### Form validation
XRUI supports basic form validation by letting you define required fields. For now, only text fields are supported, i.e. XRUI determines if required text fields are empty or not.

```csharp
_xruiModal.SetRequiredFields(_fieldOne, _fieldTwo, _fieldThree);
```

You can pass as many fields as you want in one call. Internally, XRUI checks the page where each indicated field is contained in. When a user is on a page containing required fields, the modal's validation button is disabled until all required fields contain an input. Additionally, you can subscribe your own validation methods to the validation button and flag text fields with errors to indicate users the fields to correct.  

```csharp
_xruiModal.SetFieldError(_fieldWithError);
```
</details>
	
### XRUI Alerts
<details>
<summary>Click to expand!</summary>
<table>
	<tr>	
		<td><img src="https://user-images.githubusercontent.com/25299178/150383854-a98bc873-a574-4285-bc93-c38c4833fadf.png"  alt="pc" width = 360px></td>
		<td><img src="https://user-images.githubusercontent.com/25299178/150383853-723ba5af-312f-428d-9734-a3ad430b3411.png" alt="ar" width = 360px></td>
		<td><img src="https://user-images.githubusercontent.com/25299178/150383857-6adaa27b-23f6-4f33-a472-21eb9511af78.png" alt="vr" width = 360px></td>
	</tr> 
	<tr>
		<td align="center">PC - Primary</td>
		<td align="center">AR - Success</td>
		<td align="center">VR - Warning</td>
	</tr>
</table>
	
![Peek 2021-08-03 00-03](https://user-images.githubusercontent.com/25299178/127934108-1784dc2d-36d3-4452-8119-3f910f9a258a.gif)
![Peek 2021-08-03 00-05](https://user-images.githubusercontent.com/25299178/127934111-57e1859b-5900-4487-995f-9d3f55e8da68.gif)

The provided alert template sets them as floating cards in the right corner of the screen when in PC and VR mode, and as notifications at the top of the screen in AR mode. They also come with animations to attract the attention of users. You can show alerts for different purposes; the types of alerts are inspired from [Bootstrap](https://getbootstrap.com/docs/5.0/components/alerts/).   


Show alerts using the `ShowAlert` method:

```csharp
XRUI.Instance.ShowAlert(XRUI.AlertType.Primary, "Primary message.");
XRUI.Instance.ShowAlert(XRUI.AlertType.Success, "Success message.");
XRUI.Instance.ShowAlert(XRUI.AlertType.Warning, "Warning message.");
XRUI.Instance.ShowAlert(XRUI.AlertType.Danger, 	"Error message.");
XRUI.Instance.ShowAlert(XRUI.AlertType.Info, 	"Info message.");
```

You can also provide a title:

```csharp
XRUI.Instance.ShowAlert(XRUI.AlertType.Primary, "Title", "Primary message.");
```
	
You can also give a callback, which will be triggered upon clicking the alert:
	
```csharp
 XRUI.Instance.ShowAlert(XRUI.AlertType.Primary, "Click me!", "Click to trigger callback", () => MyCallback());
```
</details>
	
	
### XRUI Contextual Menu
<details>
<summary>Click to expand!</summary>
<table>
	<tr>	
		<td><img src="https://user-images.githubusercontent.com/25299178/150390186-af38fbfb-efa2-4bd0-bf69-d17e69835674.png"  alt="pc" width = 360px></td>
		<td><img src="https://user-images.githubusercontent.com/25299178/150390183-61c38a42-88f2-4ef3-90d4-ac5586272fd8.png" alt="ar" width = 360px></td>
	</tr> 
	<tr>
		<td align="center">PC</td>
		<td align="center">AR (identical)</td>
	</tr>
</table>
	
XRUI can create contextual menus dynamically. Here, PC and AR styles are identical: the contextual menu is shown as a floating list. VR is currently not supported for this template. Similarly to the menu and list templates, a menu element template is also given to create entries in the contextual menu. Because the entries are context-dependent, they need to be generated dynamically at runtime.
	
The `ShowContextualMenu` method needs at least the x and y coordinates of the parent element (i.e. the element that was interacted which caused the contextual menu to appear), and a boolean to indicate whether or not the styling should include an arrow pointing at the parent element. A first overload gives the possibility to provide a custom template. A second overload lets developers provide left and right offsets for finer tuning of the menu’s position. The method returns a `XRUIContextualMenu` instance, which is required to add entries to the menu.
	
```csharp
var myElement = GetComponent<UIDocument>();
var myBtn = myElement.rootVisualElement.Q<Button>("myBtn");
	
// Define parent coordinates for menu positioning
Vector2 parentCoordinates = new Vector2(myBtn.worldBound.x, myBtn.worldBound.y);
	
// For using a custom template
VisualTreeAsset myTemplate = Resources.Load<VisualTreeAsset>("myTemplate");
	
myBtn.clicked +=
{
	var menu;
	// Use one of these overloads
	// Call with 2 parameters: parent coord. and whether to show a pointing arrow
	menu = XRUI.Instance.ShowContextualMenu(parentCoordinates, false);
	
	// 3 parameters: provide a custom template as 1st param.
	menu = XRUI.Instance.ShowContextualMenu(myTemplate, parentCoordinates, true);
	
	// 5 parameters: provide left and right offsets in pixels
	menu = XRUI.Instance.ShowContextualMenu(myTemplate, parentCoordinates, true, 50,
	100);
}
	
// To override the default menu entry template
menu.menuElementTemplate = Resources.Load<VisualTreeAsset>("myEntryTemplate");
	
// Add entries to the contextual menu
var entry = contextualMenu.AddMenuElement();
```
In addition, the contextual menu considers the available space on screen. By default, contextual menus will attempt to display on the right of the parent element. However, if there is no available space, they are displayed on the left instead.
	
</details>
	
## Automatic XR adaptation
XRUI's main functionality is to provide responsiveness for different build targets and XR variants. This is done by adapting all XRUI elements automatically when the build target is changed, by triggering XRUI related USS classes at runtime and in the Unity Editor.

To change the XR variant, change the reality value in the XRUI controller:

![image](https://user-images.githubusercontent.com/25299178/150392381-514b08ec-335f-4762-a3e8-70f6752b1b7b.png)

The XRUI API provides a method to assess the current XR variant. You can use it to do target-specific manipulations like so:

```csharp
if(XRUI.IsCurrentReality(XRUI.RealityType.AR)) {
    // AR specific code here
}
``` 

## XRUI Grid System
In order to organize easily and efficiently UI elements on screen, XRUI makes use of a grid system. You can use it by navigating to `XRUI > Add XRUI Grid`. In the Unity editor, you can group UI components inside rows through the scene hierarchy. The `XRUIGridController` component is attached to the root of the grid, and contains the list of all rows. A weighting system allows you to define which rows should take which amount of space (this uses the `flex-grow` attribute of CSS/USS Flexbox). 

For example, a top navbar can be setup in one row, with a weight of 0, i.e., it should not "grow"--as in, take space--more than its initial size. A second row containing the rest of the on-screen UI can have a weight of 1, i.e. it should take more of the available space than what its initial size requires. Since there are two rows and the first row has a weight of 0, this results in the second row using all remaining screen space. Horizontally, elements are contained in absolute containers, which mean they all take the entire horizontal space and can therefore overlap. 

<!-- ![Peek 2021-08-04 17-08](https://user-images.githubusercontent.com/25299178/128205987-c9fcad0c-9639-4de9-902b-1a7141320a38.gif) -->
<img src="https://user-images.githubusercontent.com/25299178/128205987-c9fcad0c-9639-4de9-902b-1a7141320a38.gif"  alt="pc" width = 700px>
<img src="https://user-images.githubusercontent.com/25299178/128047151-b90c0e4f-0a09-4a64-b54b-8d011ccba3ac.png"  alt="pc" width = 500px>
	
<!-- ![Screenshot from 2021-08-03 17-54-43](https://user-images.githubusercontent.com/25299178/128047151-b90c0e4f-0a09-4a64-b54b-8d011ccba3ac.png) -->
    
> Note: In case all UI elements within a row are absolute, the row's height becomes zero, because its USS property is set to `height: auto`. You should then indicate a minimum height in the indicated field to obtain the expected behaviour.

</details>

## Custom UI Elements
You can create your own UXML templates and refer them in the XRUI Configuration asset. You should however be careful in naming your elements, should you want to inherit the functionalities provided by the default UI elements. You can check them with Unity's UI Builder, or you can simply duplicate the UXML files and start working from here.

### USS styles
XRUI comes with its own set of styles that are imported just after Unity's in UI Toolkit's pipeline. They are imported through a theme file which is used in the provided Panel Settings asset (also linked in the XRUI Configuration asset). You can add your own root styles to this theme file, override the root XRUI styles, or remove some of the imported assets if you don't need them. Should you want to inherit some of the XRUI styles for your own UI elements, you can add the USS class `.xrui` to the desired root visual elements. Additionally, each XRUI component has its own class that uses the BEM methodology, as per Unity's recommendation. They are the following:

|XRUI Element|USS Class|
|:---:|:---:|
|Menu|`.xrui__menu`|
|Menu item|`.xrui__menu__item`|
|List|`.xrui__list`|
|List item|`.xrui__list__item`|
|Navbar|`.xrui__navbar`|
|Card|`.xrui__card`|
|Alert|`.xrui__alert`|
|Modal|`.xrui__modal`|
|Contextual Menu|`.xrui__contextual-menu `|
	

# XRUI
This is an extension for the Unity 3D editor. Its purpose is to assist users in creating efficient and adaptive UIs that can automatically adjust depending on the deployed platform and the environment's reality, supporting regular PC environment as well as AR and VR environments. This way, users only need to design and develop their UI once for all platforms, resulting in a great saving of time.   

XRUI is based on Unity's new UI system, [UI Toolkit](https://docs.unity3d.com/Manual/UIElements.html). Internally, it uses UXML and USS, so a basic knowledge and understanding of these technologies are required to use this framework. 

## Installation

1. Unzip and import the package from the Package manager.

2. The package uses a default configuration that references the default UXML templates for UI elements. 
You can create your own by navigating to `Assets > Create > XRUI > Create XRUI Configuration asset`.
You can then override the default templates for UI elements with your own (see [Custom UI Elements](#custom-ui-elements)).

3. Create an empty game object and attach the main XRUI script (`XRUI.cs`). This script is a singleton flagged as `DontDestroyOnLoad`. It contains the main API that can be easily accessed through the instance:
```csharp
using com.chwar.xrui;

void Start(){
    // XRUI.Instance...
}
```

## UI Elements

![Screenshot from 2021-08-03 00-56-47](https://user-images.githubusercontent.com/25299178/127934016-343c5483-7143-427a-891f-46cd80c94f2f.png)

XRUI provides a few UI Elements. The style is minimalistic and inspired from [Bootstrap](https://getbootstrap.com). You can add them in your project by navigating from the Unity menu to `XRUI > Add XRUI Element`. This creates a game object containing a `UIDocument` (which contains the UXML template and USS styles) and an XRUI script that matches the element. Add your own script to this object with a reference to the XRUI script to define the behaviour of the UI. 

When adding UI Elements through the XRUI menu in Unity, the system uses the template referenced in the XRUI Configuration asset (see [Installation](#installation)).

Each XRUI UI element script inherits from the `XRUIElement` class, which comes with some useful generic methods.

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
> `card.Query<TemplateContainer>().Where(ve => ve.style.display.value.Equals(DisplayStyle.None)).First();`

### Menu
<img src="https://user-images.githubusercontent.com/25299178/127934036-f7c40049-072c-420b-ac45-1125b7f0cd30.png" alt="XRUI Menu" height="500"/>

### Navbar

### Card

### List

### Modals
<img src="https://user-images.githubusercontent.com/25299178/127934127-dace8111-714b-44a5-a7c3-d9e0a1d2b240.png" alt="PC Modal" height="500"/> <img src="https://user-images.githubusercontent.com/25299178/127934131-e1f4e84f-e108-45ef-ab88-2f3f17c36ce8.png" alt="AR Modal" height="500"/>

XRUI creates modals at runtime rather than requiring you to create all of them in the editor in order to save resources.

Given the hierarchic nature of UXML, modals are easy to reproduce. XRUI provides one XR Modal template, which consists of a title, empty container, two buttons (main and secondary) sticking at the bottom, and a closing button in the top right corner. You can use this template and fill its container dynamically at runtime.

In Unity, you can reference your modals in the intended list:

<!-- Image -->
This creates `InspectorModal` objects which are accessible through the XRUI instance. The given name can be used in the code to fetch the template and contents at the time of generation. You can then pass them to the `CreateModal` method:

```csharp
public void ShowModal(string modalName)
{
    InspectorModal m = XRUI.Instance.modals.Find(modal => modal.modalName.Equals(modalName));
    Type t = Type.GetType($"Modals.{modalName}");
    if (t is null)
    {
        throw new ArgumentException($"No modal script matching the template '{modalName}' could be found.");
    }
    XRUI.Instance.CreateModal(modalName, m.mainTemplate, m.modalFlowList, t);
}
```
> Note: This function is placed outside the XRUI package (i.e. not provided in the package). This is because Unity packages can't access the Assemply-CSharp assembly, i.e. can't find user namespaces, and hence, can't find user scripts located in the Assets automatically.

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
		// Put here initialization code, event subscribtions, etc. 

		Button myButton = UIDocument.rootVisualElement.Q<Button>("myButton");
		_myButton.clicked += MyPage;
	});

	// Content to execute everytime this page is opened.
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

### Alerts
![Peek 2021-08-03 00-03](https://user-images.githubusercontent.com/25299178/127934108-1784dc2d-36d3-4452-8119-3f910f9a258a.gif)
![Peek 2021-08-03 00-05](https://user-images.githubusercontent.com/25299178/127934111-57e1859b-5900-4487-995f-9d3f55e8da68.gif)

The provided alert template sets them as floating cards in the right corner of the screen when in PC mode, as notifications at the top of the screen in AR mode, and as \[VR style\] <!-- TODO future VR style --> in VR mode. They also come with animations to attract the attention of users. You can show alerts for different purposes; the types of alerts are inspired from [Bootstrap](https://getbootstrap.com/docs/5.0/components/alerts/).   

<!-- TODO Add image -->

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

## Automatic XR adaptation
XRUI's main functionality is to provide responsiveness for different build targets and XR variants. This is done by adapting all XRUI elements automatically when the build target is changed, by triggering XRUI related USS classes at runtime and in the Unity Editor.

To change the XR variant, navigate to `XRUI > Switch Reality`. In some cases, it changes the build target -- notably for mobile AR.

The XRUI API provides a method to assess the current XR variant. You can use it to do target-specific manipulations like so:

```csharp
if(XRUI.GetCurrentReality().Equals("ar")) {
    // AR specific code here
}
``` 

## Grid System
In order to organize easily and efficiently UI elements on screen, XRUI Framework makes use of a grid system. In the Unity editor, you can group UI components in rows through the scene hierarchy. The `XRUIGridController` component is attached to the root of the grid, and contains the list of all rows. A weighting system allows users to define which rows should take which amount of space (this uses the `flex-grow` attribute of CSS/USS Flexbox). 

For example, a top navbar can be setup in one row, with a weight of 0, i.e., it should not "grow"--as in, take space--more than its initial size. A second row containing the rest of the onscren UI can have a weight of 1, i.e. it should take more of the available space than what its initial size requires. Since there are two rows and the first row has a weight of 0, this results in the second row using all remaining screen space. Vertically, elements share the space equally by default (i.e., two elements in a row will each use 50% of the screen width).  

<!-- Image -->

## Custom UI Elements
You can create your own UXML templates and refer them in the XRUI Configuration asset. You should however be careful in naming your elements, should you want to inherit the functionalities provided by the default UI elements. You can check them with Unity's UI Builder, or you can simply duplicate the UXML files and start working from here.

### USS styles
XRUI comes with its own set of styles that are imported just after Unity's in UI Toolkit's pipeline. They are imported through a theme file which is used in the provided Panel Settings asset (also linked in the XRUI Configuration asset). You can add your own root styles to this theme file, override the root XRUI styles, or remove some of the imported assets if you don't need them. Should you want to inherit some of the XRUI styles for your own UI elements, you can add the USS class `.xrui` to the desired root visual elements. Aditionally, each XRUI component has its own class that uses the BEM methodology, as per Unity's recommendation. They are the following:

|XRUI Element|USS Class|
|:---:|:---:|
|Menu|`.xrui__alert`|
|List|`.xrui__list`|
|Navbar|`.xrui__navbar`|
|Card|`.xrui__card`|
|Alert|`.xrui__alert`|
|Modal|`.xrui__modal`|

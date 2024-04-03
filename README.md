# dc-device01
Code to data collection device
Entry point (Sequence of run) 

### Files in ZpenSample Directory:

- **bin/**
  - Contains compiled binaries.
    - **Debug/**
      - Debug build output.
    - **Release/**
      - Release build output.

- **obj/**
  - Contains intermediate build files.
    - **Debug/**
      - Debug build intermediate files.
    - **Release/**
      - Release build intermediate files.

- **properties/**
  - Contains project properties files.
    - `AssemblyInfo.cs`: Contains assembly-level attributes.
    - `Resource.Designer.cs`: Auto-generated code for strongly-typed resources.
    - `Resource.resx`: Resource file.
    - `Settings.Designer.cs`: Auto-generated code for settings.
    - `Settings.settings`: Settings file.

- `app.config`: Application configuration file.

- `DataPassByStaticField.cs`: C# file defining a class for data passing by static fields.

- `FromZopenSample.cs`: C# file containing the main logic of the ZpenSample application.

- `FromZopenSample.Designer.cs`: Auto-generated code related to the design of FromZopenSample form.

- `FromZopenSample.resx`: Resource file for FromZopenSample form.

- `Program.cs`: C# file containing the entry point of the application.

- `USBWatcher.cs`: C# file defining a class for watching USB devices.

- `ZinPointF.cs`: C# file defining a class for representing a point in Zin format.

- `ZpenSample.csproj`: Project file for the ZpenSample application.



### 1: Program.cs

#### Using Directives:
- `System`: Contains fundamental types and base types defining commonly-used value and reference data types, events and event handlers, interfaces, attributes, and processing exceptions.
- `System.Windows.Forms`: Contains classes for creating Windows-based applications that utilize the rich user interface features of the Windows OS.

#### Namespace Declaration:
- The `ZpenSample` namespace contains the Program.cs file.

#### Program Class:
- `Program` is a static class containing the Main method, serving as the entry point for the application.

#### Main Method:
- The Main method is marked with the `[STAThread]` attribute, indicating the application's COM threading model is single-threaded apartment (STA).
- Actions performed in Main:
  - `Application.EnableVisualStyles()`: Enables visual styles for the application's controls, allowing it to use OS-provided visual styles.
  - `Application.SetCompatibleTextRenderingDefault(false)`: Sets the default text rendering mode to be compatible with the OS default, using GDI+ for text rendering.
  - `Application.Run(new FormZpenSample())`: Starts the application by creating and running an instance of the FormZpenSample form.


 

### 2: FormZpenSample.cs

#### Using Directives:
- Various namespaces are included, such as System, System.Windows.Forms, and System.Runtime.InteropServices.

#### Namespace Declaration:
- The code is within the ZpenSample namespace.

#### Class Declaration:
- Declares the FormZpenSample class, which inherits from Form and represents the main form of the application.

#### DllImport Attribute:
- Used to indicate that methods are implemented in unmanaged code (in zpen.dll), allowing managed code to call unmanaged functions.

#### Static Variables and Constants:
- Contains static variables and constants used throughout the code, including device connection status, pen data, command sizes, etc.

#### Constructor:
- Initializes the form and calls InitViews() and StartGetData() methods.

#### Methods:
- InitDongle(): Initializes Bluetooth AP connection status.
- StartGetData(): Opens and initializes devices and starts a thread for receiving HID data.
- ReceiveHidDataFromDongle(): Thread function for continuously receiving HID data.
- SetTextSafePost(): Processes received HID data and updates UI elements.
- DecodeData(): Decodes raw data into ZigPointF objects.
- DrawInBitmap(): Draws received pen data onto PictureBox.
- GetMacAddress(), GetClientListFromFile(), TwoDimensionToOnedDimension(): Methods for reading configuration files.
- InitViews(): Initializes UI elements.
- CheckDongleIsInsert(): Checks if the dongle is inserted.
- USBEventHandler(): Handles USB events.

#### Event Handlers:
- FormZpenSample_Load(), FormZpenSample_FormClosed(): Handle form load and form closed events.

#### Flow of the Code:
- Begins with namespace declaration and using directives.
- Defines the FormZpenSample class representing the main form.
- Declares static variables and constants.
- Imports external methods from zpen.dll.
- Initializes the form and starts data reception.
- Handles USB events and UI updates.
- Closes the form when needed.

The code manages device initialization, data reception, processing, and UI updates based on received data. It also handles USB events related to dongle insertion/removal efficiently.

 

 

 
### 3: USBWatcher.cs

#### Namespace and Using Directives:
- Part of the ZpenSample namespace.
- Includes using directives to import necessary namespaces.

#### Structures:
- PnPEntityInfo: Defines a structure holding information about a Plug and Play device.
- USBControllerDevice: Defines a structure representing a USB controller device.

#### Partial Class USB:
- Contains methods for monitoring USB insertion and removal events and querying USB device information.

#### Fields:
- insertWatcher: ManagementEventWatcher object for monitoring USB insertion events.
- removeWatcher: ManagementEventWatcher object for monitoring USB removal events.

#### Methods:
- AddUSBEventWatcher: Adds event watchers for USB insertion and removal events.
- RemoveUSBEventWatcher: Removes USB event watchers.
- WhoUSBControllerDevice: Retrieves information about the USB controller device involved in an event.
- AllUsbDevices, WhoUsbDevice, and similar methods: Query information about USB devices based on various criteria.

#### Region UsbDevice and PnPEntity:
- Contains methods related to querying USB devices and Plug and Play entities, respectively.

#### Summary Comments:
- Each member has XML documentation comments providing descriptions and usage information.

#### Summary:
- Encapsulates functionality for monitoring USB events and retrieving information about USB devices and Plug and Play entities within the system.

 

 


 
 ### 4: ZigPointF.cs

The ZigPointF.cs file defines a class named ZigPointF, representing a point in a two-dimensional space with additional properties.

#### Fields:
- private float x: Represents the x-coordinate of the point.
- private float y: Represents the y-coordinate of the point.
- private int p: Represents an additional property p of type int.

#### Constructor:
- No explicit constructor is defined in the class.

#### Methods:
- SetX(float x): Sets the value of the x-coordinate.
- GetX(): Retrieves the value of the x-coordinate.
- SetY(float y): Sets the value of the y-coordinate.
- GetY(): Retrieves the value of the y-coordinate.
- SetP(int p): Sets the value of the additional property p.
- GetP(): Retrieves the value of the additional property p.

#### Commented-out code:
- There are commented-out fields (r and t) and their corresponding setter and getter methods (SetR, GetR, SetT, GetT). These suggest the consideration of additional properties (r and t) that were not implemented or used in the current version of the code.


### 5: DataPassByStaticField.cs

The DataPassByStaticField.cs file defines a class named DataPassByStaticField, which contains static fields used for passing data related to device types, Product IDs (PID), and Vendor IDs (VID).

#### Static Fields:
- deviceTypeUSB: Represents the device type USB.
- deviceTypeHID: Represents the device type HID.
- pid: Represents the string "PID", typically used as a label for Product IDs.
- vid: Represents the string "VID", typically used as a label for Vendor IDs.
- pid_value1, pid_value2, pid_value3: Represent three different Product ID values.
- vid_value1, vid_value2: Represent two different Vendor ID values.

These static fields are declared as public, allowing them to be accessed from outside the class without an instance of the class.

The purpose of these fields is to provide convenient access to commonly used device-related values such as device types, PID, and VID. These values can be accessed statically without instantiating the DataPassByStaticField class.

 
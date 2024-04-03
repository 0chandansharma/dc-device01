# dc-device01
Code to data collection device
Entry point (Sequence of run) 

ZpenSample - Root directory.
bin - Directory containing compiled binary files (output).
Debug - Debug build output directory.
Release - Release build output directory.
obj - Directory containing object files generated during compilation.
Debug - Debug build object files directory.
Release - Release build object files directory.
properties - Directory containing project properties.
AssemblyInfo.cs - Contains assembly-level attributes such as title, description, version, etc.
Resource.Designer.cs - Auto-generated file for strongly-typed resource access.
Resource.resx - Resource file for storing localized strings, images, etc.
Settings.Designer.cs - Auto-generated file for strongly-typed settings access.
Settings.settings - Settings file for storing application settings.
app.config - Configuration file for the application.
DataPassByStaticField.cs - C# file containing a class for handling data passing by static fields.
FromZopenSample.cs - C# file containing a class related to a sample from Zopen.
FromZopenSample.Designer.cs - C# file containing design-time code for the sample from Zopen.
FromZopenSample.resx - Resource file for the sample from Zopen.
Program.cs - C# file containing the entry point and main logic of the application.
USBWatcher.cs - C# file containing a class for watching USB devices.
ZinPointF.cs - C# file containing a class related to Zin points.
ZpenSample.csproj - Project file for the ZpenSample project.

ZpenSample
    -bin
        -Debug
        -Release
    -obj
        -Debug
        -Release
    -properties
        AssemblyInfo.cs
        Resource.Designer.cs
        Resource.resx
        Settings.Designer.cs
        Settings.settings
    app.config
    DataPassByStaticField.cs
    FromZopenSample.cs
    FromZopenSample.Designer.cs\
    FromZopenSample.resx
    Program.cs
    USBWatcher.cs
    ZinPointF.cs
    ZpenSample.csproj



1: Program.cs 

 

 

Using Directives: These directives bring in namespaces that contain classes and methods used in the code. For example, System contains fundamental types and base types that define commonly-used value and reference data types, events and event handlers, interfaces, attributes, and processing exceptions. System.Windows.Forms contains classes for creating Windows-based applications that take full advantage of the rich user interface features available in the Microsoft Windows operating system. 

Namespace Declaration: The namespace keyword is used to declare a scope that contains a set of related objects. In this case, the Program.cs file is in the ZpenSample namespace. 

Program Class: This is a static class named Program. It contains the Main method, which serves as the entry point for the application. 

Main Method: The Main method is the starting point for the application. It is marked with the [STAThread] attribute, which indicates that the COM threading model for the application is single-threaded apartment (STA). Inside Main, the following actions are performed: 

Application.EnableVisualStyles(): This enables visual styles for the application's controls. It allows your application to take advantage of the visual styles provided by the operating system. 

Application.SetCompatibleTextRenderingDefault(false): This sets the application's default text rendering mode to be compatible with the operating system's default mode. In this case, it's set to false, indicating that GDI+ will be used for text rendering. 

Application.Run(new FormZpenSample()): This starts the application by creating and running an instance of the FormZpenSample form. 

 

 

 

2: FormZpenSample.cs 

 

Using Directives: These directives bring in various namespaces required for the code to function properly. For example, System.Windows.Forms is required for working with Windows Forms, and System.Runtime.InteropServices is required for working with unmanaged code. 

Namespace Declaration: The code is within the ZpenSample namespace. 

Class Declaration: The FormZpenSample class is declared, which inherits from Form. This class represents the main form of the application. 

DllImport Attribute: This attribute is used to indicate that the methods are implemented in unmanaged code (in zpen.dll). It allows managed code to call unmanaged functions. 

Static Variables and Constants: The class contains various static variables and constants used throughout the code. These include variables for storing device connection status, offline pen data, HID command size, pen size, pressure sensitivity value, etc. 

Hashtable: Two Hashtable objects (hashtablePictureBox and hashtablePen) are used to store references to PictureBox and Pen objects, respectively. 

Constructor: The constructor initializes the form and calls InitViews() and StartGetData() methods. 

Methods: 

InitDongle(): Initializes the Bluetooth AP connection status. 

StartGetData(): Opens and initializes devices and starts a thread for receiving HID data. 

ReceiveHidDataFromDongle(): Thread function for continuously receiving HID data. 

SetTextSafePost(): Processes received HID data and updates UI elements accordingly. 

DecodeData(): Decodes raw data into ZigPointF objects. 

DrawInBitmap(): Draws received pen data onto the corresponding PictureBox. 

GetMacAddress(): Reads MAC addresses from a configuration file. 

GetClientListFromFile(): Reads ID list from a configuration file. 

TwoDimensionToOnedDimension(): Converts a 2D array to a 1D array. 

InitViews(): Initializes UI elements such as PictureBox and Label. 

CheckDongleIsInsert(): Checks if the dongle is inserted. 

USBEventHandler(): Event handler for USB events (insertion/removal). 

Event Handlers: 

FormZpenSample_Load(): Handles the form load event. 

FormZpenSample_FormClosed(): Handles the form closed event. 

This code appears to handle device initialization, data reception, processing, and UI updates based on the received data. It also handles USB events related to dongle insertion/removal. 

 

Flow of the CODE 

 

 

Namespace and Using Directives: The file starts with namespace declaration and using directives, bringing in necessary namespaces for the code. 

Class Declaration: The FormZpenSample class is declared, which represents the main form of the application. 

Static Variables and Constants: Various static variables and constants are defined, including arrays for storing PictureBoxes, Labels, Bitmaps, and Graphics objects. These variables are used throughout the class. 

DllImport Attribute: External methods from zpen.dll are declared using the DllImport attribute. These methods are used to interact with the pen device. 

Constructor: The constructor initializes the form and calls InitViews() and StartGetData() methods. 

InitDongle(): This method initializes the Bluetooth AP connection status by checking if the dongle is inserted. 

StartGetData(): This method opens and initializes devices and starts a thread for receiving HID data. 

ReceiveHidDataFromDongle(): This method runs in a separate thread continuously, receiving HID data and processing it. 

SetTextSafePost(): This method processes received HID data and updates UI elements accordingly. 

DecodeData(): This method decodes raw data into ZigPointF objects. 

DrawInBitmap(): This method draws received pen data onto the corresponding PictureBox. 

GetMacAddress(), GetClientListFromFile(), TwoDimensionToOnedDimension(): These methods are used to read MAC addresses and client lists from configuration files. 

InitViews(): This method initializes UI elements such as PictureBoxes and Labels, and associates MAC addresses with PictureBoxes. 

CheckDongleIsInsert(): This method checks if the dongle is inserted. 

USBEventHandler(): This method handles USB events such as insertion and removal of devices. 

Event Handlers: FormZpenSample_Load() and FormZpenSample_FormClosed() handle form load and form closed events, respectively. 

The flow of the code involves initializing the form, setting up device connections, handling data reception, processing and drawing pen data, managing device connections, and responding to USB events. The code is structured to handle these tasks efficiently and interact with the UI elements appropriately. 

 

 

 

3: USBWatcher.cs 

 

The USBWatcher.cs file contains a class named USBWatcher along with some supporting structures and methods for monitoring USB devices. 

Here's a breakdown of the file: 

Namespace and Using Directives: 

The file is part of the ZpenSample namespace. 

It includes several using directives to import namespaces necessary for the functionality implemented in the file. 

Structures: 

PnPEntityInfo: Defines a structure to hold information about a Plug and Play device, including its PNPDeviceID, name, description, service, status, vendor ID, product ID, and class GUID. 

USBControllerDevice: Defines a structure to represent a USB controller device, containing an antecedent and dependent field representing the USB controller device ID and USB Plug and Play device ID, respectively. 

Partial Class USB: 

This class contains methods for monitoring USB insertion and removal events and querying information about USB devices. 

Fields: 

insertWatcher: A ManagementEventWatcher object for monitoring USB insertion events. 

removeWatcher: A ManagementEventWatcher object for monitoring USB removal events. 

Methods: 

AddUSBEventWatcher: Adds event watchers for USB insertion and removal events. It takes event handler delegates for USB insertion and removal events and a TimeSpan parameter specifying the allowed lag time for sending notifications. 

RemoveUSBEventWatcher: Removes USB event watchers. 

WhoUSBControllerDevice: Retrieves information about the USB controller device involved in a USB insertion or removal event. 

AllUsbDevices, WhoUsbDevice, and other similar methods: Provide functionality to query information about USB devices based on various criteria such as vendor ID, product ID, device ID, and service. 

Region UsbDevice and PnPEntity: 

These regions contain methods related to querying USB devices and Plug and Play entities, respectively. 

Summary Comments: 

Each member (structures, methods) has XML documentation comments providing descriptions and usage information. 

Overall, this file encapsulates the functionality for monitoring USB events and retrieving information about USB devices and Plug and Play entities within the system. 

 

 

 

3: ZigPointF.cs 

The ZigPointF.cs file defines a class named ZigPointF, which appears to represent a point in a two-dimensional space with additional properties p, r, and t. However, some of the properties (r and t) are currently commented out in the code. 

Here's a breakdown of the class: 

Fields: 

private float x: Represents the x-coordinate of the point. 

private float y: Represents the y-coordinate of the point. 

private int p: Represents a property p of type int. 

Constructor: 

There's no explicit constructor defined in the class. 

Methods: 

SetX(float x): Sets the value of the x-coordinate. 

GetX(): Retrieves the value of the x-coordinate. 

SetY(float y): Sets the value of the y-coordinate. 

GetY(): Retrieves the value of the y-coordinate. 

SetP(int p): Sets the value of the property p. 

GetP(): Retrieves the value of the property p. 

Commented-out code: 

There are commented-out fields (r and t) and their corresponding setter and getter methods (SetR, GetR, SetT, GetT). 

It seems like the class is designed to represent a point with x and y coordinates, along with additional properties. However, the commented-out code suggests that there might be additional properties (r and t) that were considered but not implemented or used in the current version of the code. 

 

4: DataPassByStaticField.cs  

The DataPassByStaticField.cs file defines a class named DataPassByStaticField, which contains static fields used for passing data related to device types, Product IDs (PID), and Vendor IDs (VID). 

Here's a breakdown of the class: 

Static Fields: 

deviceTypeUSB: Represents the device type USB. 

deviceTypeHID: Represents the device type HID. 

pid: Represents the string "PID", typically used as a label for Product IDs. 

vid: Represents the string "VID", typically used as a label for Vendor IDs. 

pid_value1, pid_value2, pid_value3: Represent three different Product ID values. 

vid_value1, vid_value2: Represent two different Vendor ID values. 

These static fields are declared as public, meaning they can be accessed from outside the class without an instance of the class. 

The purpose of these fields seems to be providing convenient access to commonly used device-related values such as device types, PID, and VID. These values can be accessed statically without instantiating the DataPassByStaticField class. 

 
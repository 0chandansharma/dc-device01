using System;
using System.Management;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ZpenSample
{
    class USBWatcher
    {

    }

    /// <summary>
    /// Plug and Play device information structure
    /// </summary>
    public struct PnPEntityInfo
    {
        public String PNPDeviceID;      // Device ID
        public String Name;             // Device name
        public String Description;      // Device Description
        public String Service;          // Serve
        public String Status;           // device status
        public UInt16 VendorID;         // Supplier identification
        public UInt16 ProductID;        // Product number 
        public Guid ClassGuid;          // Device installation class GUID
    }

    /// <summary>
    /// USB control device type
    /// </summary>
    public struct USBControllerDevice
    {
        /// <summary>
        /// USB controller device ID
        /// </summary>
        public String Antecedent;

        /// <summary>
        /// USB Plug and Play Device ID
        /// </summary>
        public String Dependent;
    }

    /// <summary>
    /// Monitor USB plugging and unplugging
    /// </summary>
    public partial class USB
    {
        /// <summary>
        /// USB insertion event monitoring
        /// </summary>
        private ManagementEventWatcher insertWatcher = null;

        /// <summary>
        /// USB unplug event monitoring
        /// </summary>
        private ManagementEventWatcher removeWatcher = null;

        /// <summary>
        /// Add USB event monitor
        /// </summary>
        /// <param name="usbInsertHandler">USB insertion event handler</param>
        /// <param name="usbRemoveHandler">USB unplug event handler</param>
        /// <param name="withinInterval">Allowed lag time for sending notifications</param>
        public Boolean AddUSBEventWatcher(EventArrivedEventHandler usbInsertHandler, EventArrivedEventHandler usbRemoveHandler, TimeSpan withinInterval)
        {
            try
            {
                ManagementScope Scope = new ManagementScope("root\\CIMV2");
                Scope.Options.EnablePrivileges = true;

                // USB plug monitoring
                if (usbInsertHandler != null)
                {
                    WqlEventQuery InsertQuery = new WqlEventQuery("__InstanceCreationEvent", withinInterval, "TargetInstance isa 'Win32_USBControllerDevice'");

                    insertWatcher = new ManagementEventWatcher(Scope, InsertQuery);
                    insertWatcher.EventArrived += usbInsertHandler;
                    insertWatcher.Start();
                }

                // USB unplug monitoring
                if (usbRemoveHandler != null)
                {
                    WqlEventQuery RemoveQuery = new WqlEventQuery("__InstanceDeletionEvent", withinInterval, "TargetInstance isa 'Win32_USBControllerDevice'");

                    removeWatcher = new ManagementEventWatcher(Scope, RemoveQuery);
                    removeWatcher.EventArrived += usbRemoveHandler;
                    removeWatcher.Start();
                }

                return true;
            }

            catch (Exception)
            {
                RemoveUSBEventWatcher();
                return false;
            }
        }

        /// <summary>
        /// Remove USB event monitor
        /// </summary>
        public void RemoveUSBEventWatcher()
        {
            if (insertWatcher != null)
            {
                insertWatcher.Stop();
                insertWatcher = null;
            }

            if (removeWatcher != null)
            {
                removeWatcher.Stop();
                removeWatcher = null;
            }
        }

        /// <summary>
        /// Locate the USB device that is plugged or unplugged
        /// </summary>
        /// <param name="e">USB plug and unplug event parameters</param>
        /// <returns>USB control device ID where plugging and unplugging occurred</returns>
        public static USBControllerDevice[] WhoUSBControllerDevice(EventArrivedEventArgs e)
        {
            ManagementBaseObject mbo = e.NewEvent["TargetInstance"] as ManagementBaseObject;
            if (mbo != null && mbo.ClassPath.ClassName == "Win32_USBControllerDevice")
            {

                // Console.WriteLine(" ###### " + (mbo["Antecedent"] as String));
                // Console.WriteLine(" ###### " + (mbo["Dependent"] as String));
                String Antecedent = (mbo["Antecedent"] as String).Replace("\"", String.Empty).Split(new Char[] { '=' })[1];
                String Dependent = (mbo["Dependent"] as String).Replace("\"", String.Empty).Split(new Char[] { '=' })[1];
                return new USBControllerDevice[1] { new USBControllerDevice { Antecedent = Antecedent, Dependent = Dependent } };
            }
            return null;
        }
        #region UsbDevice
        /// <summary>
        /// Get all USB device entities (filter devices without VID and PID)
        /// </summary>
        public static PnPEntityInfo[] AllUsbDevices
        {
            get
            {
                return WhoUsbDevice(UInt16.MinValue, UInt16.MinValue, Guid.Empty);
            }
        }

        /// <summary>
        /// Query the USB device entity (the device requires VID and PID)
        /// </summary>
        /// <param name="VendorID">Vendor ID, ignored by MinValue</param>
        /// <param name="ProductID">Product number, ignored by MinValue</param>
        /// <param name="ClassGuid">Device installation class Guid, ignored by Empty</param>
        /// <returns>Device List</returns>
        public static PnPEntityInfo[] WhoUsbDevice(UInt16 VendorID, UInt16 ProductID, Guid ClassGuid)
        {
            List<PnPEntityInfo> UsbDevices = new List<PnPEntityInfo>();

            // Get the USB controller and its associated device entity
            ManagementObjectCollection USBControllerDeviceCollection = new ManagementObjectSearcher("SELECT * FROM Win32_USBControllerDevice").Get();
            if (USBControllerDeviceCollection != null)
            {
                foreach (ManagementObject USBControllerDevice in USBControllerDeviceCollection)
                {   // Get the DeviceID of the device entity
                    String Dependent = (USBControllerDevice["Dependent"] as String).Split(new Char[] { '=' })[1];

                    // Filter out USB devices without VID and PID
                    Match match = Regex.Match(Dependent, "VID_[0-9|A-F]{4}&PID_[0-9|A-F]{4}");
                    if (match.Success)
                    {
                        UInt16 theVendorID = Convert.ToUInt16(match.Value.Substring(4, 4), 16);   // Supplier identification
                        if (VendorID != UInt16.MinValue && VendorID != theVendorID) continue;

                        UInt16 theProductID = Convert.ToUInt16(match.Value.Substring(13, 4), 16); // Product number
                        if (ProductID != UInt16.MinValue && ProductID != theProductID) continue;

                        ManagementObjectCollection PnPEntityCollection = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE DeviceID=" + Dependent).Get();
                        if (PnPEntityCollection != null)
                        {
                            foreach (ManagementObject Entity in PnPEntityCollection)
                            {
                                Guid theClassGuid = new Guid(Entity["ClassGuid"] as String);    // Device installation class GUID
                                if (ClassGuid != Guid.Empty && ClassGuid != theClassGuid) continue;

                                PnPEntityInfo Element;
                                Element.PNPDeviceID = Entity["PNPDeviceID"] as String;  // Device ID
                                Element.Name = Entity["Name"] as String;                // Device name
                                Element.Description = Entity["Description"] as String;  // Device Description
                                Element.Service = Entity["Service"] as String;          // Serve
                                Element.Status = Entity["Status"] as String;            // device status
                                Element.VendorID = theVendorID;     // Supplier identification
                                Element.ProductID = theProductID;   // Product number
                                Element.ClassGuid = theClassGuid;   // Device installation class GUID

                                UsbDevices.Add(Element);
                            }
                        }
                    }
                }
            }

            if (UsbDevices.Count == 0) return null; else return UsbDevices.ToArray();
        }

        /// <summary>
        /// Query the USB device entity (the device requires VID and PID)
        /// </summary>
        /// <param name="VendorID">Supplier identification，MinValueignore</param>
        /// <param name="ProductID">Product number，MinValueignore</param>
        /// <returns>Device List</returns>
        public static PnPEntityInfo[] WhoUsbDevice(UInt16 VendorID, UInt16 ProductID)
        {
            return WhoUsbDevice(VendorID, ProductID, Guid.Empty);
        }

        /// <summary>
        /// Query the USB device entity (the device requires VID and PID)
        /// </summary>
        /// <param name="ClassGuid">Device installation class GUID，Empty ignore</param>
        /// <returns>Device List</returns>
        public static PnPEntityInfo[] WhoUsbDevice(Guid ClassGuid)
        {
            return WhoUsbDevice(UInt16.MinValue, UInt16.MinValue, ClassGuid);
        }

        /// <summary>
        /// Query the USB device entity (the device requires VID and PID)
        /// </summary>
        /// <param name="PNPDeviceID">Device ID, which can be incomplete information</param>
        /// <returns>Device List</returns>        
        public static PnPEntityInfo[] WhoUsbDevice(String PNPDeviceID)
        {
            List<PnPEntityInfo> UsbDevices = new List<PnPEntityInfo>();

            // Get the USB controller and its associated device entity
            ManagementObjectCollection USBControllerDeviceCollection = new ManagementObjectSearcher("SELECT * FROM Win32_USBControllerDevice").Get();
            if (USBControllerDeviceCollection != null)
            {
                foreach (ManagementObject USBControllerDevice in USBControllerDeviceCollection)
                {   // Get the DeviceID of the device entity
                    String Dependent = (USBControllerDevice["Dependent"] as String).Split(new Char[] { '=' })[1];
                    if (!String.IsNullOrEmpty(PNPDeviceID))
                    {   // NOTE: Ignore case
                        if (Dependent.IndexOf(PNPDeviceID, 1, PNPDeviceID.Length - 2, StringComparison.OrdinalIgnoreCase) == -1) continue;
                    }

                    // Filter out USB devices without VID and PID
                    Match match = Regex.Match(Dependent, "VID_[0-9|A-F]{4}&PID_[0-9|A-F]{4}");
                    if (match.Success)
                    {
                        ManagementObjectCollection PnPEntityCollection = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE DeviceID=" + Dependent).Get();
                        if (PnPEntityCollection != null)
                        {
                            foreach (ManagementObject Entity in PnPEntityCollection)
                            {
                                PnPEntityInfo Element;
                                Element.PNPDeviceID = Entity["PNPDeviceID"] as String;  // Device ID
                                Element.Name = Entity["Name"] as String;                // Device name
                                Element.Description = Entity["Description"] as String;  // Device Description
                                Element.Service = Entity["Service"] as String;          // Serve
                                Element.Status = Entity["Status"] as String;            // device status
                                Element.VendorID = Convert.ToUInt16(match.Value.Substring(4, 4), 16);   // Supplier identification   
                                Element.ProductID = Convert.ToUInt16(match.Value.Substring(13, 4), 16); // Product number                         // Product number
                                Element.ClassGuid = new Guid(Entity["ClassGuid"] as String);            // Device installation class GUID

                                UsbDevices.Add(Element);
                            }
                        }
                    }
                }
            }

            if (UsbDevices.Count == 0) return null; else return UsbDevices.ToArray();
        }

        /// <summary>
        /// Locate USB devices based on Serve
        /// </summary>
        /// <param name="ServiceCollection">Serve collection to be queried</param>
        /// <returns>Device List</returns>
        public static PnPEntityInfo[] WhoUsbDevice(String[] ServiceCollection)
        {
            if (ServiceCollection == null || ServiceCollection.Length == 0)
                return WhoUsbDevice(UInt16.MinValue, UInt16.MinValue, Guid.Empty);

            List<PnPEntityInfo> UsbDevices = new List<PnPEntityInfo>();

            // Get the USB controller and its associated device entity
            ManagementObjectCollection USBControllerDeviceCollection = new ManagementObjectSearcher("SELECT * FROM Win32_USBControllerDevice").Get();
            if (USBControllerDeviceCollection != null)
            {
                foreach (ManagementObject USBControllerDevice in USBControllerDeviceCollection)
                {   // Get the DeviceID of the device entity
                    String Dependent = (USBControllerDevice["Dependent"] as String).Split(new Char[] { '=' })[1];

                    // Filter out USB devices without VID and PID
                    Match match = Regex.Match(Dependent, "VID_[0-9|A-F]{4}&PID_[0-9|A-F]{4}");
                    if (match.Success)
                    {
                        ManagementObjectCollection PnPEntityCollection = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE DeviceID=" + Dependent).Get();
                        if (PnPEntityCollection != null)
                        {
                            foreach (ManagementObject Entity in PnPEntityCollection)
                            {
                                String theService = Entity["Service"] as String;          // Serve
                                if (String.IsNullOrEmpty(theService)) continue;

                                foreach (String Service in ServiceCollection)
                                {   // NOTE: Ignore case
                                    if (String.Compare(theService, Service, true) != 0) continue;

                                    PnPEntityInfo Element;
                                    Element.PNPDeviceID = Entity["PNPDeviceID"] as String;  // Device ID
                                    Element.Name = Entity["Name"] as String;                // Device name
                                    Element.Description = Entity["Description"] as String;  // Device Description
                                    Element.Service = theService;                           // Serve
                                    Element.Status = Entity["Status"] as String;            // device status
                                    Element.VendorID = Convert.ToUInt16(match.Value.Substring(4, 4), 16);   // Supplier identification   
                                    Element.ProductID = Convert.ToUInt16(match.Value.Substring(13, 4), 16); // Product number
                                    Element.ClassGuid = new Guid(Entity["ClassGuid"] as String);            // Device installation class GUID

                                    UsbDevices.Add(Element);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            if (UsbDevices.Count == 0) return null; else return UsbDevices.ToArray();
        }
        #endregion

        #region PnPEntity
        /// <summary>
        /// All Plug and Play device entities (filters devices without VID and PID)
        /// </summary>
        public static PnPEntityInfo[] AllPnPEntities
        {
            get
            {
                return WhoPnPEntity(UInt16.MinValue, UInt16.MinValue, Guid.Empty);
            }
        }

        /// <summary>
        /// Locate plug-and-play device entities based on VID, PID and Device installation class GUID
        /// </summary>
        /// <param name="VendorID">Supplier identification，MinValueignore</param>
        /// <param name="ProductID">Product number，MinValueignore</param>
        /// <param name="ClassGuid">Device installation class GUID，Empty ignore</param>
        /// <returns>Device List</returns>
        /// <remarks>
        /// HID：{745a17a0-74d3-11d0-b6fe-00a0c90f57da}
        /// Imaging Device：{6bdd1fc6-810f-11d0-bec7-08002be2092f}
        /// Keyboard：{4d36e96b-e325-11ce-bfc1-08002be10318} 
        /// Mouse：{4d36e96f-e325-11ce-bfc1-08002be10318}
        /// Network Adapter：{4d36e972-e325-11ce-bfc1-08002be10318}
        /// USB：{36fc9e60-c465-11cf-8056-444553540000}
        /// </remarks>
        public static PnPEntityInfo[] WhoPnPEntity(UInt16 VendorID, UInt16 ProductID, Guid ClassGuid)
        {
            List<PnPEntityInfo> PnPEntities = new List<PnPEntityInfo>();

            // Enumerate Plug and Play device entities
            String VIDPID;
            if (VendorID == UInt16.MinValue)
            {
                if (ProductID == UInt16.MinValue)
                    VIDPID = "'%VID[_]____&PID[_]____%'";
                else
                    VIDPID = "'%VID[_]____&PID[_]" + ProductID.ToString("X4") + "%'";
            }
            else
            {
                if (ProductID == UInt16.MinValue)
                    VIDPID = "'%VID[_]" + VendorID.ToString("X4") + "&PID[_]____%'";
                else
                    VIDPID = "'%VID[_]" + VendorID.ToString("X4") + "&PID[_]" + ProductID.ToString("X4") + "%'";
            }

            String QueryString;
            if (ClassGuid == Guid.Empty)
                QueryString = "SELECT * FROM Win32_PnPEntity WHERE PNPDeviceID LIKE" + VIDPID;
            else
                QueryString = "SELECT * FROM Win32_PnPEntity WHERE PNPDeviceID LIKE" + VIDPID + " AND ClassGuid='" + ClassGuid.ToString("B") + "'";

            ManagementObjectCollection PnPEntityCollection = new ManagementObjectSearcher(QueryString).Get();
            if (PnPEntityCollection != null)
            {
                foreach (ManagementObject Entity in PnPEntityCollection)
                {
                    String PNPDeviceID = Entity["PNPDeviceID"] as String;
                    Match match = Regex.Match(PNPDeviceID, "VID_[0-9|A-F]{4}&PID_[0-9|A-F]{4}");
                    if (match.Success)
                    {
                        PnPEntityInfo Element;

                        Element.PNPDeviceID = PNPDeviceID;                                      // Device ID
                        Element.Name = Entity["Name"] as String;                                // Device name
                        Element.Description = Entity["Description"] as String;                  // Device Description
                        Element.Service = Entity["Service"] as String;                          // Serve
                        Element.Status = Entity["Status"] as String;                            // device status
                        Element.VendorID = Convert.ToUInt16(match.Value.Substring(4, 4), 16);   // Supplier identification
                        Element.ProductID = Convert.ToUInt16(match.Value.Substring(13, 4), 16); // Product number
                        Element.ClassGuid = new Guid(Entity["ClassGuid"] as String);            // Device installation class GUID

                        PnPEntities.Add(Element);
                    }
                }
            }

            if (PnPEntities.Count == 0) return null; else return PnPEntities.ToArray();
        }

        /// <summary>
        /// Locate plug-and-play device entities based on VID and PID
        /// </summary>
        /// <param name="VendorID">Supplier identification，MinValueignore</param>
        /// <param name="ProductID">Product number，MinValueignore</param>
        /// <returns>Device List</returns>
        public static PnPEntityInfo[] WhoPnPEntity(UInt16 VendorID, UInt16 ProductID)
        {
            return WhoPnPEntity(VendorID, ProductID, Guid.Empty);
        }

        /// <summary>
        /// Locate plug-and-play device entities based on Device installation class GUID
        /// </summary>
        /// <param name="ClassGuid">Device installation class GUID，Empty ignore</param>
        /// <returns>Device List</returns>
        public static PnPEntityInfo[] WhoPnPEntity(Guid ClassGuid)
        {
            return WhoPnPEntity(UInt16.MinValue, UInt16.MinValue, ClassGuid);
        }

        /// <summary>
        /// Locate devices based on Device ID
        /// </summary>
        /// <param name="PNPDeviceID">Device ID, which can be incomplete information</param>
        /// <returns>Device List</returns>
        /// <remarks>
        /// Note: For underscores, you need to write "[_]", otherwise it will be regarded as any character
        /// </remarks>
        public static PnPEntityInfo[] WhoPnPEntity(String PNPDeviceID)
        {
            List<PnPEntityInfo> PnPEntities = new List<PnPEntityInfo>();

            // Enumerate Plug and Play device entities
            String QueryString;
            if (String.IsNullOrEmpty(PNPDeviceID))
            {
                QueryString = "SELECT * FROM Win32_PnPEntity WHERE PNPDeviceID LIKE '%VID[_]____&PID[_]____%'";
            }
            else
            {   // Backslash characters in the LIKE clause will cause WQL query exceptions
                QueryString = "SELECT * FROM Win32_PnPEntity WHERE PNPDeviceID LIKE '%" + PNPDeviceID.Replace('\\', '_') + "%'";
            }

            ManagementObjectCollection PnPEntityCollection = new ManagementObjectSearcher(QueryString).Get();
            if (PnPEntityCollection != null)
            {
                foreach (ManagementObject Entity in PnPEntityCollection)
                {
                    String thePNPDeviceID = Entity["PNPDeviceID"] as String;
                    Match match = Regex.Match(thePNPDeviceID, "VID_[0-9|A-F]{4}&PID_[0-9|A-F]{4}");
                    if (match.Success)
                    {
                        PnPEntityInfo Element;

                        Element.PNPDeviceID = thePNPDeviceID;                   // Device ID
                        Element.Name = Entity["Name"] as String;                // Device name
                        Element.Description = Entity["Description"] as String;  // Device Description
                        Element.Service = Entity["Service"] as String;          // Serve
                        Element.Status = Entity["Status"] as String;            // device status
                        Element.VendorID = Convert.ToUInt16(match.Value.Substring(4, 4), 16);   // Supplier identification
                        Element.ProductID = Convert.ToUInt16(match.Value.Substring(13, 4), 16); // Product number
                        Element.ClassGuid = new Guid(Entity["ClassGuid"] as String);            // Device installation class GUID

                        PnPEntities.Add(Element);
                    }
                }
            }

            if (PnPEntities.Count == 0) return null; else return PnPEntities.ToArray();
        }

        /// <summary>
        /// Locate devices based on Serve
        /// </summary>
        /// <param name="ServiceCollection">Serve collection to be queried，nullignore</param>
        /// <returns>Device List</returns>
        /// <remarks>
        /// Classes related to Serve：
        ///     Win32_SystemDriverPNPEntity
        ///     Win32_SystemDriver
        /// </remarks>
        public static PnPEntityInfo[] WhoPnPEntity(String[] ServiceCollection)
        {
            if (ServiceCollection == null || ServiceCollection.Length == 0)
                return WhoPnPEntity(UInt16.MinValue, UInt16.MinValue, Guid.Empty);

            List<PnPEntityInfo> PnPEntities = new List<PnPEntityInfo>();

            // Enumerate Plug and Play device entities
            String QueryString = "SELECT * FROM Win32_PnPEntity WHERE PNPDeviceID LIKE '%VID[_]____&PID[_]____%'";
            ManagementObjectCollection PnPEntityCollection = new ManagementObjectSearcher(QueryString).Get();
            if (PnPEntityCollection != null)
            {
                foreach (ManagementObject Entity in PnPEntityCollection)
                {
                    String PNPDeviceID = Entity["PNPDeviceID"] as String;
                    Match match = Regex.Match(PNPDeviceID, "VID_[0-9|A-F]{4}&PID_[0-9|A-F]{4}");
                    if (match.Success)
                    {
                        String theService = Entity["Service"] as String;            // Serve
                        if (String.IsNullOrEmpty(theService)) continue;

                        foreach (String Service in ServiceCollection)
                        {   // NOTE: Ignore case
                            if (String.Compare(theService, Service, true) != 0) continue;

                            PnPEntityInfo Element;

                            Element.PNPDeviceID = PNPDeviceID;                      // Device ID
                            Element.Name = Entity["Name"] as String;                // Device name
                            Element.Description = Entity["Description"] as String;  // Device Description
                            Element.Service = theService;                           // Serve
                            Element.Status = Entity["Status"] as String;            // device status
                            Element.VendorID = Convert.ToUInt16(match.Value.Substring(4, 4), 16);   // Supplier identification
                            Element.ProductID = Convert.ToUInt16(match.Value.Substring(13, 4), 16); // Product number
                            Element.ClassGuid = new Guid(Entity["ClassGuid"] as String);            // Device installation class GUID

                            PnPEntities.Add(Element);
                            break;
                        }
                    }
                }
            }

            if (PnPEntities.Count == 0) return null; else return PnPEntities.ToArray();
        }
        #endregion        
    }
}

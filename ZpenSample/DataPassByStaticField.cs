// The DataPassByStaticField.cs file defines a class named DataPassByStaticField, which contains static fields used for passing data related to device types, Product IDs (PID), and Vendor IDs (VID).
// The purpose of these fields seems to be providing convenient access to commonly used device-related values such as device types, PID, and VID. These values can be accessed statically without instantiating the DataPassByStaticField class.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZpenSample
{
    class DataPassByStaticField
    {
        public static string deviceTypeUSB = "USB";  //Represents the device type USB.
        public static string deviceTypeHID = "HID"; //Represents the device type HID.

        public static string pid = "PID"; //Represents the string "PID", typically used as a label for Product IDs.
        public static string vid = "VID"; //Represents the string "VID", typically used as a label for Vendor IDs.

        // pid_value1, pid_value2, pid_value3: Represent three different Product ID values.
        public static UInt16 pid_value1 = 0x0101;
        public static UInt16 pid_value2 = 0x5756;
        public static UInt16 pid_value3 = 0x5758;

        // vid_value1, vid_value2: Represent two different Vendor ID values.
        public static UInt16 vid_value1 = 0x1A40;
        public static UInt16 vid_value2 = 0x0483;

        // These static fields are declared as public, meaning they can be accessed from outside the class without an instance of the class.
    }
}

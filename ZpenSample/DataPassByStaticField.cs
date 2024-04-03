using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZpenSample
{
    class DataPassByStaticField
    {
        public static string deviceTypeUSB = "USB";
        public static string deviceTypeHID = "HID";

        public static string pid = "PID";
        public static string vid = "VID";

        public static UInt16 pid_value1 = 0x0101;
        public static UInt16 pid_value2 = 0x5756;
        public static UInt16 pid_value3 = 0x5758;

        public static UInt16 vid_value1 = 0x1A40;
        public static UInt16 vid_value2 = 0x0483;
    }
}

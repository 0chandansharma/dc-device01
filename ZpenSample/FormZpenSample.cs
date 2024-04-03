// These variables and constants are used throughout the class to store data, manage device connections, process pen data, and perform various operations related to drawing and device management. They provide a structured way to manage and manipulate data within the class.
// The flow of the code involves initializing the form, setting up device connections, handling data reception, processing and drawing pen data, managing device connections, and responding to USB events. The code is structured to handle these tasks efficiently and interact with the UI elements appropriately.
using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;   //These directives bring in various namespaces required for the code to function properly. For example, System.Windows.Forms is required for working with Windows Forms, 
using System.Runtime.InteropServices; // System.Runtime.InteropServices is required for working with unmanaged code.
using System.Threading;
using System.IO;
using System.Collections;
using System.Drawing.Drawing2D;
using System.Management;
namespace ZpenSample   //The code is within the ZpenSample namespace.
{
    public partial class FormZpenSample : Form  //The FormZpenSample class is declared, which inherits from Form. This class represents the main form of the application.
    {
        [DllImport(@"zpen.dll")] //This attribute is used to indicate that the methods are implemented in unmanaged code (in zpen.dll). It allows managed code to call unmanaged functions.
        public static extern int OpenDevices();

        [DllImport(@"zpen.dll")]
        public static extern int InitDevices();

        [DllImport(@"zpen.dll")]
        public static extern int CloseDevices();

        [DllImport(@"zpen.dll", EntryPoint = "SetClientArray", CallingConvention = CallingConvention.Winapi)]
        public static extern void SetClientArray(byte[] defaultIDArray);

        [DllImport(@"zpen.dll", EntryPoint = "GetHIDData", CallingConvention = CallingConvention.Winapi)]
        public static extern byte GetHIDData(byte[] hidDataArray);

        [DllImport(@"zpen.dll", EntryPoint = "Send2HIDData", CallingConvention = CallingConvention.Winapi)]
        public static extern byte Send2HIDData(byte[] hidDataArray);

        [DllImport(@"zpen.dll", EntryPoint = "RequestOfflineDataInfo", CallingConvention = CallingConvention.Winapi)]
        public static extern byte RequestOfflineDataInfo();

        [DllImport(@"zpen.dll", EntryPoint = "RequestOfflineData", CallingConvention = CallingConvention.Winapi)]
        public static extern byte RequestOfflineData(byte[] name);

        [DllImport(@"zpen.dll", EntryPoint = "DeleteOfflineDataPage", CallingConvention = CallingConvention.Winapi)]
        public static extern byte DeleteOfflineDataPage(byte[] name);

        private static int CAPACITY_SIZE = 60;  //This constant defines the capacity size for arrays used in the class. It appears to be set to 60, indicating that arrays such as pictureBoxes, labels, bitmaps, etc., will have a fixed size of 60 elements.

        private static PictureBox[] pictureBoxes = new PictureBox[CAPACITY_SIZE]; //This is an array of PictureBox objects with a size defined by CAPACITY_SIZE. These PictureBox objects are used to display pen data on the form.
        private static Label[] labels = new Label[CAPACITY_SIZE]; // Similar to pictureBoxes, this is an array of Label objects used to display device MAC addresses on the form.
        private static Bitmap[] bitmaps = new Bitmap[CAPACITY_SIZE]; //This array stores Bitmap objects used for drawing pen data received from the devices.
        private static Graphics[] graphics = new Graphics[CAPACITY_SIZE]; //This array stores Graphics objects used for drawing on the Bitmap objects.
        private static PointF[] startPoint = new PointF[CAPACITY_SIZE]; //These arrays store PointF objects representing the start and end points of pen strokes drawn on the PictureBox.
        private static PointF[] endPoint = new PointF[CAPACITY_SIZE];
        private static bool[] firstPoints = new bool[CAPACITY_SIZE]; //This array stores boolean values indicating whether the current point being processed is the first point of a stroke.

        /// <summary>
        /// Hashtable storage collection 60 个 pictureBox
        /// </summary>
        private static Hashtable hashtablePictureBox = new Hashtable();   //Hashtable: Two Hashtable objects (hashtablePictureBox and hashtablePen) are used to store references to PictureBox and Pen objects, respectively.
        //These Hashtable objects store references to PictureBox and Pen objects, respectively, indexed by device MAC addresses.
        /// <summary>
        /// Hashtable storage collection 60 个 pen
        /// </summary>
        private static Hashtable hashtablePen = new Hashtable();

        /// <summary>
        /// base path for absolute path
        /// </summary>
        private static string basePath = AppDomain.CurrentDomain.BaseDirectory; //This variable stores the base directory path of the application.
        /// <summary>
        /// The path to the device information list file
        /// </summary>
        private static string addressListPath = "client-mac-list.txt"; //This variable stores the relative path to the device information list file (client-mac-list.txt).

        private delegate void PenDataToPictureBoxDelegate(int serialNumber, PictureBox pictureBox, Pen pen, string mac, byte[] buffer55);
        /// <summary>
        /// Device connection flag
        /// </summary>
        private static int CLIENT_CONNECT_TAG = 0x12;
        //CLIENT_CONNECT_TAG, CLIENT_CONNECT_OK_TAG, CLIENT_CONNECT_ERROR_TAG: These constants represent flags for device connection status.
        /// <summary>
        /// Device connection success flag
        /// </summary>
        private static int CLIENT_CONNECT_OK_TAG = 0x01;
        /// <summary>
        /// Device connection failure flag
        /// </summary>
        private static int CLIENT_CONNECT_ERROR_TAG = 0x00;
        /// <summary>
        /// Offline handwritten notes 
        /// </summary>
        private static int OFFLINE_PENDATA_TAG = 0x42;//Query notes,  OFFLINE_PENDATA_TAG, OFFLINE_PENDATA_DEL_TAG: These constants represent flags for offline pen data operations.
        private static int OFFLINE_PENDATA_DEL_TAG = 0x33;//Delete note
        private static int HID_CMD_SIZE = 32; // 
        private static byte[] offline_pen_filename = new byte[HID_CMD_SIZE]; //offline_pen_filename and hid_cmd_data: These arrays store HID data used for offline pen data operations.
        private static byte[] hid_cmd_data = new byte[HID_CMD_SIZE]; //HID_CMD_SIZE: This constant represents the size of HID command data.
        /// <summary>
        /// Set pen size
        /// </summary>
        private static float penSize = 2.0f;  //This variable stores the size of the pen used for drawing.

        private static float ZigPressureValue256Level = 0.00390625f;  //This variable stores a constant representing the pressure sensitivity value. It appears to be set to 0.00390625f.
        private byte[] arrayMacList;
        /// <summary>
        /// Flags for handwritten data 
        /// </summary>
        private static int PENDATA_TAG1 = 0x30; //PENDATA_TAG1, PENDATA_TAG2, PENDATA_TAG3: These constants represent flags for different types of pen data.
        private static int PENDATA_TAG2 = 0x31;
        private static int PENDATA_TAG3 = 0x32; //Supplementary data

        private USB ezUSB = new USB();

        public FormZpenSample()
        {
            InitializeComponent();  //The constructor initializes the form and calls InitViews() and StartGetData() methods.
            InitViews();  //
            StartGetData();
            InitDongle();
        }
        private void InitDongle()  // Initializes the Bluetooth AP connection status.
        {
            bool isInsert = CheckDongleIsInsert();
            if (isInsert)
            {
                this.labelConnectStatus.Text = "Bluetooth AP is connected";
                this.labelConnectStatus.ForeColor = Color.Green;
            }
            else
            {
                this.labelConnectStatus.Text = "Bluetooth AP has been disconnected  ";
                this.labelConnectStatus.ForeColor = Color.Red;
            }
        }

        private void StartGetData() //Opens and initializes devices and starts a thread for receiving HID data.
        {
            int openRet = OpenDevices();
            int initRet = InitDevices();
            SetClientArray(arrayMacList);
            Thread receiveHidDataFromDongleThread = new Thread(ReceiveHidDataFromDongle);
            receiveHidDataFromDongleThread.IsBackground = true;
            receiveHidDataFromDongleThread.Name = "receiveHidDataFromDongleThread";
            receiveHidDataFromDongleThread.Start();
        }
        private void ReceiveHidDataFromDongle() //Thread function for continuously receiving HID data.
        {
            while (true) // true
            {
                byte[] buffer = new byte[64];
                byte getDataResult = GetHIDData(buffer);

                if (getDataResult == 1)
                {
                    SetTextSafePost(buffer);
                }
                Thread.Sleep(2);
            }
        }
  
        public void SetTextSafePost(object buffer) //Processes received HID data and updates UI elements accordingly.
        {
            byte[] newBuffer = (byte[])(buffer);
            if (newBuffer[2] == PENDATA_TAG1 || newBuffer[2] == PENDATA_TAG2) //  handwritten data
            {
                byte[] buffer55 = newBuffer.Skip(9).Take(55).ToArray();
                string penDataFileName = newBuffer[3].ToString("X2") + "-" + newBuffer[4].ToString("X2") + "-" + newBuffer[5].ToString("X2") + "-" + newBuffer[6].ToString("X2");
                Pen pn = (Pen)hashtablePen[penDataFileName];
                PictureBox pb = (PictureBox)hashtablePictureBox[penDataFileName];
                int serialNumber = Convert.ToInt32(pb.Name.Split('x')[1]);
                DrawInBitmap(serialNumber - 1, pb, pn, penDataFileName, buffer55);
            }
            else if (newBuffer[2] == CLIENT_CONNECT_TAG) //  Connection Status
            {
                string deviceMac = newBuffer[4].ToString("X2") + "-" + newBuffer[5].ToString("X2") + "-" + newBuffer[6].ToString("X2") + "-" + newBuffer[7].ToString("X2");
                Console.WriteLine(deviceMac);
                PictureBox pb = (PictureBox)hashtablePictureBox[deviceMac];
                int serialNumber = Convert.ToInt32(pb.Name.Split('x')[1]);

                if (newBuffer[8] == CLIENT_CONNECT_OK_TAG) // connected
                {
                    labels[serialNumber-1].ForeColor = Color.Blue;
                }
                else if (newBuffer[8] == CLIENT_CONNECT_ERROR_TAG) // disconnect
                {
                    labels[serialNumber - 1].ForeColor = Color.Red;
                }
            }
            else if (newBuffer[2] == OFFLINE_PENDATA_TAG) //  Query notes
            {

            }
        }
        
        /// <summary>
        /// Analytical data
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="y1"></param>
        /// <param name="y2"></param>
        /// <param name="pressureValue">Pressure sensitivity value</param>
        /// <returns></returns>
        private ZigPointF DecodeData(byte x1, byte x2, byte y1, byte y2, byte pressureValue) //Decodes raw data into ZigPointF objects.
        {
            ZigPointF zigPointF = new ZigPointF();
            float x = BitConverter.ToInt16(new byte[2] { x1, x2 }, 0);
            float y = BitConverter.ToInt16(new byte[2] { y1, y2 }, 0);
            int p  = (int)pressureValue;
            zigPointF.SetX(x);
            zigPointF.SetY(y);
            zigPointF.SetP(p);
            return zigPointF;
        }

        /// <summary>
        /// Receive the corresponding handwriting data and draw on the corresponding pictureBox.
        /// </summary>
        /// <param name="serialNumber">serial number</param>
        /// <param name="pictureBox">canvas object</param>
        /// <param name="pen">brush object</param>
        /// <param name="mac">MAC address</param>
        /// <param name="buffer55">handwriting data</param>
        private void DrawInBitmap(int serialNumber, PictureBox pictureBox, Pen pen, string mac, byte[] buffer55) //Draws received pen data onto the corresponding PictureBox.
        {
            // InvokeRequired == true  It is to determine whether the current thread is a UI thread. If true, it is not a UI thread, so a delegate is used.
            // If false then no delegate is used.
            if (pictureBox.InvokeRequired == false)
            {
                // Console.WriteLine(" pictureBox.Name = " + pictureBox.Name);
                graphics[serialNumber].SmoothingMode = SmoothingMode.HighQuality;
                for (int i = 0; i < (buffer55.Length / 5); i++)
                {
                    ZigPointF zigPointF = DecodeData(buffer55[5 * i + 0],buffer55[5 * i + 1],buffer55[5 * i + 2],buffer55[5 * i + 3],buffer55[5 * i + 4]);
                    Console.WriteLine(zigPointF.GetX()+","+zigPointF.GetY()+","+zigPointF.GetP());
                    pen.Width = zigPointF.GetP() * ZigPressureValue256Level * penSize;
                    pen.Color = Color.Black;
                    // As long as the handwriting has been drawn with S Pen, it should be saved.
                    // Should add a save result, whether it is really saved
                    // Note: Saving note data will cause lag on a computer with only 4G of memory, and the file read and write stream consumes a lot of money.
                    // It is recommended that later users only save handwriting data when they pick up the pen (when idle).
                    // SavePenDataToFile(mac, sxyp);
                    // You should judge whether sxyp[0] and sxyp[1] are 0 before scaling, because scaling of 0 is meaningless.
                    // endPoint[serialNumber].X = (float)(Y_MAXIMUM - zigPointF.GetY()) / (float)50;
                    endPoint[serialNumber].X = (float)(21000 - zigPointF.GetY()) / (float)66.455697;//66.45569620253165;
                    endPoint[serialNumber].Y = (float)(zigPointF.GetX()) / (float)62.394958;//62.39495798319328;
                    if (firstPoints[serialNumber] == true)// 第 1 个点
                    {
                        if (zigPointF.GetP() != 0)
                        {
                            // Record the (X,Y) coordinates of the first point and change the flag to false
                            startPoint[serialNumber] = endPoint[serialNumber];
                            firstPoints[serialNumber] = false;
                        }
                    }
                    else//Nth point (N >=2)
                    {
                        if (zigPointF.GetP() == 0) // Pressure value = 0, stop book
                        {
                            firstPoints[serialNumber] = true;
                        }
                        else
                        {
                            graphics[serialNumber].DrawLine(pen, startPoint[serialNumber], endPoint[serialNumber]);
                            startPoint[serialNumber] = endPoint[serialNumber];
                        }
                    }
                }
                graphics[serialNumber].Save();
                graphics[serialNumber].Flush();
                pictureBox.Image = bitmaps[serialNumber];
                pictureBox.Invalidate();
            }
            else
            {
                PenDataToPictureBoxDelegate penDataToPictureBoxDelegate = new PenDataToPictureBoxDelegate(DrawInBitmap);
                pictureBox.Invoke(penDataToPictureBoxDelegate, serialNumber, pictureBox, pen, mac, buffer55);
            }
        }

        /// <summary>
        /// Get all mac addresses from configuration file
        /// </summary>
        /// <returns>mac array</returns>
        private string[] GetMacAddress() //Reads MAC addresses from a configuration file.
        {
            string path = basePath + addressListPath;
            byte[,] defaultClientArrayFromFile = GetClientListFromFile(path);
            byte[] array = TwoDimensionToOnedDimension(defaultClientArrayFromFile);
            string[] macArray = new string[array.Length / 4];
            for (int j = 0; j < array.Length / 4; j++)
            {
                macArray[j] = array[j * 4 + 0].ToString("X2") + "-" + array[j * 4 + 1].ToString("X2") + "-" + array[j * 4 + 2].ToString("X2") + "-" + array[j * 4 + 3].ToString("X2");
            }
            arrayMacList = array;

            return macArray;
        }

        /// <summary>
        /// Read the id list according to the client-mac-list.txt configuration file
        /// </summary>
        /// <param name="path">Path to client-mac-list.txt </param>
        /// <returns></returns>
        public byte[,] GetClientListFromFile(string path) //Reads ID list from a configuration file.
        {
            byte[,] defaultIDArray = new byte[60, 4];
            StreamReader sr = new StreamReader(path, Encoding.ASCII);
            try
            {
                String line;
                int num = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    string newString = line.Replace("{", "").Replace("}", "").Replace(",", " ");
                    String[] numbers = newString.Split(' ');
                    defaultIDArray[num, 0] = Convert.ToByte(numbers[0], 16);
                    defaultIDArray[num, 1] = Convert.ToByte(numbers[1], 16);
                    defaultIDArray[num, 2] = Convert.ToByte(numbers[2], 16);
                    defaultIDArray[num, 3] = Convert.ToByte(numbers[3], 16);
                    num++;
                }
                Console.WriteLine("clients sum=" + num);

                return defaultIDArray;
            }
            catch (Exception e) // System.IO.FileNotFoundException
            {
                Console.WriteLine(e);
                return null;
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
            }
        }

        /// <summary>
        /// Convert 2D array to 1D array
        /// </summary>
        /// <param name="defaultClientArray">Two-dimensional array</param>
        /// <returns>one-dimensional array</returns>
        public byte[] TwoDimensionToOnedDimension(byte[,] defaultClientArray) //Converts a 2D array to a 1D array.
        {
            byte[] tempArray = new byte[60 * 4];
            for (int i = 0; i < defaultClientArray.Length; i++)
            {
                tempArray[i] = defaultClientArray[i / 4, i % 4];
                //Console.WriteLine("now [" + i + "]= " + tempArray[i].ToString("X2"));
            }
            return tempArray;
        }

        private void InitViews() //Initializes UI elements such as PictureBox and Label.
        {
            hashtablePictureBox.Clear();
            hashtablePen.Clear();

            PictureBox[] tempPictureBoxs = new PictureBox[]{
                pictureBox1, pictureBox2, pictureBox3, pictureBox4 ,pictureBox5 ,pictureBox6 ,pictureBox7 ,pictureBox8 ,pictureBox9 ,pictureBox10,
                pictureBox11,pictureBox12,pictureBox13,pictureBox14,pictureBox15,pictureBox16,pictureBox17,pictureBox18,pictureBox19,pictureBox20,
                pictureBox21,pictureBox22,pictureBox23,pictureBox24,pictureBox25,pictureBox26,pictureBox27,pictureBox28,pictureBox29,pictureBox30,
                pictureBox31,pictureBox32,pictureBox33,pictureBox34,pictureBox35,pictureBox36,pictureBox37,pictureBox38,pictureBox39,pictureBox40,
                pictureBox41,pictureBox42,pictureBox43,pictureBox44,pictureBox45,pictureBox46,pictureBox47,pictureBox48,pictureBox49,pictureBox50,
                pictureBox51,pictureBox52,pictureBox53,pictureBox54,pictureBox55,pictureBox56,pictureBox57,pictureBox58,pictureBox59,pictureBox60,};

            Label[] tempLabels = new Label[] {
                label1,label2,label3,label4,label5,label6,label7,label8,label9,label10,
                label11,label12,label13,label14,label15,label16,label17,label18,label19,label20,
                label21,label22,label23,label24,label25,label26,label27,label28,label29,label30,
                label31,label32,label33,label34,label35,label36,label37,label38,label39,label40,
                label41,label42,label43,label44,label45,label46,label47,label48,label49,label50,
                label51,label52,label53,label54,label55,label56,label57,label58,label59,label60
            };
            for (int i = 0; i < bitmaps.Length; i++)
            {
                pictureBoxes[i] = tempPictureBoxs[i];
                labels[i] = tempLabels[i];
                Console.WriteLine(pictureBoxes[i].Width+","+pictureBoxes[i].Height);
                bitmaps[i] = new Bitmap(pictureBoxes[i].Width, pictureBoxes[i].Height);
                graphics[i] = Graphics.FromImage(bitmaps[i]);
                graphics[i].Clear(Color.White);
                firstPoints[i] = true;
                startPoint[i] = new PointF(0, 0);
                endPoint[i] = new PointF(0, 0);
            }

            string[] macArray = GetMacAddress();
            for (int i = 0; i < macArray.Length; i++)
            {
                string mac = macArray[i];
                hashtablePictureBox.Add(mac, pictureBoxes[i]);
                labels[i].Text = mac;
                hashtablePen.Add(mac, new Pen(Color.Black, 0.5f));
            }
        }


        /// <summary>
        /// Check if dongle is inserted 
        /// </summary>
        /// <returns>Whether to insert</returns>
        public bool CheckDongleIsInsert() //Checks if the dongle is inserted.
        {
            PnPEntityInfo[] pnPEntityInfos = USB.WhoPnPEntity(DataPassByStaticField.vid_value2, DataPassByStaticField.pid_value2);
            if (pnPEntityInfos != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void FormZpenSample_Load(object sender, EventArgs e) //Handles the form load event.
        {
            ezUSB.AddUSBEventWatcher(USBEventHandler, USBEventHandler, new TimeSpan(0, 0, 3));
        }

        private void FormZpenSample_FormClosed(object sender, FormClosedEventArgs e) //Handles the form closed event.
        {
            ezUSB.RemoveUSBEventWatcher();
        }


        private void USBEventHandler(object sender, EventArrivedEventArgs e) //Event handler for USB events (insertion/removal).
        {
            foreach (USBControllerDevice Device in USB.WhoUSBControllerDevice(e))
            {
                //  Console.WriteLine(" Dependent : " + Device.Dependent);
                string[] sArray = Device.Dependent.Split(new[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                byte[] arrayTemp = new byte[60];
                for (int i = 0; i < sArray.Length; i++)
                {
                    // Console.WriteLine(" sArray[" + i + "] = " + sArray[i]);
                    if (i == 1)
                    {
                        string[] ids = sArray[i].Split('&');
                        if (sArray[0].Contains(DataPassByStaticField.deviceTypeUSB) && ids[0].Contains(DataPassByStaticField.vid) && ids[0].Contains(DataPassByStaticField.vid_value2.ToString("X2")))
                        {
                            if (sArray[0].Contains(DataPassByStaticField.deviceTypeUSB) && ids[1].Contains(DataPassByStaticField.pid) && ids[1].Contains(DataPassByStaticField.pid_value2.ToString("X2")))
                            {
                                if (e.NewEvent.ClassPath.ClassName == "__InstanceCreationEvent")
                                {
                                    // Insert Dongle 
                                    Console.WriteLine(" insert time  : " + DateTime.Now);
                                    this.labelConnectStatus.BeginInvoke(new Action<String>((msg) =>
                                    {
                                        this.labelConnectStatus.Text = msg;
                                        this.labelConnectStatus.ForeColor = Color.Green;

                                        OpenDevices();
                                        string path = basePath + addressListPath;

                                        byte[,] defaultClientArrayFromFile = GetClientListFromFile(path);
                                        byte[] array = TwoDimensionToOnedDimension(defaultClientArrayFromFile);
                                      
                                        SetClientArray(array);
                                    }), "Bluetooth AP is connected");
                                }
                                else if (e.NewEvent.ClassPath.ClassName == "__InstanceDeletionEvent")
                                {
                                    // Dial Dongle 
                                    Console.WriteLine(" remove time  : " + DateTime.Now);
                                    this.labelConnectStatus.BeginInvoke(new Action<String>((msg) =>
                                    {
                                        this.labelConnectStatus.Text = msg;
                                        this.labelConnectStatus.ForeColor = Color.Red;
                                        CloseDevices();

                                        for (int index=0;index<labels.Length;index++) {

                                            labels[index].ForeColor = Color.Black;

                                        }
                                    }), "Bluetooth AP has been disconnected");
                                }
                            }
                        }
                    }
                }
            }
        }

    }
}

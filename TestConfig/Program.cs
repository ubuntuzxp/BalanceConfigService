using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConsoleApplication1
{
    struct S
    {
        public int a;
        public int b;
    }
    class Program
    {
        const int ASSDK_Err_Success = 0x0000;
        // Progress
        const int ASSDK_Err_Progress = 0x0001;
        // Terminate by hand
        const int ASSDK_Err_Terminate = 0x0002;

        // ProtocolType
        const int ASSDK_ProtocolType_None = 0;
        const int ASSDK_ProtocolType_Pecr = 1;
        const int ASSDK_ProtocolType_Hecr = 2;
        const int ASSDK_ProtocolType_TSecr = 3;

        // ProcType
        const int ASSDK_ProcType_Down = 0;
        const int ASSDK_ProcType_UP = 1;
        const int ASSDK_ProcType_Edit = 2;
        const int ASSDK_ProcType_Del = 3;
        const int ASSDK_ProcType_List = 4;
        const int ASSDK_ProcType_Empty = 5;
        const int ASSDK_ProcType_Reserve = 0x0010;

        // DataType
        const int ASSDK_DataType_PLU = 0x0000;
        const int ASSDK_DataType_Unit = 0x0001;
        const int ASSDK_DataType_Department = 0x0002;
        const int ASSDK_DataType_HotKey = 0x0003;
        const int ASSDK_DataType_Group = 0x0004;
        const int ASSDK_DataType_Discount = 0x0005;
        const int ASSDK_DataType_Origin = 0x0006;
        const int ASSDK_DataType_Country = 0x0007;
        const int ASSDK_DataType_SlaughterHouse = 0x0008;
        const int ASSDK_DataType_Cuttinghall = 0x0009;
        const int ASSDK_DataType_Tare = 0x000A;
        const int ASSDK_DataType_Nutrition = 0x000B;
        const int ASSDK_DataType_Note1 = 0x000C;
        const int ASSDK_DataType_Note2 = 0x000D;
        const int ASSDK_DataType_Note3 = 0x000E;
        //const int ASSDK_DataType_TextMessage = 0x000F;
        const int ASSDK_DataType_Options = 0x0010;
        const int ASSDK_DataType_CustomBarcode = 0x0011;
        const int ASSDK_DataType_LabelPrintRecord = 0x0012;
        const int ASSDK_DataType_HeaderInfo = 0x0013;
        const int ASSDK_DataType_FooterInfo = 0x0014;
        const int ASSDK_DataType_AdvertisementInfo = 0x0015;
        const int ASSDK_DataType_HeaderLogo = 0x0016;
        const int ASSDK_DataType_FooterLogo = 0x0017;
        const int ASSDK_DataType_LabelAdvertisement = 0x0018;
        const int ASSDK_DataType_VendorInfo = 0x0019;
        const int ASSDK_DataType_NutritionElement = 0x001A;
        const int ASSDK_DataType_NutritionInfo = 0x001B;
        const int ASSDK_DataType_Note4 = 0x001C;

        static string DecimalToBinary(int decimalNum)
        {
            string binaryNum = Convert.ToString(decimalNum, 2);
            if (binaryNum.Length < 8)
            {
                for (int i = 0; i < 8 - binaryNum.Length; i++)
                {
                    binaryNum = '0' + binaryNum;
                }
            }
            return binaryNum;
        }

        static void Main(string[] args)
        {
            //StringBuilder sb = new StringBuilder();
            //sb.Append((180373763 >> 24) & 0xFF).Append(".");
            //sb.Append((180373763 >> 16) & 0xFF).Append(".");
            //sb.Append((180373763 >> 8) & 0xFF).Append(".");
            //sb.Append(180373763 & 0xFF);

            //IPAddress ipaddr = new IPAddress(180373763);
            //try
            //{
            //    Task.Run(() => { throw new Exception(); });
            //}
            //catch (Exception)
            //{
            //    Console.WriteLine("1");

            //}
            //Console.WriteLine("2");
            //Console.ReadKey();

            //return;
            //var ssss = DecimalToBinary(29);
            //ReadWriteIntPtr();
            //ReadWriteIntPtr();
            var ini = AclasSDK_Initialize();

            var DeviceInfo = new TASSDKDeviceInfo();

            AclasSDK_GetDevicesInfo(MakeHostToDWord("10.192.73.4"), 0, 0, ref DeviceInfo);
            var listInfo = new List<TASSDKDeviceInfo>();
            for (int i = 0; i < 255; i++)
            {
                listInfo.Add(DeviceInfo);
            }


            IPHostEntry myHost = new IPHostEntry();
            var hostName = Dns.GetHostName();
            var hostEntry = Dns.GetHostEntry(hostName);
            var boIp = GetBroadcast("192.168.10.1", "255.255.254.0");

            var ip = MakeHostToDWord("10.192.73.255");
            //分配内存
            //这边可以改为建立数组吗？
            //可以
            var arry = new TASSDKDeviceInfo[255];
            var res = Marshal.UnsafeAddrOfPinnedArrayElement(arry, 0);

            //分配内存
            //var res = Marshal.AllocHGlobal(255 * Marshal.SizeOf(typeof(TASSDKDeviceInfo)));

            //res为数组首地址
            var count = AclasSDK_GetNetworkSectionDevicesInfo(ip, 0, 0, res, 255);
            byte[] bytes = new byte[10];//你的二进制数组

            var firstInfo = Marshal.PtrToStructure<TASSDKDeviceInfo>(res);

            var secondInfo = Marshal.PtrToStructure<TASSDKDeviceInfo>(new IntPtr(res.ToInt32() + Marshal.SizeOf(firstInfo)));
            //根据首地址循环取出所有元素
            var relist = new List<TASSDKDeviceInfo>();
            for (int i = 0; i < 255; i++)
            {
                TASSDKDeviceInfo info;
                if (i == 0)
                {
                    info = firstInfo;
                }
                else
                {
                    info = (TASSDKDeviceInfo)Marshal.PtrToStructure(res, typeof(TASSDKDeviceInfo));
                    res = new IntPtr(res.ToInt32() + Marshal.SizeOf(info));
                    info = Marshal.PtrToStructure<TASSDKDeviceInfo>(res);
                }
                if (info.ID > 0)
                {
                    relist.Add(info);
                }
            }
            var addr = arry.ToList().Count(a => a.ID > 0);

            TASSDKOnProgressEvent OnProgress = new TASSDKOnProgressEvent(OnProgressEvent);
            AclasSDK_WaitForTask(AclasSDK_ExecTaskA(DeviceInfo.Addr, DeviceInfo.Port, DeviceInfo.ProtocolType,
      ASSDK_ProcType_UP, ASSDK_DataType_PLU, @"D:\syncservice\config\plu12.txt", OnProgress, null));
            AclasSDK_Finalize();
        }

        static void ReadWriteIntPtr()
        {
            var sss = MakeHostToDWord("10.192.73.3");
            var ip = new IPAddress(sss);
            var ini = AclasSDK_Initialize();
            var ipList = new List<TASSDKDeviceInfo>();
            //获取本地机器名 
            string _myHostName = Dns.GetHostName();
            //获取本机IP 
            string _myHostIP = Dns.GetHostEntry(_myHostName).AddressList[1].ToString();
            //截取IP网段
            string ipDuan = _myHostIP.Remove(_myHostIP.LastIndexOf('.'));
            //枚举网段计算机
            for (int i = 1; i <= 255; i++)
            {
                Ping myPing = new Ping();
                //myPing.PingCompleted += new PingCompletedEventHandler(_myPing_PingCompleted);
                //myPing.SendPingAsync
                string pingIP = ipDuan + "." + i.ToString();
                //var re = myPing.Send(pingIP, 1);
                //if (re.Status == IPStatus.Success)
                //{
                //    ipList.Add(re.Address.ToString());
                //}
                var DeviceInfo = new TASSDKDeviceInfo();
                AclasSDK_GetDevicesInfo(MakeHostToDWord(pingIP), 0, 0, ref DeviceInfo);
                if (DeviceInfo.ID > 0)
                {
                    ipList.Add(DeviceInfo);
                }
            }
        }


        [DllImport("AclasSDK.dll")]
        static public extern Boolean AclasSDK_Initialize(Pointer Adjuct = null);
        [DllImport("AclasSDK.dll", CharSet = CharSet.Ansi)]
        static public extern int AclasSDK_GetNetworkSectionDevicesInfo(UInt32 Addr, UInt32 Port, UInt32 ProtocolType,
       IntPtr lpDeviceInfos, UInt32 dwCount);
        [DllImport("AclasSDK.dll", CharSet = CharSet.Ansi)]
        static private extern Boolean AclasSDK_GetDevicesInfo(UInt32 Addr, UInt32 Port, UInt32 ProtocolType, ref TASSDKDeviceInfo DeviceInfo);
        [DllImport("AclasSDK.dll")]
        static public extern void AclasSDK_WaitForTask(IntPtr TaskHandle);
        [DllImport("AclasSDK.dll")]
        static public extern IntPtr AclasSDK_ExecTaskA(UInt32 Addr, UInt32 Port, UInt32 ProtocolType, UInt32 ProcType, UInt32 DataType,
    string FileName, TASSDKOnProgressEvent OnProgress, Pointer lpUserData);
        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
        public delegate void TASSDKOnProgressEvent(uint nErrorCode, uint Index, uint Total, IntPtr lpUserData);
        [DllImport("AclasSDK.dll")]
        static public extern void AclasSDK_Finalize();

        public static void OnProgressEvent(UInt32 nErrorCode, UInt32 Index, UInt32 Total, IntPtr lpUserData)
        {
            const string sInfoProgress = "Progress: {0}/{1}";
            const string sInfoComplete = "Complete, Total: {0}";
            const string sInfoStop = "Proc Stop!";
            const string sInfoFailed = "Proc Failed!";

            switch (nErrorCode)
            {
                case ASSDK_Err_Success:
                    {
                        MessageBox.Show(string.Format(sInfoComplete, Total));
                        break;
                    }
                case ASSDK_Err_Progress:
                    {
                        //MessageBox.Show(string.Format(sInfoProgress, Index, Total));                        
                        break;
                    }
                case ASSDK_Err_Terminate:
                    {
                        MessageBox.Show(sInfoStop);
                        break;
                    }
                default:
                    MessageBox.Show(sInfoFailed);
                    break;
            }
        }

        public static string GetBroadcast(string ipAddress, string subnetMask)
        {

            byte[] ip = IPAddress.Parse(ipAddress).GetAddressBytes();
            byte[] sub = IPAddress.Parse(subnetMask).GetAddressBytes();

            // 广播地址=子网按位求反 再 或IP地址 
            for (int i = 0; i < ip.Length; i++)
            {
                ip[i] = (byte)((~sub[i]) | ip[i]);
            }
            return new IPAddress(ip).ToString();
        }

        public static uint MakeHostToDWord(string sHost)
        {
            int i;
            string[] Segment;
            uint result;
            result = 0;

            Segment = sHost.Split('.');
            if (Segment.Length != 4)
                return result;
            for (i = 0; i < (Segment.Length); i++)
            {
                if ((Convert.ToUInt32(Segment[i]) >= 0) && (Convert.ToUInt32(Segment[i]) <= 255))
                {
                    result = result + Convert.ToUInt32(Convert.ToUInt32(Segment[i]) << ((3 - i) * 8));
                }
                else
                    return result;
            }
            return result;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Explicit, Size = 257)]
        private struct TASSDKDeviceInfo
        {
            [FieldOffset(0)]
            public UInt32 ProtocolType; // ProtocolType
            [FieldOffset(4)]
            public UInt32 Addr;
            [FieldOffset(8)]
            public UInt32 Port;
            [FieldOffset(12)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] Name;
            [FieldOffset(28)]
            public UInt32 ID;
            [FieldOffset(32)]
            public UInt32 Version;
            [FieldOffset(36)]
            public Byte Country;
            [FieldOffset(37)]
            public Byte DepartmentID;
            [FieldOffset(38)]
            public Byte KeyType;
            [FieldOffset(39)]
            public UInt64 PrinterDot;
            [FieldOffset(47)]
            public UInt64 PrnStartDate;
            [FieldOffset(55)]
            public UInt32 LabelPage;
            [FieldOffset(59)]
            public UInt32 PrinterNo;
            [FieldOffset(63)]
            public UInt16 PLUStorage;
            [FieldOffset(65)]
            public UInt16 HotKeyCount;
            [FieldOffset(67)]
            public UInt16 NutritionStorage;
            [FieldOffset(69)]
            public UInt16 DiscountStorage;
            [FieldOffset(71)]
            public UInt16 Note1Storage;
            [FieldOffset(73)]
            public UInt16 Note2Storage;
            [FieldOffset(75)]
            public UInt16 Note3Storage;
            [FieldOffset(77)]
            public UInt16 Note4Storage;

            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 177)]
            //[FieldOffset(79)]
            //public byte[] Adjuct;
        }
    }
}

using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace SyncConfigService
{
    public class ConfigBusiness
    {
        private static Logger log = LogManager.GetCurrentClassLogger();
        private static readonly HttpClient httpClient = new HttpClient();
        private static string GetConfigDataURL = ConfigurationManager.AppSettings["GetConfigDataURL"];
        private static string ConfigDirectory = ConfigurationManager.AppSettings["ConfigDirectory"];
        private static string BaseDirectory = ConfigurationManager.AppSettings["BaseDirectory"];

        #region 配置信息
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

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Explicit, Size = 256)]
        public struct TASSDKDeviceInfo
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
            //[FieldOffset(80)]
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 177)]
            //public byte[] Adjuct;
        }
        #endregion
        #region 非托管代码
        [DllImport("AclasSDK.dll")]
        static public extern Boolean AclasSDK_Initialize(Pointer Adjuct = null);
        [DllImport("AclasSDK.dll")]
        static public extern void AclasSDK_Finalize();
        [DllImport("AclasSDK.dll", CharSet = CharSet.Ansi)]
        static public extern int AclasSDK_GetNetworkSectionDevicesInfo(UInt32 Addr, UInt32 Port, UInt32 ProtocolType,
       IntPtr lpDeviceInfos, UInt32 dwCount);
        [DllImport("AclasSDK.dll", CharSet = CharSet.Ansi)]
        static public extern Boolean AclasSDK_GetDevicesInfo(UInt32 Addr, UInt32 Port, UInt32 ProtocolType, ref TASSDKDeviceInfo DeviceInfo);
        [DllImport("AclasSDK.dll")]
        static public extern void AclasSDK_WaitForTask(IntPtr TaskHandle);
        [DllImport("AclasSDK.dll")]
        static public extern IntPtr AclasSDK_ExecTaskA(UInt32 Addr, UInt32 Port, UInt32 ProtocolType, UInt32 ProcType, UInt32 DataType,
    string FileName, TASSDKOnProgressEvent OnProgress, Pointer lpUserData);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void TASSDKOnProgressEvent(uint nErrorCode, uint Index, uint Total, IntPtr lpUserData);
        #endregion


        /// <summary>
        /// 同步PLU
        /// </summary>
        public static void SyncConfig()
        {
            var versionPath = $@"{ConfigDirectory}\version.ini";
            var idPath = $@"{ConfigDirectory}\storeid.ini";
            var pluPath = $@"{ConfigDirectory}\plu.txt";
            var note1Path = $@"{ConfigDirectory}\note1.txt";
            var note2Path = $@"{ConfigDirectory}\note2.txt";
            var version = ReadFileInfo(versionPath);
            var stroreid = ReadFileInfo(idPath);
            //仓库Id未设置就返回
            if (string.IsNullOrEmpty(stroreid))
            {
                log.Error("请配置仓库Id");
                return;
            }
            var list = GetAllInfos();
            var isAddNew = IsAddNew(list);
            var balanceList = new List<BalanceInfo>();
            list.ForEach(a =>
            {
                balanceList.Add(new BalanceInfo { Id = a.ID.ToString(), Ip = a.Addr.ToString() });
            });

            //var res = RequestUrl(GetConfigDataURL, new Dictionary<string, string>
            //      {
            //        {"stroreid", stroreid},
            //        { "balanceinfo",JsonConvert.SerializeObject( balanceList)},
            //        { "localversion",string.IsNullOrEmpty(version)?string.Empty:version.Split(',')[0]},
            //        { "localupdatetime",string.IsNullOrEmpty(version)?string.Empty:version.Split(',')[1]},
            //        { "isupdate",isAddNew.ToString().ToLower()},
            //      });

            //如果添加新秤就请求更新数据
            var localversion = isAddNew ? string.Empty : string.IsNullOrEmpty(version) ? string.Empty : version.Split(',')[0];
            var localupdatetime = string.IsNullOrEmpty(version) ? string.Empty : version.Split(',')[1];
            var url = $@"{GetConfigDataURL}?stroreid={stroreid}&balanceinfo={JsonConvert.SerializeObject(balanceList)}&localversion={localversion}&localupdatetime={localupdatetime}&isupdate={isAddNew.ToString().ToLower()}";
            var res = GetUrl(url);
            log.Info("参数信息 " + url);
            log.Info("返回信息 " + res);
            var model = JsonConvert.DeserializeObject<ResponseModel>(res);
            log.Info("所有秤信息:" + JsonConvert.SerializeObject(list));
            if (model.data.IsUpdate)
            {
                CreatePLU(pluPath, model.data.PluInfo);
                CreateNote(note1Path, model.data.PluInfo, "最佳食用时间");
                CreateNote(note2Path, model.data.PluInfo, "前");
                list.ForEach(a =>
                {
                    AclasSDK_WaitForTask(AclasSDK_ExecTaskA(a.Addr, a.Port, a.ProtocolType,
              ASSDK_ProcType_Down, ASSDK_DataType_PLU, pluPath, null, null));
                    log.Info($"秤{a.ID}PLU更新成功！");
                    AclasSDK_WaitForTask(AclasSDK_ExecTaskA(a.Addr, a.Port, a.ProtocolType,
    ASSDK_ProcType_Down, ASSDK_DataType_Note1, note1Path, null, null));
                    log.Info($"秤{a.ID}Note1更新成功！");
                    AclasSDK_WaitForTask(AclasSDK_ExecTaskA(a.Addr, a.Port, a.ProtocolType,
ASSDK_ProcType_Down, ASSDK_DataType_Note2, note1Path, null, null));
                    log.Info($"秤{a.ID}Note2更新成功！");
                    //备份PLU
                    BackUpPLU(a);
                    log.Info($"秤{a.ID}备份PLU成功！");
                });
                //更新本地版本
                FileOperation(versionPath, model.data.Version.ToString() + "," + DateTime.Now.ToUnix());

            }
        }

        /// <summary>
        /// 创建PLU文件
        /// </summary>
        private static void CreatePLU(string path, List<PLUInfo> info)
        {
            var builder = new StringBuilder();
            var t = typeof(PLUInfo);
            var fileds = t.GetProperties();
            log.Debug(fileds.Length);
            fileds.ToList().ForEach(a =>
            {
                builder.Append($"{a.Name}\t");
            });
            builder.Append(Environment.NewLine);
            info.ForEach(a =>
            {
                //a.Note1 = a.ValidDate == 0 ? string.Empty : "最佳食用时间";
                //a.Note2 = a.ValidDate == 0 ? string.Empty : "前";
                a.Label1ID = 1;
                a.BarcodeType1 = 150;
                a.Flag1 = a.ValidDate == 0 ? Convert.ToByte(0) : Convert.ToByte(29);
                a.ID = a.ItemCode;
                fileds.ToList().ForEach(f =>
                {
                    var propertyInfo = typeof(PLUInfo).GetProperty(f.Name);
                    var obj = propertyInfo.GetValue(a);
                    builder.Append($"{obj}\t");
                });
                builder.Append(Environment.NewLine);
            });
            FileOperation(path, builder.ToString());
        }

        /// <summary>
        /// 创建note文件
        /// </summary>
        private static void CreateNote(string path, List<PLUInfo> info, string value)
        {
            var builder = new StringBuilder();
            builder.Append($"PLUID\t");
            builder.Append($"Value\t");
            builder.Append(Environment.NewLine);
            info.ForEach(a =>
            {
                var str = a.ValidDate == 0 ? string.Empty : value;
                builder.Append($"{a.ID}\t");
                builder.Append($"{str}\t");
                builder.Append(Environment.NewLine);
            });
            FileOperation(path, builder.ToString());
        }

        /// <summary>
        /// 备份PLU文件
        /// </summary>
        private static void BackUpPLU(TASSDKDeviceInfo info)
        {
            var backPath = $@"{BaseDirectory}\pluback";
            if (!Directory.Exists(backPath))
            {
                Directory.CreateDirectory(backPath);
            }

            AclasSDK_WaitForTask(AclasSDK_ExecTaskA(info.Addr, info.Port, info.ProtocolType,
ASSDK_ProcType_UP, ASSDK_DataType_PLU, $@"{backPath}\{info.ID}\backplu_{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}.txt", null, null));
            //File.Copy(path, $@"{ backPath}\backplu_{ DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}.txt");
        }

        /// <summary>
        /// 获取此网络下所有秤信息
        /// </summary>
        private static List<TASSDKDeviceInfo> GetAllInfos()
        {
            IPHostEntry myHost = new IPHostEntry();
            var hostName = Dns.GetHostName();
            var hostEntry = Dns.GetHostEntry(hostName);
            var addr = Dns.GetHostAddresses(hostName)
.Where(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
.First().ToString();
            var boIp = GetBroadcast(addr, "255.255.255.0");
            var ip = MakeHostToDWord(boIp);
            //分配内存
            var arry = new TASSDKDeviceInfo[255];
            var res = Marshal.UnsafeAddrOfPinnedArrayElement(arry, 0);
            //res为数组首地址
            var count = AclasSDK_GetNetworkSectionDevicesInfo(ip, 0, 0, res, 255);
            var firstInfo = Marshal.PtrToStructure<TASSDKDeviceInfo>(res);
            var list = new List<TASSDKDeviceInfo>();
            //根据首地址循环取出所有元素
            for (int i = 0; i < 255; i++)
            {
                var info = new TASSDKDeviceInfo();
                if (i == 0)
                {
                    info = firstInfo;
                }
                else
                {
                    info = Marshal.PtrToStructure<TASSDKDeviceInfo>(res);
                    res = new IntPtr(res.ToInt32() + Marshal.SizeOf(info));
                    info = Marshal.PtrToStructure<TASSDKDeviceInfo>(res);
                }
                if (info.ID > 0)
                {
                    list.Add(info);
                }
            }
            return list;
        }

        /// <summary>
        /// 是否有新秤
        /// </summary>
        private static bool IsAddNew(List<TASSDKDeviceInfo> list)
        {
            var path = $@"{ConfigDirectory}\info.ini";
            var ids = string.Empty;
            list = list.OrderByDescending(a => a.ID).ToList();
            list.ForEach(a =>
            {
                ids += a.ID + ",";
            });
            ids = ids.TrimEnd(',');
            //读出秤配置信息
            if (!File.Exists(path))
            {
                //文件不存在就更新
                FileOperation(path, ids);
                return true;
            }
            var str = File.ReadAllText(path);
            //添加了新秤就更新
            if (ids != str)
            {
                FileOperation(path, ids);
                return true;
            }
            else
                return false;
        }

        private static void UpdateInfo(List<TASSDKDeviceInfo> list)
        {
            var path = $@"{ConfigDirectory}\info.ini";
            var ids = string.Empty;
            list.ForEach(a =>
            {
                ids += a.ID + ",";
            });
            ids = ids.TrimEnd(',');
            FileOperation(path, ids);
        }
        /// <summary>
        /// 本地版本
        /// </summary>,
        /// <param name="path"></param>
        public static string ReadFileInfo(string path)
        {
            //文件不存在就更新
            if (!File.Exists(path))
            {
                return string.Empty;
            }
            var str = File.ReadAllText(path);
            return str;
        }

        private static string GetBroadcast(string ipAddress, string subnetMask)
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

        public static void FileOperation(string path, string data)
        {
            if (!Directory.Exists(ConfigDirectory))
            {
                Directory.CreateDirectory(ConfigDirectory);
            }
            File.WriteAllText(path, data, Encoding.Unicode);
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

        private static string RequestUrlJson(string url, Dictionary<string, object> parameters)
        {
            var response = httpClient.PostAsync(url, new StringContent(JsonConvert.SerializeObject(parameters)));
            log.Info("参数信息 " + JsonConvert.SerializeObject(parameters));
            var responseContent = response.Result.Content.ReadAsStringAsync().Result;
            return responseContent;
        }

        /// <summary>
        /// HttpPost请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string RequestUrl(string url, Dictionary<string, string> parameters)
        {
            log.Info("参数信息 " + JsonConvert.SerializeObject(parameters));
            var response = httpClient.PostAsync(url, new FormUrlEncodedContent(parameters));
            var responseContent = response.Result.Content.ReadAsStringAsync().Result;
            return responseContent;
        }

        /// <summary>
        /// HttpGet请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetUrl(string url)
        {
            var response = httpClient.GetAsync(url);
            var responseContent = response.Result.Content.ReadAsStringAsync().Result;
            return responseContent;
        }
    }
}

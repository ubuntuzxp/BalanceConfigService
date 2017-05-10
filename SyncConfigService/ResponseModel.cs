using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncConfigService
{
    public class ResponseModel
    {
        public Data data { get; set; }
        public string success { get; set; }
        public int code { get; set; }
        public string msg { get; set; }
        public long server_time { get; set; }
        public float exec_time { get; set; }
    }

    public class Data
    {
        public bool IsUpdate { get; set; }
        public List<PLUInfo> PluInfo { get; set; }
        public string Message { get; set; }
        public string Version { get; set; }
    }
    public class PLUInfo
    {
        public string ID { get; set; }
        //public string DepartmentID { get; set; }
        //public string GroupID { get; set; }
        public string ItemCode { get; set; }
        public string Name1 { get; set; }
        public string Name2 { get; set; }
        public string Name3 { get; set; }
        public int Label1ID { get; set; }
        //public string Label2ID { get; set; }
        public int BarcodeType1 { get; set; }
        //public string BarcodeType2 { get; set; }
        //public string UnitID { get; set; }
        public float Price { get; set; }
        /// <summary>
        /// 保鲜天（小时）
        /// </summary>
        public int FreshnessDate { get; set; }
        //public string PackageRange { get; set; }
        //public string PackageType { get; set; }
        //public string PackageWeight { get; set; }
        //public string Text1ID { get; set; }
        //public string Text2ID { get; set; }
        //public string Text3ID { get; set; }
        //public string Text4ID { get; set; }
        //public string DiscountID { get; set; }
        public byte Flag1 { get; set; }
        //public string Flag2 { get; set; }
        //public string TareID { get; set; }
        //public string ICEValue { get; set; }
        //public string ProducedDate { get; set; }
        //public string PackageDays { get; set; }
        //public string PackageHours { get; set; }
        //public string Flag3 { get; set; }
        //public string OriginID { get; set; }
        //public string DiscountRate { get; set; }
        //public string TareValue { get; set; }
        //public string HalfDiscount { get; set; }
        //public string QuarterDiscount { get; set; }
        //public string Tax1 { get; set; }
        //public string Tax2 { get; set; }
        //public string Tax3 { get; set; }
        //public string Tax4 { get; set; }
        /// <summary>
        /// 保质期
        /// </summary>
        public int ValidDate { get; set; }
        //public string Text5ID { get; set; }
        //public string Text6ID { get; set; }


        //public string Text7ID { get; set; }
        //public string Text8ID { get; set; }
        //public string LimitPrice { get; set; }
        //public string TraceabilityCode { get; set; }
        //public string PackagePrice { get; set; }
        //public string ProducedDateRule { get; set; }
        //public string PackageDateFrom { get; set; }
        //public string FreshnessDateFrom { get; set; }

        //public string ValidDateFrom { get; set; }
        //public string DiscountBeginTime { get; set; }
        //public string DiscountEndTime { get; set; }
        //public string DiscountPrice { get; set; }

        //public string DiscountFlag { get; set; }
        //public string Message1 { get; set; }
        //public string UnitPrintName { get; set; }

    }
}

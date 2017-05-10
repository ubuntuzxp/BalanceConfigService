using System;
using System.Globalization;

namespace SyncConfigService
{
    public static class DateTimeExt
    {
        private static readonly DateTime MinDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly ChineseLunisolarCalendar LunarCalendar = new ChineseLunisolarCalendar();

        public static long ToUnix(this DateTime dt)
        {
            return (long)dt.Subtract(MinDate).TotalSeconds;
        }

        public static DateTime ToDateTime(this long target)
        {
            return MinDate.AddMilliseconds(target);
        }
        /// <summary>
        /// yyyy-MM-dd HH:mm:ss
        /// </summary>
        public static string ToDateTimeStr(this long target, string format = DateFormat.yyyyMMddHHmmss)
        {
            return MinDate.AddMilliseconds(target).ToString(format);
        }
        /// <summary>
        /// yyyy-MM-dd HH:mm:ss
        /// </summary>
        public static string ToDateTimeStr(this DateTime date, string format = DateFormat.yyyyMMddHHmmss)
        {
            return date.ToString(format);
        }
        /// <summary>
        /// yyyy-MM-dd
        /// </summary>
        public static string ToDateStr(this long target, string format = DateFormat.yyyyMMdd)
        {
            return MinDate.AddMilliseconds(target).ToString(format);
        }
        /// <summary>
        /// yyyy-MM-dd
        /// </summary>
        public static string ToDateStr(this DateTime date, string format = DateFormat.yyyyMMdd)
        {
            return date.ToString(format);
        }
        public static DateTime ToDateTime(this int target)
        {
            return MinDate.AddSeconds(target);
        }
        /// <summary>
        /// 距离明天分钟数
        /// </summary>
        public static int MinutesBeforeTomorrow(this DateTime date)
        {
            return 24 * 60 - date.Hour * 60 - date.Minute;
        }

        /// <summary>
        /// 今天0点
        /// </summary>
        public static DateTime TodayBegin(this DateTime date)
        {
            return date.Add(-date.TimeOfDay);
        }
        /// <summary>
        /// 明天0点
        /// </summary>
        public static DateTime TomorrowBegin(this DateTime date)
        {
            return date.Add(-date.TimeOfDay).AddDays(1);
        }


        private static DateTime UnixMinDate = new DateTime(1970, 1, 1);

        public static string MinObjectId(this DateTime date)
        {
            return $"{((long)Math.Floor((date.ToUniversalTime() - UnixMinDate).TotalSeconds)).ToString("x8")}0000000000000000";
        }
        public static string MaxObjectId(this DateTime date)
        {
            return $"{((long)Math.Floor((date.ToUniversalTime() - UnixMinDate).TotalSeconds)).ToString("x8")}ffffffffffffffff";
        }



    }

    public class DateFormat
    {
        public const string yyyyMMdd = "yyyy-MM-dd";
        public const string yyyyMMddHHmmss = "yyyy-MM-dd HH:mm:ss";
    }
}

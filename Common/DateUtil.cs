using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Common
{
    public class DateUtil
    {
        //trade delay
        static int deley = 10;

        public static List<DateUnit> ConvertHourlyDateUnit(DateTime start, DateTime end)
        {
            List<DateUnit> list = new List<DateUnit>();
            for (DateTime dateTime = start; dateTime <= end; dateTime += TimeSpan.FromDays(1))
            {
                if (!(dateTime.Date.DayOfWeek == DayOfWeek.Saturday || dateTime.Date.DayOfWeek == DayOfWeek.Sunday))
                {
                    list.Add(new DateUnit()
                    {
                        Start = new DateTime(dateTime.Year,dateTime.Month,dateTime.Day,9,30,0),
                        End = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 10, 30, 0),
                    });
                    list.Add(new DateUnit()
                    {
                        Start = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 10, 30, 0),
                        End = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 11, 30, deley),
                    });
                    list.Add(new DateUnit()
                    {
                        Start = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 13, 00, 0),
                        End = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 14, 00, 0),
                    });
                    list.Add(new DateUnit()
                    {
                        Start = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 14, 00, 0),
                        End = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 15, 00, deley),
                    });

                }
            }
            return list;
        }

        public static List<DateUnit> ConvertBiHourlyDateUnit(DateTime start, DateTime end)
        {
            List<DateUnit> list = new List<DateUnit>();
            for (DateTime dateTime = start; dateTime <= end; dateTime += TimeSpan.FromDays(1))
            {
                if (!(dateTime.Date.DayOfWeek == DayOfWeek.Saturday || dateTime.Date.DayOfWeek == DayOfWeek.Sunday))
                {
                    list.Add(new DateUnit()
                    {
                        Start = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 9, 30, 0),
                        End = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 11, 30, deley),
                    });
                    list.Add(new DateUnit()
                    {
                        Start = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 13, 00, 0),
                        End = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 15, 00, deley),
                    });

                }
            }
            return list;
        }

        public static bool isWeekend(DateTime dateTime)
        {
            return (dateTime.Date.DayOfWeek == DayOfWeek.Saturday || dateTime.Date.DayOfWeek == DayOfWeek.Sunday);
        }
        public static bool isSaturday
            (DateTime dateTime)
        {
            return (dateTime.Date.DayOfWeek == DayOfWeek.Saturday);
        }
        public static bool isSunday(DateTime dateTime)
        {
            return ( dateTime.Date.DayOfWeek == DayOfWeek.Sunday);
        }
        public static List<DateUnit> ConvertDailyDateUnit(DateTime start, DateTime end)
        {
            List<DateUnit> list = new List<DateUnit>();
            for (DateTime dateTime = start; dateTime <= end; dateTime += TimeSpan.FromDays(1))
            {
                if (!(dateTime.Date.DayOfWeek == DayOfWeek.Saturday || dateTime.Date.DayOfWeek == DayOfWeek.Sunday))
                {

                    list.Add(new DateUnit()
                    {
                        Start = GetStartTradeTime(dateTime),
                        End = GetEndTradeTime(dateTime)
                    });
                }
            }
            return list;
        }

        public static List<DateUnit> ConvertWeeklyDateUnit(DateTime start, DateTime end)
        {
            List<DateUnit> list = new List<DateUnit>();
            DateTime s=GetThisMonday(start);
            DateTime e;
            for (DateTime dateTime = GetThisMonday(start); dateTime <= GetThisFriday(end); dateTime += TimeSpan.FromDays(1))
            {
                if (!(dateTime.Date.DayOfWeek == DayOfWeek.Saturday || dateTime.Date.DayOfWeek == DayOfWeek.Sunday))
                {
                    if (dateTime.DayOfWeek == DayOfWeek.Friday)
                    {
                        e = dateTime;
                        list.Add(new DateUnit()
                        {
                            Start = GetStartTradeTime(s),
                            End = GetEndTradeTime(e)
                        });
                    }
                    if (dateTime.DayOfWeek == DayOfWeek.Monday)
                    {
                        s = dateTime;
                    }
                }
            }
            return list;
        }

        public static List<DateUnit> ConvertMonthlyDateUnit(DateTime start, DateTime end)
        {
            List<DateUnit> list = new List<DateUnit>();
            DateTime s = GetThisFirstDate(start);
            DateTime e;
            for (DateTime dateTime = GetThisFirstDate(start); dateTime <= GetThisLastDate(end); dateTime += TimeSpan.FromDays(1))
            {
                if (dateTime.Day == GetMonthNumber(dateTime))
                    {
                        e = dateTime;
                        list.Add(new DateUnit()
                        {
                            Start = GetStartTradeTime(s),
                            End = GetEndTradeTime(e)
                        });
                    }
                    if (dateTime.Day == 1)
                    {
                        s = dateTime;
                    }
            }
            return list;
        }

        private static DateTime GetThisFirstDate(DateTime d)
        {
            return GetStartTradeTime(d.AddDays(1 - d.Day));
        }
        private static DateTime GetThisLastDate(DateTime d)
        {
            return GetEndTradeTime(d.AddDays(GetMonthNumber(d) - d.Day));
        }

        public  static int GetMonthNumber(DateTime d)
        {
            int[] t = { 1, 3, 5, 7, 8, 10, 12 };
            int[] t1 = { 4, 6, 9, 11 };
            int dt = 0;
            if (t.Contains<int>(d.Month))
                dt = 31;
            if (t1.Contains<int>(d.Month))
                dt = 30;
            if (d.Month == 2)
            {
                if (d.Year % 4 == 0)
                {
                    dt = 29;
                }
                else
                {
                    dt = 28;
                }
            }
            return dt;
        }


        public static int GetDateDiff(DateTime date1,DateTime date2)
        { 
            TimeSpan span = date1.Subtract(date2);
            return span.Days;
        }
        private static DateTime GetStartTradeTime(DateTime d)
        {
            return new DateTime(d.Year, d.Month, d.Day, 9, 30, 0);
        }
        private static DateTime GetEndTradeTime(DateTime d)
        {
            return new DateTime(d.Year, d.Month, d.Day, 15, 0, deley);
        }
        private static DateTime GetThisFriday(DateTime d)
        {
            DateTime dd = new DateTime(d.Year, d.Month, d.Day, 15, 0, deley);

            return GetEndTradeTime(dd.AddDays(5 - (int)d.DayOfWeek));
        }

        private static DateTime GetLastFriday(DateTime d)
        {
            DateTime dd = new DateTime(d.Year, d.Month, d.Day, 15, 05, 0);
            int c = 0;
            if (d.DayOfWeek >= DayOfWeek.Friday)
            {
                c = 5 - (int)d.DayOfWeek;

            }
            if (d.DayOfWeek < DayOfWeek.Friday)
            {
                c = -2 - (int)d.DayOfWeek;
            }
            return GetEndTradeTime(dd.AddDays(c));
        }

        private static DateTime GetNextMonday(DateTime d)
        {
            return GetStartTradeTime(d.AddDays(8 - (int)d.DayOfWeek));
        }

        private static DateTime GetThisMonday(DateTime d)
        {
            return GetStartTradeTime(d.AddDays(1 - (int)d.DayOfWeek));
        }

        /// <summary>
        /// 判断是不是节假日,节假日返回true 
        /// </summary>
        /// <param name="date">日期格式：yyyy-MM-dd</param>
        /// <returns></returns>
        public static bool IsHolidayByDate(string date)
        {
            bool isHoliday = false;
            System.Net.WebClient WebClientObj = new System.Net.WebClient();
            System.Collections.Specialized.NameValueCollection PostVars = new System.Collections.Specialized.NameValueCollection();
            PostVars.Add("d", date.Replace("-", ""));//参数
            try
            {
                byte[] byRemoteInfo = WebClientObj.UploadValues(@"http://www.easybots.cn/api/holiday.php", "POST", PostVars);//请求地址,传参方式,参数集合
                string sRemoteInfo = System.Text.Encoding.UTF8.GetString(byRemoteInfo);//获取返回值

                string result = JObject.Parse(sRemoteInfo)[date.Replace("-", "")].ToString();
                if (result == "0")
                {
                    isHoliday = false;
                }
                else if (result == "1" || result == "2")
                {
                    isHoliday = true;
                }
            }
            catch
            {
                isHoliday = isWeekend(DateTime.Parse(date));
            }
            return isHoliday;
        }
        public static bool IsHolidayByDate(DateTime date)
        {
            return IsHolidayByDate(FormatDate(date));
        }
        public static DateTime GetCeilingWorkDay(DateTime d)
        {
            DateTime ret = new DateTime();
            int i = 20;
            while (i > 0)
            {
                if (IsHolidayByDate(d) || isWeekend(d))
                {
                    d = d.AddDays(-1);
                }
                else
                {
                    ret = d;
                    break;
                }
                i--;
            }

            return ret;
        }

        public  static DateTime GetCeilingWorkDay(string d)
        {
            return GetCeilingWorkDay(DateTime.Parse(d));
        }
        public static string FormatDate(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }
    }


 

    public struct DateUnit
    {
        public DateTime Start;
        public DateTime End;
    }
}

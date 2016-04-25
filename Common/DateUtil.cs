using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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



    }

    public struct DateUnit
    {
        public DateTime Start;
        public DateTime End;
    }
}

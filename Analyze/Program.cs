﻿using big;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analyze
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int day_before = 0;
            if (args.Length == 0)
            {
                day_before = -Constant.DAYS_BEFORE;
            }
            else
            {
                day_before = int.Parse(args[0]);
            }

            exec(day_before);
        }


        public static void exec(int day_before)
        {
            int big = 500;
            DateTime now = DateTime.Now;
            string tag = now.ToString("yyyyMMdd");
            DateTime end = now.AddDays(day_before);
            DateTime start = new DateTime();
            string[] list = Constant.ANALYZE_TIME.Split('-');

            BizApi.DeleteAnalyzeData(tag,big);
            foreach (string month in list)
            {
                start = end.AddMonths(-int.Parse(month));
                BizApi.InsertAnalyzeData(tag, start, end,month,big);
            }
        }
    }
}

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

            int[] list = new int[] { 3, 6, 12, 24 };
            exec(list, day_before);
        }


        public static void exec(int[] a, int day_before)
        {
            DateTime now = DateTime.Now;
            string tag = now.ToString("yyyyMMdd");
            DateTime end = now.AddDays(day_before);
            DateTime start = new DateTime();
            foreach (int i in a)
            {
                start = end.AddMonths(-i);
                BizApi.InsertAnalyzeData(tag, start, end);
            }
        }
    }
}

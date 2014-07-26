﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using SotckAnalyzer;
using DownloadData;
using Report;
using Common.type;

namespace Modulizer
{
    class Program
    {
        public static void Main(string[] args)
        {

            //List<DateUnit> u=DateUtil.ConvertMonthlyDateUnit(DateTime.Parse("2012-09-03"), DateTime.Now);

            ModuleInfo info =new ModuleInfo();

            info.stock = "sz002424";
            info.filter = "1000";
            List <IStockModule> list= new List<IStockModule>();
            list.Add(new DownloadModule());
            list.Add(new AnalyzeModule());
            list.Add(new ReportModule());

            foreach (IStockModule module in list)
            {
                module.Execute(info);
            }

        }
    }
}
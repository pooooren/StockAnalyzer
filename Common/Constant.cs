﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections;
using System.Xml;
using System.IO;

namespace Common
{
    public  class Constant
    {

        private static Dictionary<string, string> map = new Dictionary<string, string>();
        private static Constant t = new Constant();
        private  Constant()
        {
            XmlDocument doc = new XmlDocument();
            //Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
            doc.Load(AppDomain.CurrentDomain.BaseDirectory+"app.xml");

            XmlNode node = doc.SelectSingleNode("/StockApp");
      
            foreach (XmlNode n in node.ChildNodes)
            {
                map.Add(n.Name, n.InnerText);
            }

            if (!Directory.Exists(map["rootFolder"]))
            {
                Directory.CreateDirectory(map["rootFolder"]);
            }
            if (!Directory.Exists(map["analyzeFolder"]))
            {
                Directory.CreateDirectory(map["analyzeFolder"]);
            }
        }

        public static string ROOT_FOLDER = map["rootFolder"].EndsWith(@"\") ? map["rootFolder"] : map["rootFolder"] + @"\";// doc.SelectSingleNode("");
        public static string ANALYZE_FOLDER = map["analyzeFolder"].EndsWith(@"\") ? map["analyzeFolder"]  : map["analyzeFolder"] + @"\";
        public static string BIG_DEAL = map["bigDeal"].Equals("") ? "500" : map["bigDeal"];
        public static string STOCK_FILE =  map["stockFile"];
        public static string AUTO_DOWNLOAD_FILE = map["autoDownloadFile"];
        public static string ANALYZE_FILE = map["analyzeFile"].Equals("") ? ROOT_FOLDER + @"\analyze.txt" : map["analyzeFile"];
        public static string UPDATE_FILE = map["updateFile"].Equals("") ? ROOT_FOLDER + @"\{0}\update.txt" : map["updateFile"];
        public static string DOWNLOAD_ALL = map["downloadAll"].Equals("") ? "0" : map["downloadAll"];
        public static string ANALYZE_START_DATE = map["analyzeStartDate"].Equals("") ? "2012-09-01" : map["analyzeStartDate"];
        public static string ANALYZE_CHART_DIR =  map["analyeChartDir"];
        public static bool CLEAN = Boolean.Parse(map["clean"]);
    
    }
}

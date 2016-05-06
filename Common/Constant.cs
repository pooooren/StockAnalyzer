using System;
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
        public Constant()
        {
            if (t == null)
            {
                XmlDocument doc = new XmlDocument();
                string file = "";
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "app.xml"))
                {
                    file = AppDomain.CurrentDomain.BaseDirectory + "app.xml";
                }
                else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "bin\\app.xml"))
                {
                    file = AppDomain.CurrentDomain.BaseDirectory + "bin\\app.xml";
                }
                else
                {
                    throw new Exception(AppDomain.CurrentDomain.BaseDirectory + " app.xml" + " not found!!");
                }
                //Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
                doc.Load(file);

                XmlNode node = doc.SelectSingleNode("/StockApp");

                foreach (XmlNode n in node.ChildNodes)
                {
                    map.Add(n.Name, n.InnerText);
                }

                //if (!Directory.Exists(map["rootFolder"]))
                //{
                //    Directory.CreateDirectory(map["rootFolder"]);
                //}
                //if (!Directory.Exists(map["analyzeFolder"]))
                //{
                //    Directory.CreateDirectory(map["analyzeFolder"]);
                //}
            }
        }

        public static string ROOT_FOLDER = map["rootFolder"].EndsWith(@"\") ? map["rootFolder"] : map["rootFolder"] + @"\";// doc.SelectSingleNode("");
        public static string ANALYZE_FOLDER = map["analyzeFolder"].EndsWith(@"\") ? map["analyzeFolder"]  : map["analyzeFolder"] + @"\";
        public static string BIG_DEAL = map["bigDeal"].Equals("") ? "500" : map["bigDeal"];
        public static string STOCK_FILE = map["stockFile"].Equals("") ? ROOT_FOLDER : map["stockFile"];
        //public static string AUTO_DOWNLOAD_FILE = map["autoDownloadFile"];
        public static string ANALYZE_FILE = map["analyzeFile"].Equals("") ? ROOT_FOLDER + @"\analyze.txt" : map["analyzeFile"];
        public static string UPDATE_FILE = map["updateFile"].Equals("") ? ROOT_FOLDER + @"\{0}\update.txt" : map["updateFile"];
        public static string DOWNLOAD_ALL = map["downloadAll"].Equals("") ? "0" : map["downloadAll"];
        public static string ANALYZE_START_DATE = map["analyzeStartDate"].Equals("") ? "2012-09-01" : map["analyzeStartDate"];
        public static string ANALYZE_CHART_DIR = map["analyeChartDir"].Equals("") ? ROOT_FOLDER : map["analyeChartDir"];
        public static bool CLEAN = Boolean.Parse(map["clean"]);

        public static string MYSQL_STRING = map["mysqlstring"];
        public static bool DOWNLOAD_AND_INSERT = Boolean.Parse(map["downloadAndInsert"]);

        public static int DOWNLOAD_THREAD_NUMBER = map["downloadthreadnum"].Equals("") ? 2 :Int32.Parse(map["downloadthreadnum"]);
        //default start date and update time
        public static DateTime DEFAULT_LASTUPDATE = new DateTime(2014, 1, 1);


        public static int TOP = map["top"].Equals("") ? 50 : Int32.Parse(map["top"]);
        public static bool ONLY_TOP =  map["onlytop"].Equals("")?true: Boolean.Parse(map["onlytop"]);

        public static int QUERY_TOP = map["querytop"].Equals("")?50: int.Parse(map["querytop"]);

        //分析提前天数
        public static int DAYS_BEFORE = map["daysbefore"].Equals("") ? 30: int.Parse(map["daysbefore"]);

        public static int ANALYZE_LEVEL = 0;

        public static string ANALYZE_TIME = map["analyzetime"].Equals("") ? "24-12-6-3" : map["analyzetime"];

        public static int MIN_ANALYZE_VALUE = -999999;

        public static int RETRY = 3;

      
    }
}

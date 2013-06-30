﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using SotckAnalyzer.data;
using SotckAnalyzer.analyzer;
using SotckAnalyzer.util;

namespace SotckAnalyzer
{
    static class Program
    {


        static String connectionString = "server=127.0.0.1;user id=root; password=as; database=albertsong; pooling=false;charset=utf8";
        //static string Constant.ROOT_FOLDER = Constant.ROOT_FOLDER;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            //generateAll();
            //queryDailyData();

            //
            //MySqlConnection conn = MySQL.GetConnection();
            //conn.Open();
            //foreach (DirectoryInfo dir in new DirectoryInfo(@"D:\project\stock\data").GetDirectories())
            //{
            //    foreach (FileInfo fi in dir.GetFiles())
            //    {
            //        Console.WriteLine(fi.FullName);
            //        List<TradeRecord> l = ExcelEngine.ReadData(fi.FullName);
            //        MySQL.InsertDailyRecord(conn, l);
            //    }
            //}

            //MySQL mysql = new MySQL();


            //conn.Close();

            // conectMySQL();

            // DataDownload.downloadDataToCsv("sh600038", "2013-05-20", "2013-06-28");
            //DailyDataSet set = CsvAnalyzer.ReadCsv(@"D:\project\stock\data\sh600038\sh600038-2013-05-20.csv");
            //FilterData fd = new FilterData(set, "<6");
            //fd.Analye();
            //Console.WriteLine(fd.setByBigDeal.Count);

            List<DailyDataSet> dds = CsvAnalyzer.ReadCsv("sh600038", "2013-05-20", "2013-06-28");
            Console.WriteLine("date,bigBuyShare,bigSellShare,toalShare,bigBuyMoney,bigSellMoney,toalMoney,Open,Close,Average,Hightest,WhenHighest,Lowest,WhenLowest");
            foreach (DailyDataSet ds in dds)
            {

                FilterData fd = new FilterData(ds, ">500");
                fd.Analye();
                Console.Write(StockUtil.FormatDate(fd.set.date)+",");
                Console.Write(fd.TotalBuyShareByBigDeal + ",");
                Console.Write(fd.TotalSellShareByBigDeal + ",");
                Console.Write(fd.set.TotalShare + ",");
                Console.Write(fd.TotalBuyMoneyByBigDeal + ",");
                Console.Write(fd.TotalSellMoneyByBigDeal + ",");
                Console.Write(fd.set.TotalMoney + ",");
                Console.Write(fd.set.OpenPrice + ",");
                Console.Write(fd.set.ClosePrice + ",");
                Console.Write(fd.set.Average + ",");
                Console.Write(fd.set.HighestPrice + ",");
                Console.Write(StockUtil.FormatTime(fd.set.TimeWhenHighest) + ",");
                Console.Write(fd.set.LowestPrice + ",");
                Console.Write(StockUtil.FormatTime(fd.set.TimeWhenLowest) + ",");
                Console.WriteLine();

            }
        }

        public static void conectMySQL()
        {
            string connStr = "server=localhost;user=root;database=test;port=3306;password=Password1;";

            MySqlConnection conn = new MySqlConnection(connStr);

            MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand();

            try
            {
                conn.Open();
                cmd.Connection = conn;

                cmd.CommandText = "insert into test(id,data) values(?id,?data1)";


                cmd.Parameters.AddWithValue("?id", "1");
                cmd.Parameters.AddWithValue("?data1", "0.01");

                for (int i = 0; i < 10; i++)
                {

                    cmd.Parameters["?id"].Value = i;
                    cmd.Parameters["?data1"].Value = i + 0.01;
                    cmd.ExecuteNonQuery();
                }

                conn.Close();
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.WriteLine("nDone.");
        }

        public static void queryDailyData()
        {
            WebClient client = new WebClient();
            byte[] data = client.DownloadData("http://hq.sinajs.cn/list=sh601006");

            string[] dataArray = System.Text.Encoding.GetEncoding(936).GetString(data).Replace('"', ',').Split(',');
            //dataArray[0]=
            String[] retData = new String[32];
            for (int i = 0; i < 32; i++)
                retData[i] = dataArray[i + 1];
            Console.WriteLine(retData[0]);

            //return retData;
        }





    }
}

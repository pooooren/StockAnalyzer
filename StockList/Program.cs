using big;
using big.entity;
using Rest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Import;
using Common;
using Analyze;
using System.Data;

namespace StockList
{
    public class Program
    {
        public static void Main(string[] args)
        {

            //List<AnalyzeData> list = BizApi.QueryAnalyzeDataByDate("sh601899", new DateTime(2015, 12, 12), DateTime.Now,500,12);
            //Console.WriteLine(list.Count);
            List<AnalyzeData> list = BizApi.QueryAnalyzeDataById("sh601899", 3, 500, 12);
            Console.WriteLine(list.Count);
            //BizApi.ComputeAll(1, new DateTime(2015, 1, 1), DateTime.Now);
            //string sql = "select * from basicinfo";
            //DataSet ds = MySqlHelper.GetDataSet(sql);
            //Console.WriteLine(ds.Tables[0].Rows.Count);
            //AutoDownload.AutoDownload.DownloadSingle(new InfoData() { sid = "sz002792" });
            //List<InfoData> list = BizApi.QueryInfoByIndustry2("金融服务","银行");

            //BizApi.InsertAnalyzeData(new DateTime(2014,1,1),DateTime.Now);
            //List<InfoData> list = BizApi.QueryRzrq();
            ////decimal[] list = BizApi.QueryExtractList("sh600000");
            //List<AnalyzeData> list1 = BizApi.QueryAnalyzeData(DateTime.Now);


            //StockResource sr = new StockResource();
            //DateTime end=new DateTime(2015,3,12);
            //DateTime start=end.AddMonths(-1);
            //List<AnalyzeData> list1 = BizApi.ComputeAll(1,start,end);
            //foreach (AnalyzeData ad in list1)
            //    Console.WriteLine(ad.sid + "," + ad.value + "," + ad.name + "," + ad.firstlevel + "," + ad.secondlevel);
            //BizApi.InsertInfoExt(new InfoData() { sid = "sh600000" });

            //InfoExtData ifd = BizApi.QueryInfoExtById("sh600000");

            //AnalyzeData ad = BizApi.ComputeSingle3("sh600000", 1, 1000, new DateTime(2015, 1, 1), new DateTime(2015, 3, 1));
            //BizApi.ComputeAll_3(int.Parse(args[0]), int.Parse(args[1]));
            //Console.WriteLine(BizApi.QueryLatestPrice("sh601288","20150420"));

            //GenerateHtml.Generate.GenerateSingle(@"G:\github\StockAnalyzer\web\", "20150422",new string[] { "12","6","3","1" }, new string[] { "XPG" }, new string[] { "金融服务"}, new string[] {"上海市" });
            //Console.WriteLine(BizApi.QueryMaxMinPriceByRange("sh600272",24));

            //List<BasicData> list = ImportRawData.ReadCsvFile(@"D:\stock\store\data\sh600687\sh600687_2015-01-23.csv", "sh600687", new decimal[] { 1000,3000 }, DateTime.MinValue);

            //List<LineData> list = BizApi.QueryLineByLimit("sh600687", 30);


            //Console.WriteLine(MyBase64.CompressNumber(999999999999999999L));
            //Console.WriteLine(MyBase64.UnCompressNumber("I0OgMJB"));
            //Console.WriteLine(MyBase64.UnCompressNumber("z8TlHFk"));
            //string price=BizApi.QueryLatestPrice("sz002490", "20160326");

            //InfoExt.Program.QueryInfoExtFromEastMoney("sh600832");
            //Console.WriteLine(price);
            //List<InfoData> list = BizApi.QueryInfoAll();
            //foreach (InfoData id in list)
            //{
            //    try
            //    {
            //        BizApi.AddBigDetail(id.sid);
            //        Console.WriteLine(id.sid + " succeed to add bigdetail");
            //    }
            //    catch
            //    {
            //        Console.WriteLine(id.sid + " already to add bigdetail--------");
            //    }
            //}

            //GenerateHtml.GenerateAll("", "");
        }
    }

 
  
}

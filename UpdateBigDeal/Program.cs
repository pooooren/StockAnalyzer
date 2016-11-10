using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using big;
using big.entity;
using Common;
using LumenWorks.Framework.IO.Csv;
using System.Data;

namespace UpdateBigDeal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // DateTime start = new DateTime(2014, 3, 21);
            //DateTime end = new DateTime(2014, 3, 21);

            //DownloadData("sh600000", start.ToString("yyyy-MM-dd"), end.ToString("yyyy-MM-dd"));

            //SplitAnalyzeData();
            List<InfoData> list = BizApi.QueryInfoAll();
            //List<InfoData> list = new List<InfoData>() { BizApi.QueryInfoById("sh600157") };
            foreach (InfoData id in list)
            {
                string delete_sql = String.Format(
                    "ALTER TABLE {0} ADD COLUMN `sellmoney` DECIMAL NULL COMMENT '' AFTER `sellshare`,ADD COLUMN `buymoney` DECIMAL NULL COMMENT '' AFTER `buyshare`; ", id.sid);
                MySqlHelper.ExecuteNonQuery(delete_sql);
                Console.WriteLine(id.sid);
            }

            
        }

        public static string DownloadData(string stock, string startDate, string endDate)
        {
            for (DateTime dateTime = DateTime.Parse(startDate);
                 dateTime <= DateTime.Parse(endDate);
                 dateTime += TimeSpan.FromDays(1))
            {
                if (!(dateTime.Date.DayOfWeek == DayOfWeek.Saturday || dateTime.Date.DayOfWeek == DayOfWeek.Sunday))
                    //LOG.Info(toDate(dateTime.Date));

                    DownloadDataToCsvByReader(stock, StockUtil.FormatDate(dateTime.Date));
            }

            return stock;
        }


        public static String DownloadDataToCsvByReader(string stock, string date)
        {
            try
            {
                WebClient client = new WebClient();
                string url = "http://market.finance.sina.com.cn/downxls.php?date=" + date + "&symbol=" + stock;
                string data = System.Text.Encoding.GetEncoding(936).GetString(client.DownloadData(url));
                data = ReplaceChinese(data);

                if (data != null && data.Length > 2048)
                {
                    TextReader stream = new StringReader(data);
                    ImportFileByReader(stock, date, stream);
                }
                else
                {
                    //StockLog.Log.Debug("None: " + stock + "-" + date);
                }
            }
            catch
            {
                StockLog.Log.Error("Error" + stock + "-" + date);
            }


            return stock + "-" + date;
        }

        public static void ImportFileByReader(string sid, string date, TextReader reader)
        {
            //string sid = file.Substring(file.LastIndexOf("\\") + 1);
            StockLog.Log.Debug(sid + " start ");
            DateTime beginning = DateTime.Now;
            //BizApi.CreateDataTable(sid);
            //try
            //{
                //decimal weight = BizApi.QueryWeight(sid);
                //decimal[] extractlist = BizApi.QueryExtractList(sid);
                decimal[] extractlist = new decimal[4];
                extractlist[0] = 0;
                extractlist[1] = 500;
                extractlist[2] = 1000;
                extractlist[3] = 2000;

                DateTime lastupdate = DateTime.Parse(Constant.ANALYZE_START_DATE);//BizApi.QueryExtractLastUpdate(sid);

                List<BasicData> list = ReadCsvByReader(sid, date, reader, extractlist, lastupdate);

                foreach (BasicData bd in list)
                {
                    if (bd.big == 0)
                    {
                        try
                        {
                            BizApi.InsertBasicData(bd);
                        }
                        catch
                        {
                            StockLog.Log.Error(sid + " " + bd.time + " insert fail");
                        }
                        //Console.WriteLine(sid + " insert " + bd.time + " " + bd.big);
                    }
                    else
                    {
                        //Console.WriteLine(sid + " update " + bd.time + " " + bd.big);

                        try
                        {
                            BizApi.UpdateBasicDataForBigDeal(bd);
                        }
                        catch
                        {
                            StockLog.Log.Error(sid +" "+ bd.time + " update fail");
                        }
                    }
                }
                TimeSpan end = DateTime.Now - beginning;

                //StockLog.Log.Debug(sid + " complete at " + end);
            //}
            //catch
            //{
            //    StockLog.Log.Error(sid + " import fail");
            //}

        }

        public static List<BasicData> ReadCsvByReader(string sid, string date, TextReader stream, decimal[] bigs, DateTime lastupdate)
        {
            int index = 0;

            //bigs = null;
            //bigs = new decimal[1];
            //bigs[0] = 1000M;
            List<BasicData> list = new List<BasicData>();
            BasicData[] array = new BasicData[bigs.Length];

            if (date.Length != 10) throw new Exception("time format is wrong -" + date);
            string time = date;
            DateTime t = new DateTime(Int32.Parse(time.Substring(0, 4)), Int32.Parse(time.Substring(5, 2)), Int32.Parse(time.Substring(8, 2)));

            //处理过了就忽略
            if (t < lastupdate.AddDays(1)) return null;

            Dictionary<decimal, string> temp = new Dictionary<decimal, string>();

            for (int j = 0; j < bigs.Length; j++)
            {
                array[j] = new BasicData();
                array[j].big = (int)bigs[j];
                array[j].time = t;
                array[j].sid = sid;
                array[j].c_type = "d";
                temp[bigs[j]] = "";
            }

            // open the file "data.csv" which is a CSV file with headers
            using (CsvReader csv = new CsvReader(stream, true))
            {  //颠倒记录
                IEnumerable<string[]> hi = csv.Reverse<string[]>();

                decimal current;

                decimal open = 0, close = 0, high = 0, low = 0;


                foreach (String[] record in hi)
                {

                    string price_str = record[1];
                    string change_str = record[2];
                    string share_str = record[3];
                    string type_str = record[5];
                    string time_str = record[0];
                    try
                    {

                        Decimal price = Decimal.Parse(price_str.IndexOf("\0") > 0 ? price_str.Remove(price_str.IndexOf("\0")) : price_str);
                        Double share = Double.Parse(share_str.IndexOf("\0") > 0 ? share_str.Remove(share_str.IndexOf("\0")) : share_str);

                        //count close,open,low,high
                        current = price;
                        if (index == 0)
                        {
                            open = current;
                            high = current;
                            low = current;
                        }
                        if (high < current) high = current;
                        if (low > current) low = current;
                        close = current;
                        index++;

                        for (int k = 0; k < bigs.Length; k++)
                        {

                            int fieldCount = csv.FieldCount;
                            string[] headers = csv.GetFieldHeaders();

                            array[k].open = open;
                            array[k].close = close;
                            array[k].high = high;
                            array[k].low = low;
                            array[k].totalshare += share;
                            array[k].totalmoney += Decimal.Multiply(price, (Decimal)share);

                            if (Int32.Parse(share_str) >= bigs[k])
                            {
                                //15位编码编码，
                                //第1位买单1卖单0
                                //后3位股数（百手）
                                //后3位时间数（相对于9：25的分钟数）
                                //后4位位股价（1正0负，后三位是相对开盘的万分比
                                //后4位，股价变化(1正0负，后3位是万分比）
                                //Console.WriteLine(time_str + ": " + share + " " + type_str);

                                string code = "";
                                if (type_str == "S")
                                {
                                    //string code = "-" + MyBase64.CompressNumber(long.Parse(p(share) + p1(time_str) + p2(current, open)));
                                    if (bigs[k] > 0)
                                    {
                                        code = p(share) + p1(time_str) + RateToOpen(current, open) + RateToBefore(decimal.Parse(change_str), current);
                                        code = MyBase64.CompressNumber(long.Parse(code));
                                        code = "-" + code;
                                        temp[bigs[k]] += code;
                                    }
                                    array[k].sellshare += share;
                                    array[k].sellmoney += Decimal.Multiply(price, (Decimal)share);
                                }
                                if (type_str == "B")
                                {
                                    if (bigs[k] > 0)
                                    {
                                        //string code = "-" + MyBase64.CompressNumber(long.Parse(p(share) + p1(time_str) + p2(current, open)));
                                        code = p(share) + p1(time_str) + RateToOpen(current, open) + RateToBefore(decimal.Parse(change_str), current);
                                        //Console.WriteLine(code);
                                        code = MyBase64.CompressNumber(long.Parse(code));
                                        code = "+" + code;
                                        temp[bigs[k]] += code;
                                    }
                                    array[k].buyshare += share;
                                    array[k].buymoney += Decimal.Multiply(price, (Decimal)share);
                                }

                                array[k].bigdetail = temp[bigs[k]];

                                //Console.WriteLine(array[k].bigdetail);
                            }
                        }
                    }
                    catch
                    {
                        StockLog.Log.Error(sid + " import fail");
                    }
                }
            }
            for (int j = 0; j < bigs.Length; j++)
            {
                list.Add(array[j]);
            }
            return list;
        }



        public static void SplitAnalyzeData()
        {
            List<AnalyzeData> list = QueryAnalyzeData();
            foreach(AnalyzeData ad in list)
            {
                BizApi.InsertAnalyzeData(ad, "analyzedata_" + ad.month + "_" + ad.big);
                Console.WriteLine("{0} {1} {2} {3}", ad.sid, ad.tag, ad.big, ad.month);
            }
        }




        public static List<AnalyzeData> QueryAnalyzeData()
        {
            List<AnalyzeData> list = new List<AnalyzeData>();

            string sql = string.Format("select  sid , tag , name , firstlevel , secondlevel , value , enddate , startdate , rank , big , level , month from analyzedata order by tag");

            DataSet ds = MySqlHelper.GetDataSet(sql);
            if (ds == null) return list;
            DataTable dt = ds.Tables[0];

            foreach (DataRow dr in dt.Rows)
            {
                AnalyzeData bd = new AnalyzeData()
                {
                    sid = dr["sid"].ToString(),
                    tag = dr["tag"].ToString(),
                    name = dr["name"].ToString(),
                    firstlevel = dr["firstlevel"].ToString(),
                    secondlevel = dr["secondlevel"].ToString(),
                    enddate = BizCommon.ParseToString(DateTime.Parse(dr["enddate"].ToString())),
                    //lastupdate = dr["lastupdate"].ToString(),
                    value = Decimal.Parse(dr["value"].ToString()),
                    rank = Int32.Parse(dr["rank"].ToString()),
                    //startdate = DateTime.Parse(dr["startdate"].ToString()),
                    startdate = BizCommon.ParseToString(DateTime.Parse(dr["startdate"].ToString())),
                    month = Int32.Parse(dr["month"].ToString()),

                    level = Int32.Parse(dr["level"].ToString()),
                    big = Int32.Parse(dr["big"].ToString())
                };

                list.Add(bd);
            }

            return list;
        }
        public static List<BigData> Parse(string sid, string time, string bigdeal)
        {
            List<BigData> list = new List<BigData>();

            bigdeal = bigdeal.Replace("-", ",-").Replace("+", ",+");

            bigdeal = bigdeal.StartsWith(",") ? bigdeal.Substring(1, bigdeal.Length - 1) : bigdeal;
            string[] bigs = bigdeal.Split(',');
            foreach (string b in bigs)
            {
                string type = b.StartsWith("+") ? "B" : "S";

                string code = MyBase64.UnCompressNumber(b.Substring(1, b.Length - 1)).ToString();

                for (int j = 0; j < (14 - code.Length); j++)
                {
                    code = "0" + code;
                }

                list.Add(
                    new BigData()
                    {
                        sid = sid,
                        shares = int.Parse(code.Substring(0, 3)),
                        minutes = int.Parse(code.Substring(3, 3)),
                        rateToOpen = int.Parse(code.Substring(6, 4).StartsWith("0") ? "-" + code.Substring(7, 3) : code.Substring(7, 3)),
                        rateToChange = int.Parse(code.Substring(10, 4).StartsWith("0") ? "-" + code.Substring(11, 3) : code.Substring(11, 3)),
                        time = BizCommon.ParseToDate(time),
                        type = type
                    });


            }

            return list;

        }
        /// <summary>
        /// 股票变化. 万分比，四位，1为正，0是负
        /// </summary>
        /// <param name="change"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        public static string RateToBefore(decimal change, decimal current)
        {
            //集合竞价
            if (current == change) return "9999";

            int rate = (int)Math.Floor(change / (current - change) * 10000);

            //一单涨停，设为999；
            if (rate >= 1000) rate = 999;
            if (rate <= -1000) rate = -999;

            string ret = "";
            if (rate > 0)
            {
                if (rate < 10) ret = "100" + rate;
                if (rate > 10 && rate < 100) ret = "10" + rate;
                if (rate > 99 && rate < 1000) ret = "1" + rate;
            }
            else
            {

                if (-rate < 10) ret = "000" + (-rate);
                if (-rate > 9 && -rate < 99) ret = "00" + (-rate);
                if (-rate > 99 && -rate < 1000) ret = "0" + (-rate);
            }

            return ret;
        }

        /// <summary>
        /// 股数编码，3位，百手
        /// </summary>
        /// <param name="bb"></param>
        /// <returns></returns>
        public static string p(double bb)
        {
            int aa = (int)Math.Floor(bb / 100);

            if (aa < 10) return "00" + aa.ToString();
            else if (aa < 100) return "0" + aa.ToString();
            else return aa.ToString();

        }

        /// <summary>
        /// 处理股价百分比，4位第一位是1正，0负，后面三位相对于开盘价的千分比
        /// </summary>
        /// <param name="open"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        public static string RateToOpen(decimal current, decimal open)
        {
            decimal cc = (current - open) / open * 10000;

            int rate = (int)Math.Floor(cc);
            //涨停，设为999；
            if (rate >= 1000) rate = 999;
            if (rate <= -1000) rate = -999;

            string ret = "";
            if (rate > 0)
            {
                if (rate < 10) ret = "100" + rate;
                if (rate > 9 && rate < 100) ret = "10" + rate;
                if (rate > 99 && rate < 1000) ret = "1" + rate;
            }
            else
            {

                if (-rate < 10) ret = "000" + (-rate);
                if (-rate > 10 && -rate < 99) ret = "00" + (-rate);
                if (-rate > 99 && -rate < 1000) ret = "0" + (-rate);
            }
            return ret;
        }

        /// <summary>
        /// 处理时间，也是3位，相对于9：25分的分钟数
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string p1(string time)
        {
            TimeSpan ts = TimeSpan.Parse(time);
            TimeSpan start = new TimeSpan(9, 25, 00);
            TimeSpan noon = new TimeSpan(11, 30, 01);
            TimeSpan noon1 = new TimeSpan(13, 00, 00);
            TimeSpan end = new TimeSpan(15, 00, 01);
            if (ts < noon)
            {
                double a = (ts - start).TotalMinutes;
                return p(a * 100);
            }
            else
            {
                double a1 = (ts - noon1).TotalMinutes + 125;
                return p(a1 * 100);
            }
        }

        /// <summary>
        /// 处理中文字符，减少文件大小
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ReplaceChinese(string data)
        {
            data = data.Replace('\t', ',');
            data = data.Replace("买盘", "B");
            data = data.Replace("卖盘", "S");
            data = data.Replace("中性盘", "Z");
            data = data.Replace("--", "0");
            data = data.Replace("成交时间", "time");
            data = data.Replace("成交价", "price");
            data = data.Replace("价格变动", "change");
            data = data.Replace("成交量(手)", "share");
            data = data.Replace("成交额(元)", "money");
            data = data.Replace("性质", "type");
            return data;
        }
    }
}

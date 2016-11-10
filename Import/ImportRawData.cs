using big;
using big.entity;
using Common;
using LumenWorks.Framework.IO.Csv;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Import
{
    //http://market.finance.sina.com.cn/downxls.php?date=2011-07-08&symbol=sh600900


    public class ImportRawData
    {
        public static object Contstant { get; private set; }

        public static void ImportFileByReader(string sid, string date, TextReader reader)
        {
            //string sid = file.Substring(file.LastIndexOf("\\") + 1);
            StockLog.Log.Debug(sid + " start ");
            DateTime beginning = DateTime.Now;
            //BizApi.CreateDataTable(sid);
            //try
            //{
            //decimal weight = BizApi.QueryWeight(sid);
            decimal[] ext_list = BizApi.QueryExtractList(sid);
            decimal[] extractlist = new decimal[ext_list.Length + 1];

            for (int i = 0; i < ext_list.Length; i++)
            {
                extractlist[i] = ext_list[i];
            }
            extractlist[ext_list.Length] = 0;
            //decimal[] extractlist = new decimal[4];
            //extractlist[0] = 0;
            //extractlist[1] = 500;
            //extractlist[2] = 1000;
            //extractlist[3] = 2000;
            try
            {
                if (sid == "sh600048")
                {
                    StockLog.Log.Debug(sid + " complete at ");
                }
                DateTime lastupdate = BizApi.QueryExtractLastUpdate(sid);

                List<BasicData> list = ReadCsvByReader(sid, date, reader, extractlist, lastupdate);

                foreach (BasicData bd in list)
                {

                    BizApi.InsertBasicData(bd);
                }
                TimeSpan end = DateTime.Now - beginning;

                StockLog.Log.Debug(sid + " complete at " + end);
            }
            catch
            {
                StockLog.Log.Error(sid + " import fail");
            }

        }
        public static void ImportFileByReaderOld(string sid, string date, TextReader reader)
        {
            //string sid = file.Substring(file.LastIndexOf("\\") + 1);
            StockLog.Log.Debug(sid + " start ");
            DateTime beginning = DateTime.Now;
            BizApi.CreateDataTable(sid);
            string file = sid + "_" + date;
            try
            {
                //decimal weight = BizApi.QueryWeight(sid);
                decimal[] extractlist = BizApi.QueryExtractList(sid);
                DateTime lastupdate = BizApi.QueryExtractLastUpdate(sid);

                List<BasicData> list = ReadCsvByReader(sid, date, reader, extractlist, lastupdate);

                foreach (BasicData bd in list)
                {
                    //Console.WriteLine(bd.time+" "+bd.sellshare);
                    BizApi.InsertBasicData(bd);
                }
                TimeSpan end = DateTime.Now - beginning;

                StockLog.Log.Debug(file + " complete at " + end);
            }
            catch
            {
                StockLog.Log.Error(file + " import fail");
            }

        }


        public static List<BasicData> ReadCsvByReaderOld(string sid, string date, TextReader stream, decimal[] bigs, DateTime lastupdate)
        {
            int index = 0;

            List<BasicData> list = new List<BasicData>();
            BasicData[] array = new BasicData[bigs.Length];

            if (date.Length != 10) throw new Exception("time format is wrong -" + date);
            string time = date;
            DateTime t = new DateTime(Int32.Parse(time.Substring(0, 4)), Int32.Parse(time.Substring(5, 2)), Int32.Parse(time.Substring(8, 2)));

            //处理过了就忽略
            if (t < lastupdate.AddDays(1)) return null;

            for (int j = 0; j < bigs.Length; j++)
            {
                array[j] = new BasicData();
                array[j].big = (int)bigs[j];
                array[j].time = t;
                array[j].sid = sid;
                array[j].c_type = "d";
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
                    string share_str = record[3];
                    string type_str = record[5];
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

                                if (type_str == "S")
                                {
                                    array[k].sellshare += share;
                                    array[k].sellmoney += Decimal.Multiply(price, (Decimal)share);
                                }
                                if (type_str == "B")
                                {
                                    array[k].buyshare += share;
                                    array[k].buymoney += Decimal.Multiply(price, (Decimal)share);
                                }
                            }
                        }
                    }
                    catch
                    {
                        StockLog.Log.Error(sid + "_" + date + " import fail");
                    }
                }
            }
            for (int j = 0; j < bigs.Length; j++)
            {
                list.Add(array[j]);
            }
            return list;
        }
        /// <summary>
        /// 保存大单的具体情况
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="date"></param>
        /// <param name="stream"></param>
        /// <param name="bigs"></param>
        /// <param name="lastupdate"></param>
        /// <returns></returns>
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
                                    if (Common.Constant.DETAIL) { if (bigs[k] > 0)
                                        {
                                            code = p(share) + p1(time_str) + RateToOpen(current, open) + RateToBefore(decimal.Parse(change_str), current);
                                            code = MyBase64.CompressNumber(long.Parse(code));
                                            code = "-" + code;
                                            temp[bigs[k]] += code;
                                        }
                                    }
                                    array[k].sellshare += share;
                                    array[k].sellmoney += Decimal.Multiply(price, (Decimal)share);
                                }
                                if (type_str == "B")
                                {
                                    if(Common.Constant.DETAIL) {
                                        if (bigs[k] > 0)
                                        {
                                            //string code = "-" + MyBase64.CompressNumber(long.Parse(p(share) + p1(time_str) + p2(current, open)));
                                            code = p(share) + p1(time_str) + RateToOpen(current, open) + RateToBefore(decimal.Parse(change_str), current);
                                            //Console.WriteLine(code);
                                            code = MyBase64.CompressNumber(long.Parse(code));
                                            code = "+" + code;
                                            temp[bigs[k]] += code;
                                        }
                                    }
                                    array[k].buyshare += share;
                                    array[k].buymoney += Decimal.Multiply(price, (Decimal)share);
                                }
                                if (Common.Constant.DETAIL)
                                {
                                    array[k].detail = temp[bigs[k]];
                                }

                                //Console.WriteLine(array[k].detail);
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

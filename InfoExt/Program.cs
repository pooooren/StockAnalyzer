using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using big;
using big.entity;
using Common;
using HtmlAgilityPack;

namespace InfoExt
{
    public class Program
    {
        public static void Main(string[] args)
        {
            List<InfoData> iflist = BizApi.QueryInfoAll();
            foreach (InfoData id in iflist)
            {
                InfoExtData ifd = QueryInfoExtFromEastMoney(id.sid);
                BizApi.InsertInfoExt(ifd);
                //Console.WriteLine(ifd.sid + " infoext inserted.");
            }

            //InfoExtData ifd = QueryInfoExtFromEastMoney("sh600339");
            
        }

        public static InfoExtData QueryInfoExtFromEastMoney(string sid)
        {
            InfoExtData ifd = new InfoExtData();
            ifd.sid = sid;
            //Console.WriteLine("start");
            try
            {

                string url = string.Format("http://quote.eastmoney.com/{0}.html", sid);
                WebClient client = new WebClient();
                byte[] gg = client.DownloadData(url);
                string haha = System.Text.Encoding.GetEncoding("gb2312").GetString(gg);
                TextReader stream = new StringReader(haha);
                HtmlDocument document = new HtmlDocument();
                document.Load(stream);

                HtmlNode rootNode = document.DocumentNode;
                HtmlNodeCollection c = rootNode.SelectNodes("//table[@id='rtp2']");

                //处理html
                string innerString = c[0].InnerText;
                innerString = innerString.Replace(System.Environment.NewLine, "");
                innerString = innerString.Replace("：", ":");
                innerString = System.Text.RegularExpressions.Regex.Replace(innerString, @"\s{1,}", " ", RegexOptions.IgnoreCase);
                //innerString = innerString.Replace("： ", ":");

                bool shourutongbi = false;
                bool jingliruntongbi = false;
                string[] array = innerString.Trim().Split(' ');
                foreach (string item in array)
                {
                    if (item.Trim().Length >-1 && item.IndexOf(":") >-1)
                    {
                        string[] bb = item.Split(':');

                       if(bb[0].IndexOf("收益")>-1){
                           ifd.shouyi = decimal.Parse(bb[1]);
                       }
                       if (bb[0].IndexOf("PE") >-1)
                       {
                           ifd.shiyinglv = decimal.Parse(bb[1].Trim().Equals("-") ? "0" : bb[1].Trim());
                       }
                       if (bb[0].IndexOf("净资产") >-1)
                       {
                           ifd.jingzichan = decimal.Parse(bb[1]);
                       }
                       if (bb[0].IndexOf("市净率") >-1)
                       {
                           ifd.shijinglv = decimal.Parse(bb[1]);
                       }
                       if (bb[0].IndexOf("收入") >-1)
                       {
                           ifd.shouru = decimal.Parse(bb[1].Substring(0, bb[1].Length-1));
                           shourutongbi = true;
                       }
                       if (bb[0].IndexOf("净利润") >-1)
                       {
                           ifd.jinglirun = decimal.Parse(bb[1].Substring(0, bb[1].Length - 1));
                           jingliruntongbi = true;
                       }
                       if (bb[0].IndexOf("同比") >-1)
                       {
                           if (shourutongbi)
                           {
                               ifd.shourutongbi = decimal.Parse(bb[1].Substring(0, bb[1].Length - 1));
                               shourutongbi = false;
                           }
                           if (jingliruntongbi)
                           {
                               ifd.jingliruntongbi = decimal.Parse(bb[1].Substring(0, bb[1].Length - 1));
                               jingliruntongbi = false;
                           }

                       }
                       if (bb[0].IndexOf("毛利率") >-1)
                       {
                           ifd.maolilv = decimal.Parse(bb[1].Substring(0, bb[1].Length - 1));
                       }
                       if (bb[0].IndexOf("净利率") >-1)
                       {
                           ifd.jinglilv = decimal.Parse(bb[1].Substring(0, bb[1].Length - 1));
                       }
                       if (bb[0].IndexOf("ROE") >-1)
                       {
                           ifd.ROE = decimal.Parse(bb[1].Substring(0, bb[1].Length - 1));
                       }
                       if (bb[0].IndexOf("负债率") >-1)
                       {
                           ifd.fuzhailv = decimal.Parse(bb[1].Substring(0, bb[1].Length - 1));
                       }
                       if (bb[0].IndexOf("总股本") >-1)
                       {
                           ifd.zongguben = decimal.Parse(bb[1].Substring(0, bb[1].Length - 1));
                       }
                       if (bb[0].IndexOf("总值") >-1)
                       {
                           ifd.zongzhi = decimal.Parse(bb[1].Substring(0, bb[1].Length - 1));
                       }
                       if (bb[0].IndexOf("流通股") >-1)
                       {
                           ifd.liutonggu = decimal.Parse(bb[1].Substring(0, bb[1].Length - 1));
                       }
                       if (bb[0].IndexOf("流值") >-1)
                       {
                           ifd.liuzhi = decimal.Parse(bb[1].Substring(0, bb[1].Length - 1));
                       }
                       if (bb[0].IndexOf("每股未分配利润") >-1)
                       {
                           ifd.meiguweifenpeilirun = decimal.Parse(bb[1].Substring(0, bb[1].Length - 1));
                       }
                       if (bb[0].IndexOf("上市时间") >-1)
                       {
                           ifd.shangshishijian = bb[1] ;
                       }
                    }
                }
            }
            catch
            {
                StockLog.Log.Error(sid + " infoext parse fail");
            }

            return ifd;
        }

        public static InfoExtData1 QueryInfoExtFromTonghuashun(string sid)
        {
            InfoExtData1 ifd = new InfoExtData1();
            ifd.sid = sid;
            //Console.WriteLine("start");
            try
            {

                string url = "http://stockpage.10jqka.com.cn/" + BizCommon.ProcessStockId(sid);
                WebClient client = new WebClient();
                byte[] gg = client.DownloadData(url);
                string haha = System.Text.Encoding.UTF8.GetString(gg);
                TextReader stream = new StringReader(haha);
                HtmlDocument document = new HtmlDocument();
                document.Load(stream);

                HtmlNode rootNode = document.DocumentNode;
                HtmlNodeCollection c = rootNode.SelectNodes("//dl[@class='company_details']");

                //处理html
                string innerString = c[0].InnerText;
                innerString = innerString.Replace(System.Environment.NewLine, "");
                innerString = innerString.Replace("\t", " ");
                innerString = System.Text.RegularExpressions.Regex.Replace(innerString, @"\s{1,}", " ", RegexOptions.IgnoreCase);
                innerString = innerString.Replace("： ", ":");

                string[] array = innerString.Split(' ');
                foreach (string item in array)
                {
                    if (item.Trim().Length >-1 && item.IndexOf(":") >-1)
                    {
                        string[] bb = item.Split(':');
                        switch (bb[0].Trim())
                        {
                            case "总股本": //亿股
                                ifd.zongguben = decimal.Parse(bb[1].Substring(0, bb[1].Length - 1));
                                break;
                            case "流通股"://亿股
                                ifd.liutonggu = decimal.Parse(bb[1].Substring(0, bb[1].Length - 1));
                                break;
                            case "营业收入增长率":
                                ifd.yingyeshouruzengzhanglv = decimal.Parse(bb[1].Substring(0, bb[1].Length - 1));
                                break;
                            case "营业收入": //亿元
                                ifd.yingyeshouru = decimal.Parse(bb[1].Substring(0, bb[1].Length - 2));
                                break;
                            case "净利润"://亿元
                                ifd.jinglirun = decimal.Parse(bb[1].Substring(0, bb[1].Length - 2));
                                break;
                            case "净利润增长率":
                                ifd.jinglirunzengzhanglv = decimal.Parse(bb[1].Substring(0, bb[1].Length - 1));
                                break;
                            case "每股收益"://元
                                ifd.meigujingzichan = decimal.Parse(bb[1].Substring(0, bb[1].Length - 1));
                                break;
                            case "每股净资产"://元
                                ifd.meigujingzichan = decimal.Parse(bb[1].Substring(0, bb[1].Length - 1));
                                break;
                            case "净资产收益率":
                                ifd.jingzichanshouyilv = decimal.Parse(bb[1].Substring(0, bb[1].Length - 1));
                                break;
                            case "每股现金流"://元
                                ifd.meiguxianjinliu = decimal.Parse(bb[1].Substring(0, bb[1].Length - 1));
                                break;
                            case "每股公积金"://元
                                ifd.meigugongjijin = decimal.Parse(bb[1].Substring(0, bb[1].Length - 1));
                                break;
                            case "每股未分配利润"://元
                                ifd.meiguweifenpeilirun = decimal.Parse(bb[1].Substring(0, bb[1].Length - 1));
                                break;
                        }
                    }
                }
            }
            catch
            {
                StockLog.Log.Error(sid + " infoext import fail");
            }

            return ifd;
        }

    }
}

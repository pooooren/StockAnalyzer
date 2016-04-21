using big;
using big.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using BizCommon = big.BizCommon;
using Common;
using System.ComponentModel;

namespace Rest
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Stock" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Stock.svc or Stock.svc.cs at the Solution Explorer and start debugging.
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class StockResource
    {
        [WebGet(UriTemplate = "test", ResponseFormat = WebMessageFormat.Json)]
        public string version()
        {
            return "1.0";
        }


        [WebGet(UriTemplate = "query/id/{id}?big={big}&type={type}&start={start}&end={end}", ResponseFormat = WebMessageFormat.Json)]
        public IList<BasicData> QueryData(string id, string big, string type, string start, string end)
        {
            DateTime endDate, startDate;
            IList<BasicData> list;
            if (string.IsNullOrEmpty(end)) endDate = DateTime.Now; else endDate = BizCommon.ParseToDate(end);
            if (string.IsNullOrEmpty(big)) big = "1000";
            if (string.IsNullOrEmpty(type)) type = "w";
            if (string.IsNullOrEmpty(start)) startDate = new DateTime(2014, 1, 1); else startDate = BizCommon.ParseToDate(start);

            switch (type)
            {
                case "m":
                    list = BizApi.QueryByMonth(id, Int32.Parse(big), startDate, endDate);
                    break;
                case "d":
                    list = BizApi.QueryByDay(id, Int32.Parse(big), startDate, endDate);
                    break;
                default:
                    list = BizApi.QueryByWeek(id, Int32.Parse(big), startDate, endDate);
                    break;
            }

            return list;
        }

        [WebGet(UriTemplate = "latestprice/id/{id}?tag={tag}", ResponseFormat = WebMessageFormat.Json)]
        public string QueryLatestPrice(string id,string tag)
        {
            
            return BizApi.QueryLatestPrice(id,tag);
        }

        [WebGet(UriTemplate = "maxmin/id/{id}?range={range}", ResponseFormat = WebMessageFormat.Json)]
        public string QueryMaxMin(string id,string range)
        {
            int r = 24;
            if (string.IsNullOrEmpty(range)) r = int.Parse(range);
            return BizApi.QueryMaxMinPriceByRange(id,r);
        }


        [WebGet(UriTemplate = "getData?date={date}&stockno={stockno}&type={type}", ResponseFormat = WebMessageFormat.Json)]
        public string QueryLineDataForQingyou(string date, string stockno, string type)
        {
            return BizApi.QueryStockDataForQingyou(BizCommon.ParseToDate(date), stockno, type);
        }

        [WebGet(UriTemplate = "line/id/{id}?type={type}&start={start}&end={end}", ResponseFormat = WebMessageFormat.Json)]
        public IList<LineData> QueryLineData(string id, string type, string start, string end)
        {
            DateTime endDate, startDate;
            IList<LineData> list;
            if (string.IsNullOrEmpty(end)) endDate = DateTime.Now; else endDate = BizCommon.ParseToDate(end);
            if (string.IsNullOrEmpty(type)) type = "d";
            if (string.IsNullOrEmpty(start)) startDate = new DateTime(2014, 1, 1); else startDate = BizCommon.ParseToDate(start);

            switch (type)
            {
                case "m":
                    list = BizApi.QueryLineByMonth(id, startDate, endDate);
                    break;
                case "d":
                    list = BizApi.QueryLineByDay(id, startDate, endDate);
                    break;
                case "w":
                    list = BizApi.QueryLineByWeek(id, startDate, endDate);
                    break;
                default:
                    list = BizApi.QueryLineByDay(id, startDate, endDate);
                    break;
            }

            return list;
        }

        [WebGet(UriTemplate = "infoext/id/{id}", ResponseFormat = WebMessageFormat.Json)]
        public InfoExtData QueryInfoExtById(string id)
        {
            return BizApi.QueryInfoExtById(id);
        }

        [WebGet(UriTemplate = "info/id/{id}", ResponseFormat = WebMessageFormat.Json)]
        public InfoData QueryInfoById(string id)
        {
            return BizApi.QueryInfoById(id);
        }

        [WebInvoke(UriTemplate = "info/all?type={type}", Method = "GET", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        //[WebGet(UriTemplate = "info/industry", ResponseFormat = WebMessageFormat.Json)]
        public string[] QueryInfoByAllIndustry(string type)
        {
            if (type.Equals("L"))
            {
                return BizApi.QueryAllLocation();
            }
            else
            {
                return BizApi.QueryAllIndustry();
            }

        }
        
        [WebGet(UriTemplate = "info/rzrq", ResponseFormat = WebMessageFormat.Json) ]
        [Description("融资融券股票列表")]
        public List<InfoData> QueryInfoByRzrq()
        {
            return BizApi.QueryInfoRzrq() ;
        }

       
        [WebGet(UriTemplate = "info/industry1/{industry1}", ResponseFormat = WebMessageFormat.Json)]
        [Description("按照行业查询股票")]
        public List<InfoData> QueryInfoByIndutry(string industry1)
        {
            return BizApi.QueryInfoByIndustry(industry1);
        }

        [Description("按照二级行业进行查询")]
        [WebGet(UriTemplate = "info/industry1/{industry1}/industry2/{industry2}", ResponseFormat = WebMessageFormat.Json)]
        public List<InfoData> QueryInfoByIndutry2(string industry1, string industry2)
        {
            return BizApi.QueryInfoByIndustry2(industry1, industry2);
        }

        [WebGet(UriTemplate = "info/location/{location}", ResponseFormat = WebMessageFormat.Json)]
        public List<InfoData> QueryInfoByLocation(string location)
        {
            return BizApi.QueryInfoByLocation(location);
        }

        [Description("查询股票的排名,sid-股票代码,level-0,tag-时间戳,old-提前几个月的数据，目前是24-12-6-3,daybefore-提前几天开始计算，默认是０")]
        [WebGet(UriTemplate = "analyzevalue?sid={sid}&level={level}&tag={tag}&old={old}&daybefore={daybefore}", ResponseFormat = WebMessageFormat.Json)]
        public string QueryAnalyze1(string sid,string level, string tag, string old,string daybefore)
        {
            tag = BizCommon.ProcessWeekend(tag);
            int level_val = 1;
            DateTime now = DateTime.Now;
            int i_daybeofre = 0;
            if (string.IsNullOrEmpty(tag)) tag = BizCommon.ParseToString(now); else now = BizCommon.ParseToDate(tag);
            if (string.IsNullOrEmpty(daybefore)) i_daybeofre =0; else i_daybeofre = int.Parse(daybefore);
            if (!string.IsNullOrEmpty(level)) level_val = Int32.Parse(level);
            if (string.IsNullOrEmpty(old)) old = Constant.ANALYZE_TIME;

            string vv = "";
            string[] list = old.Split('-');
            foreach (string v in list)
            {

                int o =-Int32.Parse(v);


                DateTime end_date = now.AddDays(-i_daybeofre);
                DateTime start_date = end_date.AddMonths(o);


                vv+= BizApi.QueryAnalyzeDataValue(sid, tag, o, level_val)+",";
            }

            return vv.Substring(0, vv.Length - 1); ;
        }
        [WebGet(UriTemplate = "analyzevalue1", ResponseFormat = WebMessageFormat.Json)]
        public string QueryAnalyze2()
        {
            return Constant.ANALYZE_TIME;
        }
        
        [WebGet(UriTemplate = "analyze?level={level}&tag={tag}&old={old}&daybefore={daybefore}&industry={industry}&location={location}&type={type}", ResponseFormat = WebMessageFormat.Json)]
        public List<AnalyzeData> QueryAnalyze(string level, string tag, string old, string daybefore,string industry,string location,string type)
        {
            tag = BizCommon.ProcessWeekend(tag);
            int level_val = 1;
            DateTime now = DateTime.Now;
            int days_before = Constant.DAYS_BEFORE;
            int o = -12;
            if (string.IsNullOrEmpty(tag)) tag = BizCommon.ParseToString(now); else now = BizCommon.ParseToDate(tag);
            if (!string.IsNullOrEmpty(daybefore)) days_before = Int32.Parse(daybefore);
            if (!string.IsNullOrEmpty(level)) level_val = Int32.Parse(level);
            if (!string.IsNullOrEmpty(old)) o = -Int32.Parse(old);

            DateTime end_date = now.AddDays((double)(-days_before));
            DateTime start_date = end_date.AddMonths(o);


            return BizApi.QueryAnalyzeData(tag, o, level_val, industry,location,type);
        }


        [WebGet(UriTemplate = "analyzebyId?sid={sid}&old={old}&big={big}&month={month}", ResponseFormat = WebMessageFormat.Json)]
        public List<AnalyzeData> QueryAnalyzeById(string sid,string old,string big,string month)
        {
            int o = string.IsNullOrEmpty(old) ? 6 :Int32.Parse(old);
            int m = string.IsNullOrEmpty(month) ? 12 : Int32.Parse(month);
            int b = string.IsNullOrEmpty(big) ? 500 :Int32.Parse(big);
            return BizApi.QueryAnalyzeDataById(sid, o,b,m);
        }
        [WebGet(UriTemplate = "analyzebyDate?sid={sid}&start={start}&end={end}&big={big}&month={month}", ResponseFormat = WebMessageFormat.Json)]
        public List<AnalyzeData> QueryAnalyzeByDate(string sid, string start,string end,string big,string month)
        {
            int b= string.IsNullOrEmpty(big) ? 500 :Int32.Parse(big);
            int m = string.IsNullOrEmpty(month) ? 12 : Int32.Parse(month);
            DateTime endDate = string.IsNullOrEmpty(end) ? DateTime.Now : BizCommon.ParseToDate(end);
            return BizApi.QueryAnalyzeDataByDate(sid, BizCommon.ParseToDate(start), endDate,b,m);
        }
        [WebGet(UriTemplate = "analyze1?level={level}&tag={tag}&old={old}&daybefore={daybefore}&industry={industry}&location={location}&type={type}", ResponseFormat = WebMessageFormat.Json)]
        [Description("type-CYB,ZB,SZ,SH")]
        public List<AnalyzeData> QueryAnalyzeRetry(string level, string tag, string old, string daybefore, string industry, string location, string type)
        {
            DateTime now = DateTime.Now;
            List<AnalyzeData> list = new List<AnalyzeData>();
            int retry = 0;
            while (true && retry < 10)
            {
                
                DateTime end_date = now.AddDays((double)(-Int32.Parse(daybefore)));
                DateTime start_date = end_date.AddMonths(-Int32.Parse(old));

                list=BizApi.QueryAnalyzeData(tag, Int32.Parse(old), Int32.Parse(level), industry, location, type);
                if (list.Count == 0)
                {
                    retry++;
                    now = now.AddDays(-retry);
                }
                else
                {
                    break;
                }
            }
            return list;
        }


        [WebGet(UriTemplate = "statistics?level={level}&tag={tag}&type={type}", ResponseFormat = WebMessageFormat.Json)]
        public List<AnalyzeData> QueryAnalyzeStatistics(string level, string tag, string type)
        {
            tag = BizCommon.ProcessWeekend(tag);
            int level_val = 1;
            //DateTime end_date = DateTime.Now;
            if (!string.IsNullOrEmpty(level)) level_val = Int32.Parse(level);
            if (!string.IsNullOrEmpty(tag)) tag = BizCommon.ParseToString(DateTime.Now);

            //if (string.IsNullOrEmpty(type))
            //    return BizApi.QueryAnalyzeStatisticsByName(tag, level_val);
            //if (type.Equals("name"))
            //    return BizApi.QueryAnalyzeStatisticsByName(tag, level_val);
            //if (type.Equals("industry"))
            //    return BizApi.QueryAnalyzeStatisticsByIndustry(tag, level_val);
            return null;
        }

        //[WebGet(UriTemplate = "analyze/date/{date}", ResponseFormat = WebMessageFormat.Json)]
        //public List<AnalyzeData> QueryAnalyze(string date)
        //{
        //        return BizApi.QueryAnalyzeData(Common.ParseToDate(date), 1);
        //}

        [WebGet(UriTemplate = "lastupdate/id/{id}", ResponseFormat = WebMessageFormat.Json)]
        public string QueryLastUpdate(string id)
        {
            return BizCommon.ParseToString(BizApi.QueryExtractLastUpdate(id));
        }

        
    }
}

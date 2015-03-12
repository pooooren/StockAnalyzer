﻿using big.entity;
using Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace big
{
    enum SQLState
    {
        Init = -1,
        Select = 0,
        Insert = 1,
        Update = 2,
        Delete = 3
    }
    public class BizApi
    {
        public static int create_table = -1;
        public static int extract_data = -1;
        public static int info = -1;
        public static string BASIC_TABLE = "basictemplate";
        public static string EXTRACT_TABLE_STATUS = "extractstatus";
        public static string CREATE_TABLE_STATUS = "createstatus";

        public static string INFO = "basicinfo";
        public static string INFOEXT = "basicinfoext";
        public static string ANALYZE="analyzedata";

        //public static DateTime DEFAULT_LASTUPDATE = new DateTime(2014, 1, 1);

        #region infoext
        public static void InsertInfoExt(InfoExtData ied)
        {
            string sql1 = String.Format("delete from {0} where sid='{1}' ", INFOEXT, ied.sid);
            MySqlHelper.ExecuteNonQuery(sql1);

            string sql = String.Format("insert into {0}values(sid,lastupdate,zongguben,liutonggu,yingyeshouruzengzhanglv,yingyeshouru,jinglirun,jinglirunzengzhanglv,meigushouyi,meigujingzichan,jingzichanshouyilv,meiguxianjinliu,meigugongjijin,meiguweifenpeilirun,shiyinglv,shijinglv)values('{1}','{2}',{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16}", INFOEXT, ied.sid, BizCommon.ParseToString(DateTime.Now), ied.zongguben, ied.liutonggu, ied.yingyeshouruzengzhanglv, ied.yingyeshouru, ied.jinglirun, ied.jinglirunzengzhanglv, ied.meigushouyi, ied.meigujingzichan, ied.jingzichanshouyilv, ied.meiguxianjinliu, ied.meigugongjijin, ied.meiguweifenpeilirun, ied.shiyinglv, ied.shijinglv);
            MySqlHelper.ExecuteNonQuery(sql);
        }
        #endregion
        #region analyze

        public static List<AnalyzeData> QueryAnalyzeStatisticsByName(DateTime selectDate,int level) {
          string sql = string.Format("select count(1) value,sum(rank) rank,name,sid,firstlevel,secondlevel from {0} where rank<{2} and lastupdate='{1}' and level={3} group by name having value >2 order by value desc,rank asc", ANALYZE, selectDate.ToString("yyyy-MM-dd"), Constant.TOP,level);
          DataSet ds = MySqlHelper.GetDataSet(sql);
          DataTable dt = ds.Tables[0];
          List<AnalyzeData> list = new List<AnalyzeData>();
          foreach (DataRow dr in dt.Rows)
          {
              AnalyzeData bd = new AnalyzeData()
              {
                  sid = dr["sid"].ToString(),
                  name = dr["name"].ToString(),
                  firstlevel = dr["firstlevel"].ToString(),
                  secondlevel = dr["secondlevel"].ToString(),
                  lastupdate = BizCommon.ParseToString(selectDate),
                  value = Decimal.Parse(dr["value"].ToString()),
                  rank = Int32.Parse(dr["rank"].ToString())
              };

              list.Add(bd);
          }

          return list;
        }

        public static List<AnalyzeData> QueryAnalyzeStatisticsByIndustry(DateTime selectDate, int level)
        {
            string sql = string.Format("select  count(1) value,firstlevel from {0} where rank<{2} and lastupdate='{1}' and level={3} group by firstlevel order by value desc  limit 10", ANALYZE, selectDate.ToString("yyyy-MM-dd"), Constant.TOP, level);
            DataSet ds = MySqlHelper.GetDataSet(sql);
            DataTable dt = ds.Tables[0];
            List<AnalyzeData> list = new List<AnalyzeData>();
            foreach (DataRow dr in dt.Rows)
            {
                AnalyzeData bd = new AnalyzeData()
                {
                    firstlevel = dr["firstlevel"].ToString(),
                    lastupdate = BizCommon.ParseToString(selectDate),
                    value = Decimal.Parse(dr["value"].ToString())
                };

                list.Add(bd);
            }

            return list;
        }

        public static List<AnalyzeData> QueryAnalyzeData(DateTime selectDate)
        {
            string sql = string.Format("select A.sid,A.name,A.value,A.firstlevel,A.secondlevel,A.lastupdate,A.rank,A.startdate from {0} A join {4} B on B.sid=A.sid and A.rank<{2} and A.lastupdate='{1}' and A.big=B.weight*1000 order by rank", ANALYZE, selectDate.ToString("yyyy-MM-dd"), Constant.TOP);
            DataSet ds = MySqlHelper.GetDataSet(sql);
            DataTable dt = ds.Tables[0];
            List<AnalyzeData> list = new List<AnalyzeData>();
            foreach (DataRow dr in dt.Rows)
            {
                AnalyzeData bd = new AnalyzeData()
                {
                    sid = dr["sid"].ToString(),
                    name = dr["name"].ToString(),
                    firstlevel = dr["firstlevel"].ToString(),
                    secondlevel = dr["secondlevel"].ToString(),
                    lastupdate = BizCommon.ParseToString(DateTime.Parse(dr["lastupdate"].ToString())),
                    //lastupdate = dr["lastupdate"].ToString(),
                    value = Decimal.Parse(dr["value"].ToString()),
                    rank = Int32.Parse(dr["rank"].ToString()),
                    //startdate = DateTime.Parse(dr["startdate"].ToString()),
                    startdate = BizCommon.ParseToString(DateTime.Parse(dr["startdate"].ToString())),

                };

                list.Add(bd);
            }

            return list;
        }

        public static List<AnalyzeData> QueryAnalyzeData(DateTime lastupdate,DateTime start,int level)
        {
            string sql = string.Format("select sid,name,value,firstlevel,secondlevel,lastupdate,rank,startdate,big from {0} where rank<{1} and lastupdate='{2}' and startdate='{3}' and level={4} order by rank", ANALYZE,Constant.TOP, lastupdate.ToString("yyyy-MM-dd"), start.ToString("yyyy-MM-dd"),level);
            DataSet ds = MySqlHelper.GetDataSet(sql);
            DataTable dt = ds.Tables[0];
            List<AnalyzeData> list = new List<AnalyzeData>();
            foreach (DataRow dr in dt.Rows)
            {
                AnalyzeData bd = new AnalyzeData()
                {
                    sid=dr["sid"].ToString(),
                    name = dr["name"].ToString(),
                    firstlevel = dr["firstlevel"].ToString(),
                    secondlevel = dr["secondlevel"].ToString(),
                    lastupdate = BizCommon.ParseToString(DateTime.Parse(dr["lastupdate"].ToString())),
                    value = Decimal.Parse(dr["value"].ToString()),
                    rank = Int32.Parse(dr["rank"].ToString()),
                    startdate = BizCommon.ParseToString(DateTime.Parse(dr["startdate"].ToString())),
                    level=level,
                    big = Int32.Parse(dr["big"].ToString())
                };

                list.Add(bd);
            }

            return list;
        }
        public static void  InsertAnalyzeData(DateTime start, DateTime end){
            InsertAnalyzeDataAll(BizApi.QueryInfoAll(), start, end);
        }

        public static void InsertAnalyzeDataAll(List<InfoData> id_list, DateTime start, DateTime end)
        {
            //List<AnalyzeData> list0 = new List<AnalyzeData>();
            List<AnalyzeData> list1 = new List<AnalyzeData>();
            //List<AnalyzeData> list2 = new List<AnalyzeData>();

            foreach (InfoData id in id_list)
            {
                string[] bigs = id.list.Split(','); ;
                //list0.Add(ComputeSingle2(id.sid, 0, Int32.Parse(bigs[0]), start, end));
                list1.Add(ComputeSingle2(id.sid, 1, Int32.Parse(bigs[1]), start, end));
                //list2.Add(ComputeSingle2(id.sid, 2, Int32.Parse(bigs[2]), start, end));
            }

            //sort
            //list0.Sort(new AnalyzeComparator());
            list1.Sort(new AnalyzeComparator());
            //list2.Sort(new AnalyzeComparator());
            //InsertAnalyzeData(list0, start, end, 0);
            InsertAnalyzeData(list1, start, end, 1);
            //InsertAnalyzeData(list2, start, end, 2);
        }

        public static void InsertAnalyzeData(List<AnalyzeData> list, DateTime start, DateTime end, int level)
        {
            //int index=50;

            string sql1 = String.Format("delete from {0} where lastupdate='{1}' and startdate='{2}' and level={3}", ANALYZE,DateTime.Now.ToString("yyyy-MM-dd"), start,level);
            MySqlHelper.ExecuteNonQuery(sql1);

            //List<AnalyzeData> list = ComputeAll(id_list,start, end);
            for (int i = 0; i < list.Count; i++)
            {
                if (Constant.ONLY_TOP)
                {
                    if (i > (Constant.TOP-1)) break;
                }
                AnalyzeData ad = list[i];

                string sql = String.Format(
                "INSERT INTO {0}(sid,value,name,firstlevel,secondlevel,lastupdate,rank,startdate,big,level)VALUES('{1}',{2},'{3}','{4}','{5}','{6}',{7},'{8}',{9},{10})",
                        ANALYZE, ad.sid, ad.value, ad.name, ad.firstlevel, ad.secondlevel, DateTime.Now.ToString("yyyy-MM-dd"), i, start.ToString("yyyy-MM-dd"), ad.big,ad.level);
                MySqlHelper.ExecuteNonQuery(sql);
            }
        }

        public static List<AnalyzeData> ComputeAll(int level, DateTime start, DateTime end)
        {
            List<AnalyzeData> a_list = new List<AnalyzeData>();
            List<InfoData> list = BizApi.QueryInfoAll();
            foreach (InfoData id in list)
            {
                a_list.Add(ComputeSingle2(id.sid,level,(int)(id.weight*1000),start,end));
            }
            a_list.Sort(new AnalyzeComparator());
            return a_list;
        }

        public static AnalyzeData ComputeSingle2(string sid,int level, int big, DateTime start, DateTime end)
        {
            //decimal value = 0;
            string sql = string.Format("select name,firstlevel,secondlevel,format(sqrt(sum(((buyshare-sellshare)/A.totalshare)*DATEDIFF(now(),time)*(((close-(totalmoney/A.totalshare))*(close-(totalmoney/A.totalshare))+(open-(totalmoney/A.totalshare))*(open-(totalmoney/A.totalshare))+(high-(totalmoney/A.totalshare))*(high-(totalmoney/A.totalshare))+(low-(totalmoney/A.totalshare))*(low-(totalmoney/A.totalshare)))/((high-low)*(high-low)*4)))),3) as value from {0} A join {4} B on B.sid='{0}' and A.big={1} and A.time >'{2}' and A.time<'{3}'", sid, big, start, end,INFO);
            DataSet ds = MySqlHelper.GetDataSet(sql);
            if (ds == null)
                return new AnalyzeData()
                {
                    sid = sid,
                    value = 0,
                    level=level,
                    big=big
                };
            DataTable dt = ds.Tables[0];
            string[] list = new string[dt.Rows.Count];
            if (ds.Tables[0].Rows.Count > 0)
            {
                AnalyzeData cd = new AnalyzeData()
                {
                    sid = sid,
                    level=level,
                    big=big,
                    value = Decimal.Parse(dt.Rows[0]["value"].ToString() == "" ? "0" : dt.Rows[0]["value"].ToString()),
                    name = dt.Rows[0]["name"].ToString(),
                    firstlevel = dt.Rows[0]["firstlevel"].ToString(),
                    secondlevel = dt.Rows[0]["secondlevel"].ToString()
                };
               
                return cd;
            }
            else
            {
                return new AnalyzeData()
                {
                    sid = sid,
                    value = 0
                };
            }


        }

        #endregion
        #region 插入更新
        /// <summary>
        /// if table exist,update status, if not exist, create table
        /// </summary>
        /// <param name="sid"></param>
        public static void CreateDataTable(string sid)
        {

            //if (create_table == -1)
            //{
            string sql0 = String.Format("select 1 as Count from {0} where tablename='{1}' ", CREATE_TABLE_STATUS, sid);
            DataSet ds = MySqlHelper.GetDataSet(sql0);
            //create_table = ds.Tables[0].Rows.Count;

            // }

            if (ds.Tables[0].Rows.Count == 0)
            {
                string sql = String.Format("create table {0} like {1}", sid, BASIC_TABLE);
                MySqlHelper.ExecuteNonQuery(sql);
                string sql1 = string.Format("insert {0}(tablename,lastupdate) value('{1}','{2}')", CREATE_TABLE_STATUS, sid, DateTime.Now.ToString());
                MySqlHelper.ExecuteNonQuery(sql1);
                string sq3 = string.Format("insert {0}(sid,lastupdate)values('{1}','{2}')", EXTRACT_TABLE_STATUS, sid, DateTime.MinValue);
                MySqlHelper.ExecuteNonQuery(sq3);
            }
            //if (create_table == 1)
            //{
            //    MySqlHelper.ExecuteNonQuery(string.Format("update {0} set lastupdate='{2}' where tablename='{1}'", CREATE_TABLE_STATUS, newname, DateTime.Now.ToString()));
            //    create_table = 2;
            //}
        }

        //update
        public static void CleanData(string sid)
        {

            string sql = String.Format("delete from {0}", sid);
            MySqlHelper.ExecuteNonQuery(sql);
            Console.WriteLine("clean data :" + sid);
            string sq3 = string.Format("update {0} set lastupdate='{2}' where sid='{1}'", EXTRACT_TABLE_STATUS, sid, DateTime.MinValue);
            MySqlHelper.ExecuteNonQuery(sq3);
            Console.WriteLine("update extract date :" + sid);
        }

        //插入分析好的数据
        public static void InsertBasicData(BasicData bd)
        {
            ValidateBasicData(bd);
            string sid = bd.sid;
            //CreateDataTable(sid);
            string sql = String.Format(
                "INSERT INTO {0}(time,c_type,big,buyshare,buymoney,sellshare,sellmoney,totalshare,totalmoney,open,close,high,low)VALUES('{1}','{2}',{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13})",
                        sid, bd.time, bd.c_type, bd.big, bd.buyshare, bd.buymoney, bd.sellshare, bd.sellmoney, bd.totalshare, bd.totalmoney, bd.open, bd.close, bd.high, bd.low);
            MySqlHelper.ExecuteNonQuery(sql);
            UpdateExtractStatus(bd);
        }
        #region basicinfor
        public static void InsertInfo(InfoData id)
        {
            if (info == -1)
            {
                DataSet ds = MySqlHelper.GetDataSet(String.Format("select 1 as Count from {0} where sid='{1}' ", INFO, id.sid));
                info = ds.Tables[0].Rows.Count;
            }

            if (info == 0)
            {
                string sql =
                    string.Format("insert {0}(sid,name,lastupdate,totalshare,top10total,floatshare,top10float,list,weight,firstlevel,secondlevel,location,valid) value('{1}','{2}','{3}',{4},{5},{6},{7},'{8}',{9},'{10}','{11}','{12}',{13})", INFO, id.sid, id.name, DateTime.Now.ToString("yyyy-MM-dd"), id.totalshare, id.top10total, id.floatshare, id.top10float, id.list, id.weight, id.firstlevel, id.secondlevel, id.location, id.valid);

                MySqlHelper.ExecuteNonQuery(sql);
                info = 1;
            }
            else
            {
                string sql =
                    string.Format("update {0} set name='{2}',lastupdate={3},totalshare={4},top10total={5},floatshare={6},top10float={7},list='{8}',weight={9},firstlevel='{10}',secondlevel='{11}',location='{12}',valid={13}) where sid='{1}'", INFO, id.sid, id.name, DateTime.Now.ToString("yyyy-MM-dd"), id.totalshare, id.top10total, id.floatshare, id.top10float, id.list, id.weight, id.firstlevel, id.secondlevel, id.location, id.valid);
                MySqlHelper.ExecuteNonQuery(sql);
            }
        }


        #endregion
        //更新数据抽取的时间戳
        public static void UpdateExtractStatus(BasicData bd)
        {
            ValidateBasicData(bd);
            string sid = bd.sid;

            if (extract_data == -1)
            {
                string sql = String.Format("select 1 from {0} where sid='{1}'", EXTRACT_TABLE_STATUS, sid);
                DataSet ds = MySqlHelper.GetDataSet(sql);
                extract_data = ds.Tables[0].Rows.Count;
            }
            if (extract_data == 0)
            {
                string sql = string.Format("insert {0}(sid,lastupdate)values('{1}','{2}')", EXTRACT_TABLE_STATUS, sid, bd.time);
                MySqlHelper.ExecuteNonQuery(sql);
                extract_data = 1;
            }
            else
            {
                string sql = string.Format("update {0} set lastupdate='{2}' where sid='{1}'", EXTRACT_TABLE_STATUS, sid, bd.time);
                MySqlHelper.ExecuteNonQuery(sql);
            }
        }

        public static void ValidateBasicData(BasicData bd)
        {
            if (bd == null) return;
        }
        #endregion
        //bymonth 
        //select sum(sellshare),DATE_FORMAT(time ,'%X %V') as week from test.000830_500 GROUP BY week ORDER BY week
        //by week
        //select sum(sellshare),DATE_FORMAT(time ,'%X %m') as month from test.000830_500 GROUP BY month ORDER BY month

        #region 查询api
        public static List<BasicData> QueryByMonth(string sid, int big, DateTime start, DateTime end)
        {
            return BuildDataList(sid, big, start, end, "%X_%m", "m");

        }

        public static List<BasicData> QueryByWeek(string sid, int big, DateTime start, DateTime end)
        {
            return BuildDataList(sid, big, start, end, "%X_%V", "w");
        }


        public static List<BasicData> QueryByDay(string sid, int big, DateTime start, DateTime end)
        {
            return BuildDataList(sid, big, start, end, "%X%m%d", "d");
        }


        private static List<BasicData> BuildDataList(string sid, int big, DateTime start, DateTime end, string searchTag, string type)
        {

            string sql = string.Format("select sum(buyshare) as buyshare,sum(buymoney) as buymoney,sum(sellshare) as sellshare,sum(sellmoney) as sellmoney,sum(totalshare) as totalshare,sum(totalmoney) as totalmoney,DATE_FORMAT(time ,'{4}') as tag,close,open,max(high) as high,min(low) as low from {0} where big={1} and time >'{2}' and time<'{3}' GROUP BY tag ORDER BY tag", sid, big, start, end,searchTag);

            DataSet ds = MySqlHelper.GetDataSet(sql);
            DataTable dt = ds.Tables[0];
            List<BasicData> list = new List<BasicData>();
            double incrementalTotalShare = 0;
            double incrementalBuyShare = 0;
            double incrementalSellShare = 0;
            Decimal incrementalTotalMoney = 0;
            Decimal incrementalBuyMoney = 0;
            Decimal incrementalSellMoney = 0;
            foreach (DataRow dr in dt.Rows)
            {
                incrementalTotalShare += (Double)dr["totalshare"];
                incrementalBuyShare += (Double)dr["buyshare"];
                incrementalSellShare += (Double)dr["sellshare"];
                incrementalTotalMoney += (Decimal)((Double)dr["totalmoney"]);
                incrementalBuyMoney += (Decimal)((Double)dr["buymoney"]);
                incrementalSellMoney += (Decimal)((Double)dr["sellmoney"]);
                BasicData bd = new BasicData()
                {
                    sid = sid,
                    big = big,
                    c_type = type,
                    buyshare = (Double)dr["buyshare"],
                    buymoney = (Decimal)((Double)dr["buymoney"]),
                    sellshare = (Double)dr["sellshare"],
                    sellmoney = (Decimal)((Double)dr["sellmoney"]),
                    totalshare = (Double)dr["totalshare"],
                    totalmoney = (Decimal)((Double)dr["totalmoney"]),
                    tag = (string)dr["tag"],
                    incrementalTotalShare = incrementalTotalShare,
                    incrementalBuyShare = incrementalBuyShare,
                    incrementalSellShare = incrementalSellShare,
                    incrementalTotalMoney = ProcessMoney(incrementalTotalMoney),
                    incrementalBuyMoney = ProcessMoney(incrementalBuyMoney),
                    incrementalSellMoney = ProcessMoney(incrementalSellMoney),

                    high = (Decimal)(dr["high"]),
                    low = (Decimal)(dr["low"]),
                    open = (Decimal)(dr["open"]),
                    close = (Decimal)(dr["close"])

                };

                list.Add(bd);
            }

            return list;
        }
        public static double ProcessMoney(decimal money)
        {
            return Math.Floor((double)money / 100);
            //return (double)money * 100;
        }
        /// <summary>
        /// 查询最后一次插入数据的时间
        /// </summary>
        /// <param name="sid">stock id</param>
        /// <param name="big">big deal</param>
        /// <returns></returns>
        public static DateTime QueryExtractLastUpdate(string sid)
        {
            IList<BasicData> list = new List<BasicData>();
            string sql = string.Format("select lastupdate from {0} where sid='{1}' ", EXTRACT_TABLE_STATUS, sid);
            DataSet ds = MySqlHelper.GetDataSet(sql);
            DateTime dt;
            if (ds.Tables[0].Rows.Count > 0)
                dt = DateTime.Parse(ds.Tables[0].Rows[0][0].ToString());
            else
                dt = Constant.DEFAULT_LASTUPDATE;
            return dt;
        }

        public static decimal QueryWeight(string sid)
        {
            decimal weight = 1.0M;
            string sql = string.Format("select weight from {0} where sid='{1}'", INFO, sid);
            DataSet ds = MySqlHelper.GetDataSet(sql);
            if (ds.Tables[0].Rows.Count > 0)
                weight = Decimal.Parse(ds.Tables[0].Rows[0][0].ToString());

            return weight;
        }

        public static decimal[] QueryExtractList(string sid)
        {
            string str = "";
            string sql = string.Format("select list from {0} where sid='{1}'", INFO, sid);
            DataSet ds = MySqlHelper.GetDataSet(sql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                str = ds.Tables[0].Rows[0][0].ToString();
                string[] liststr = str.Split(',');
                decimal[] list = new decimal[liststr.Length];
                for (int i = 0; i < liststr.Length; i++)
                    list[i] = decimal.Parse(liststr[i]);
                return list;
            }
            else
            {
                return new decimal[] { 500, 1000, 2000 };
            }

        }

        #endregion

        #region info

        public static string[] QueryAllIndustry()
        {
            string sql = string.Format("select distinct firstlevel as firstlevel from {0}", INFO);
            DataSet ds = MySqlHelper.GetDataSet(sql);

            DataTable dt = ds.Tables[0];
            string[] list = new string[dt.Rows.Count];
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    list[i] = dt.Rows[i][0].ToString();
                }
            }

            return list;
        }

        public static List<InfoData> QueryInfoAll()
        {
            InfoData id = new InfoData();
            string sql = string.Format("select sid,name,lastupdate,totalshare,floatshare,location,firstlevel,secondlevel,weight,list from {0} order by sid", INFO);
            DataSet ds = MySqlHelper.GetDataSet(sql);
            //DataTable dt = ds.Tables[0];
            return BuildInfoData(ds.Tables[0]);
        }

        public static InfoData QueryInfoById(string sid)
        {
            string sql = string.Format("select sid,name,lastupdate,totalshare,floatshare,location,firstlevel,secondlevel,weight,list from {0} where sid='{1}' ", INFO, sid);
            DataSet ds = MySqlHelper.GetDataSet(sql);
            //DataTable dt = ds.Tables[0];
            return BuildInfoData(ds.Tables[0])[0];
        }

        public static List<InfoData> QueryInfoByIndustry(string insutry)
        {
            string sql = string.Format("select sid,name,lastupdate,totalshare,floatshare,location,firstlevel,secondlevel,weight,list from {0} where firstlevel='{1}' ", INFO, insutry);
            DataSet ds = MySqlHelper.GetDataSet(sql);
            return BuildInfoData(ds.Tables[0]);
        }

        public static List<InfoData> QueryInfoByIndustry2(string insutry, string industry2)
        {

            string sql = string.Format("select sid,name,lastupdate,totalshare,floatshare,location,firstlevel,secondlevel,weight,list from {0} where firstlevel='{1}' and secondlevel='{2}' ", INFO, insutry,industry2);
            DataSet ds = MySqlHelper.GetDataSet(sql);
            return BuildInfoData(ds.Tables[0]);
        }


        public static string[] QueryAllLocation()
        {
            string sql = string.Format("select distinct location as location from {0}", INFO);
            DataSet ds = MySqlHelper.GetDataSet(sql);

            DataTable dt = ds.Tables[0];
            string[] list = new string[dt.Rows.Count];
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i][0].ToString().Equals(""))
                        list[i] = "empty";
                    else
                        list[i] = dt.Rows[i][0].ToString();
                }
            }

            return list;
        }
        public static List<InfoData> QueryInfoByLocation(string location)
        {
            string sql = string.Format("select sid,name,lastupdate,totalshare,floatshare,location,firstlevel,secondlevel,weight,list from {0} where location='{1}' ", INFO, location);
            DataSet ds = MySqlHelper.GetDataSet(sql);
            //DataTable dt = ds.Tables[0];
            return BuildInfoData(ds.Tables[0]);
        }

        //public static List<InfoData> QueryInfoByLocation(string location)
        //{
        //    string sql = string.Format("select sid,name,lastupdate,totalshare,floatshare,location,firstlevel,secondlevel,weight,list from {0} where location='{1}' ", INFO, location);
        //    DataSet ds = MySqlHelper.GetDataSet(sql);
        //    //DataTable dt = ds.Tables[0];
        //    return BuildInfoData(ds.Tables[0]);
        //}

        public static List<InfoData> BuildInfoData(DataTable dt)
        {
            List<InfoData> list = new List<InfoData>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    InfoData id = new InfoData();
                    id.sid = dr["sid"].ToString();
                    id.name = dr["name"].ToString();
                    id.lastupdate = DateTime.Parse(dr["lastupdate"].ToString());
                    id.totalshare = Double.Parse(dr["totalshare"].ToString());
                    id.floatshare = Double.Parse(dr["floatshare"].ToString());
                    id.location = dr["location"].ToString();
                    id.firstlevel = dr["firstlevel"].ToString(); ;
                    id.secondlevel = dr["secondlevel"].ToString();
                    id.weight = Decimal.Parse(dr["weight"].ToString());
                    id.list = dr["list"].ToString();
                    list.Add(id);
                }
            }

            return list;
        }
        #endregion

        #region k-line
        public static List<LineData> QueryLineByDay(string sid, DateTime start, DateTime end)
        {
            string sql = string.Format("select open,close,high,low,DATE_FORMAT(time ,'%X%m%d') as tag from {0} where time >'{1}' and time<'{2}' GROUP BY tag ORDER BY tag", sid, start, end);
            DataSet ds = MySqlHelper.GetDataSet(sql);
            DataTable dt = ds.Tables[0];
            return BuildLineData(dt);
        }

        public static List<LineData> QueryLineByWeek(string sid, DateTime start, DateTime end)
        {
            //这里只能取到第一天的信息
            //TODO: 取区间的开始和最后值
            string sql = string.Format("select open,close,max(high) as high,min(low) as low,DATE_FORMAT(time ,'%X_%V') as tag from {0} where time >'{1}' and time<'{2}' GROUP BY tag ORDER BY tag", sid, start, end);
            DataSet ds = MySqlHelper.GetDataSet(sql);
            DataTable dt = ds.Tables[0];
            return BuildLineData(dt);
        }

        public static List<LineData> QueryLineByMonth(string sid, DateTime start, DateTime end)
        {
            //这里只能取到第一天的信息
            //TODO: 取区间的开始和最后值
            string sql = string.Format("select open,close,max(high) as high,min(low) as low,DATE_FORMAT(time ,'%X_%m') as tag from {0} where time >'{1}' and time<'{2}' GROUP BY tag ORDER BY tag", sid, start, end);
            DataSet ds = MySqlHelper.GetDataSet(sql);
            DataTable dt = ds.Tables[0];
            return BuildLineData(dt);
        }

        private static List<LineData> BuildLineData(DataTable dt)
        {
            List<LineData> list = new List<LineData>();
            foreach (DataRow dr in dt.Rows)
            {
                LineData bd = new LineData()
                {

                    open = (Decimal)dr["open"],
                    close = (Decimal)(dr["close"]),
                    high = (Decimal)dr["high"],
                    low = (Decimal)(dr["low"]),
                    tag = (string)dr["tag"]
                };

                list.Add(bd);
            }

            return list;
        }
        #endregion
    }


    public class AnalyzeComparator : IComparer<AnalyzeData>
    {

        public int Compare(AnalyzeData obj1, AnalyzeData obj2)
        {
            if ((obj1 == null) && (obj2 == null))
            {
                return 0;
            }
            else if ((obj1 != null) && (obj2 == null))
            {
                return -1;
            }
            else if ((obj1 == null) && (obj2 != null))
            {
                return 1;
            }

            if (obj1.value > obj2.value) return -1;
            else return 1;
        }
    }
}


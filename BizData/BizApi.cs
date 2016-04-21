using big.entity;
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

        public static string BASIC_TABLE = "basictemplate";
        public static string EXTRACT_TABLE_STATUS = "extractstatus";
        public static string CREATE_TABLE_STATUS = "createstatus";

        public static string INFO = "basicinfo";
        public static string INFOEXT = "basicinfoext";
        public static string ANALYZE = "analyzedata";

        public static string RZRQ = "rzrq";

        //public static DateTime DEFAULT_LASTUPDATE = new DateTime(2014, 1, 1);


        #region  Qingyou
        private static string QueryStockDataForQingyou(DateTime start, string sid)
        {
            string ret = "";
            string sql = string.Format("select open,close,high,low, totalshare,totalmoney,buyshare,sellshare,DATE_FORMAT(time ,'%X%m%d') as tag from {0} where time ='{1}' and big=0", sid, BizCommon.ProcessSQLString(start));
            List<LineData> list = BuildLineDataForQingyou(sql);
            if (list.Count == 0) return sid+"-------";
            LineData ld = list[0];

            ret = sid + "-" + ld.tag + "-" + ld.open + "-" + ld.close + "-" + ld.high + "-" + ld.low + "-" + ld.totalmoney + "-" + ld.totalshare + "-" + ld.buyshare + "-" + ld.sellshare + ",";

            return ret.Substring(0, ret.Length - 1);
            
        }
        public static string QueryStockDataForQingyou(DateTime start, string sid, string type)
        {
            string ret = "";
            List<LineData> lineList = new List<LineData>();
            if (!string.IsNullOrEmpty(sid))
            {
                return QueryStockDataForQingyou(start, sid);
            }
            else
            {
                List<InfoData> list = new List<InfoData>();
                if (string.IsNullOrEmpty(type))
                {
                    list = QueryInfoAll();
                }
                else
                {
                    if (type == "rzrq")
                    {
                        list = QueryInfoRzrq();
                    }
                }

                foreach (InfoData id in list)
                {
                    ret += QueryStockDataForQingyou(start, id.sid) + ",";
                }

                return ret.Substring(0, ret.Length - 1);
            }
        }

        private static List<LineData> BuildLineDataForQingyou(string sql)
        {
            List<LineData> list = new List<LineData>();

            DataSet ds = MySqlHelper.GetDataSet(sql);
            if (ds == null) return list;
            DataTable dt = ds.Tables[0];
        
            foreach (DataRow dr in dt.Rows)
            {
                LineData bd = new LineData()
                {

                    open = (Decimal)dr["open"],
                    close = (Decimal)(dr["close"]),
                    high = (Decimal)dr["high"],
                    low = (Decimal)(dr["low"]),
                    tag = (string)dr["tag"],
                    totalshare = (Double)(dr["totalshare"]),
                    totalmoney = (Double)(dr["totalmoney"]),
                    buyshare = (Double)(dr["buyshare"]),
                    sellshare = (Double)(dr["sellshare"])
                };

                list.Add(bd);
            }
            
            //list.Reverse();
            return list;
        }

        #endregion
        #region update SQL
        public static void AddBigDetail(string sid)
        {
            //string sql2 = String.Format("ALTER TABLE {0} ADD COLUMN bigdetail VARCHAR(10000) NULL AFTER low", sid);
            string sql2=String.Format("ALTER TABLE {0} CHANGE COLUMN bigdetail bigdetail TEXT NULL DEFAULT NULL",sid);
            UpdateSQL(sql2, sid);
        }
        public static void UpdateSQL(string sql, string sid)
        {
            string sql2 = String.Format(sql, sid);
            MySqlHelper.ExecuteNonQuery(sql2);
        }

        public static void UpdateBasicDataForBigDeal(BasicData bd)
        {
            string sql = String.Format("select 1 from {0} where time='{1}' and big={2} ", bd.sid, bd.time, bd.big);
            DataSet ds = MySqlHelper.GetDataSet(sql);
            int count = ds.Tables[0].Rows.Count;
            if (count > 0)
            {
                string sql1 = String.Format("update  {0} set bigdetail='{1}' where time='{2}' and big={3} ", bd.sid, bd.bigdetail, bd.time, bd.big);
                MySqlHelper.ExecuteNonQuery(sql1);
            }
            else
            {
                InsertBasicData(bd);
                //string sql1 = String.Format("update  {0} set bigdetail='{1}' where time='{2}' and big={3} ", bd.sid, bd.bigdetail, bd.time, bd.big);
                //MySqlHelper.ExecuteNonQuery(sql1);
            }

            string sql2 = string.Format("update {0} set extupdate='{1}' where sid='{2}'", EXTRACT_TABLE_STATUS, bd.time, bd.sid);
            MySqlHelper.ExecuteNonQuery(sql2);
        }

        #endregion
        #region rzrq

        public static List<InfoData> QueryInfoRzrq()
        {
            string sql = string.Format("select sid,name,lastupdate,totalshare,floatshare,location,firstlevel,secondlevel,weight,list from {0}  where sid in (select sid from {1}) order by sid", INFO, RZRQ);
            return BuildInfoData(sql);
        }

        public static void InsertRzrq(RzrqData rd)
        {
            string sql1 = String.Format("delete from {0} where sid='{1}' and time='{2}' ", RZRQ, rd.sid, BizCommon.ProcessSQLString(rd.time));
            MySqlHelper.ExecuteNonQuery(sql1);

            string sql = String.Format("insert into {0}(time,sid,name,rongziyue,rongzimairue,rongzichanghuane,rongquanyuliangjine,rongquanyuliang,rongquanchanghuanliang,rongquanmaichuliang,rongquanyue)values('{1}','{2}','{3}',{4},{5},{6},{7},{8},{9},{10},{11})", RZRQ, BizCommon.ProcessSQLString(rd.time), rd.sid, rd.name, rd.rongziyue, rd.rongzimairue, rd.rongzichanghuane, rd.rongquanyuliangjine, rd.rongquanyuliang, rd.rongquanchanghuanliang, rd.rongquanmaichuliang, rd.rongquanyue);
            MySqlHelper.ExecuteNonQuery(sql);
            Console.WriteLine(rd.sid + " rzrq inserted.");
        }
        #endregion
        #region infoext
        public static void InsertInfoExt(InfoExtData ied)
        {
            string sql1 = String.Format("delete from {0} where sid='{1}' ", INFOEXT, ied.sid);
            MySqlHelper.ExecuteNonQuery(sql1);

            string sql = String.Format("insert into {0}(sid,lastupdate,shouyi,shiyinglv,jingzichan,shijinglv,shouru, shourutongbi, jinglirun, jingliruntongbi,maolilv,jinglilv, ROE,  fuzhailv,zongguben, liutonggu,zongzhi,liuzhi,meiguweifenpeilirun,shangshishijian )values('{1}','{2}',{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},'{20}')", INFOEXT, ied.sid, DateTime.Now.ToShortDateString(), ied.shouyi, ied.shiyinglv, ied.jingzichan, ied.shijinglv, ied.shouru, ied.shourutongbi, ied.jinglirun, ied.jingliruntongbi, ied.maolilv, ied.jinglilv, ied.ROE, ied.fuzhailv, ied.zongguben, ied.liutonggu, ied.zongzhi, ied.liuzhi, ied.meiguweifenpeilirun, ied.shangshishijian);
            MySqlHelper.ExecuteNonQuery(sql);
            Console.WriteLine(ied.sid + " infoext inserted.");
        }

        public static InfoExtData QueryInfoExtById(string sid)
        {
            string sql = string.Format("select sid,lastupdate,shouyi,shiyinglv,jingzichan,shijinglv,shouru,shourutongbi,jinglirun,jingliruntongbi,maolilv,jinglilv,ROE,fuzhailv,zongguben, liutonggu,zongzhi,liuzhi,meiguweifenpeilirun,shangshishijian from {0} where sid='{1}' ", INFOEXT, sid);
            List<InfoExtData> list = BuildInfoExtData(sql);
            if (list.Count > 0)
            {
                return list[0];
            }
            else
            {
                return new InfoExtData()
                {
                    sid=sid
                };
            }
            
        }

        public static List<InfoExtData> BuildInfoExtData(string sql)
        {
            List<InfoExtData> list = new List<InfoExtData>();

            DataSet ds = MySqlHelper.GetDataSet(sql);

            if (ds == null) return list;
            DataTable dt = ds.Tables[0];
            
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    InfoExtData ied = new InfoExtData();
                    ied.sid = dr["sid"].ToString();
                    ied.lastupdate = BizCommon.ProcessSQLString(DateTime.Parse(dr["lastupdate"].ToString()));
                    ied.shouyi = decimal.Parse(dr["shouyi"].ToString());
                    ied.shiyinglv = decimal.Parse(dr["shiyinglv"].ToString());
                    ied.jingzichan = decimal.Parse(dr["jingzichan"].ToString());
                    ied.shijinglv = decimal.Parse(dr["shijinglv"].ToString());
                    ied.shouru = decimal.Parse(dr["shouru"].ToString());
                    ied.shourutongbi = decimal.Parse(dr["shourutongbi"].ToString());
                    ied.jinglirun = decimal.Parse(dr["jinglirun"].ToString());
                    ied.jingliruntongbi = decimal.Parse(dr["jingliruntongbi"].ToString());
                    ied.maolilv = decimal.Parse(dr["maolilv"].ToString());
                    ied.jinglilv = decimal.Parse(dr["jinglilv"].ToString());
                    ied.ROE = decimal.Parse(dr["ROE"].ToString());
                    ied.fuzhailv = decimal.Parse(dr["fuzhailv"].ToString());
                    ied.zongguben = decimal.Parse(dr["zongguben"].ToString());
                    ied.liutonggu = decimal.Parse(dr["liutonggu"].ToString());
                    ied.zongzhi = decimal.Parse(dr["zongzhi"].ToString());
                    ied.liuzhi = decimal.Parse(dr["liuzhi"].ToString());
                    ied.meiguweifenpeilirun = decimal.Parse(dr["meiguweifenpeilirun"].ToString());
                    //ied.shangshishijian = BizCommon.ProcessSQLString(DateTime.Parse(dr["shangshishijian"].ToString()));
                    list.Add(ied);
                }
            }

            return list;
        }

        #endregion
        #region analyze

        public static List<AnalyzeData> QueryAnalyzeStatisticsByName(string tag, int level,int month,int big)
        {
            List<AnalyzeData> list = new List<AnalyzeData>();
            string sql = string.Format("select count(1) value,sum(rank) rank,name,sid,firstlevel,secondlevel from {0} where rank<{2} and tag='{1}' and level={3} group by name having value >2 order by value desc,rank asc", string.Format("{0}_{1}_{2}", ANALYZE, month, big), tag, Constant.TOP, level);
            DataSet ds = MySqlHelper.GetDataSet(sql);
            if (ds == null) return list;

            DataTable dt = ds.Tables[0];
            
            foreach (DataRow dr in dt.Rows)
            {
                AnalyzeData bd = new AnalyzeData()
                {
                    sid = dr["sid"].ToString(),
                    tag = tag,
                    name = dr["name"].ToString(),
                    firstlevel = dr["firstlevel"].ToString(),
                    secondlevel = dr["secondlevel"].ToString(),
                    enddate = BizCommon.ParseToString(DateTime.Parse(dr["enddate"].ToString())),
                    value = Decimal.Parse(dr["value"].ToString()),
                    rank = Int32.Parse(dr["rank"].ToString())
                };

                list.Add(bd);
            }

            return list;
        }


        public static List<AnalyzeData> QueryAnalyzeDataByTag(string tag,int month, int big)
        {
            List<AnalyzeData> list = new List<AnalyzeData>();

            string sql = string.Format("select A.sid,A.name,A.value,A.firstlevel,A.secondlevel,A.enddate,A.rank,A.startdate from {0} A where A.rank<{2} and A.tag='{1}' and A.big={3} order by rank", string.Format("{0}_{1}_{2}", ANALYZE, month, big), tag, Constant.TOP,big);
            DataSet ds = MySqlHelper.GetDataSet(sql);
            if (ds == null) return list;
            DataTable dt = ds.Tables[0];
            
            foreach (DataRow dr in dt.Rows)
            {
                AnalyzeData bd = new AnalyzeData()
                {
                    sid = dr["sid"].ToString(),
                    tag = tag,
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

                };

                list.Add(bd);
            }

            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="old">months ago</param>
        /// <param name="big"></param>
        /// <param name="month"> analye months</param>
        /// <returns></returns>
        public static List<AnalyzeData> QueryAnalyzeDataById(string sid, int old = 6,int big=500,int month=12)
        {
            Dictionary<string, BasicData> basicList = QueryBasicDataDicByRange(sid, old, "d", big);
            Dictionary<string, AnalyzeData> list = new Dictionary<string, AnalyzeData>();
            List<AnalyzeData> newlist = new List<AnalyzeData>();
            string sql = "";

            if (old != 0) {
                string newTag = BizCommon.ParseToString(DateTime.Now.AddMonths(-old));
                sql = string.Format("select A.sid,A.tag,A.name,A.value,A.firstlevel,A.secondlevel,A.enddate,A.rank,A.startdate,A.month from {0} A join {3} B on B.sid=A.sid and A.rank<{2} and A.sid='{1}'   and A.tag>'{4}' and A.big={5} and month={6} order by tag", string.Format("{0}_{1}_{2}", ANALYZE, month, big), sid, Constant.TOP,INFOEXT,newTag,big,month);
        }
            else
               sql = string.Format("select A.sid,A.tag,A.name,A.value,A.firstlevel,A.secondlevel,A.enddate,A.rank,A.startdate,A.month from {0} A join {3} B on B.sid=A.sid and A.rank<{2} and A.sid='{1}' and A.big={4} and A.month={5} order by rank", string.Format("{0}_{1}_{2}", ANALYZE, month, big), sid, Constant.TOP, INFOEXT,big,month);
            DataSet ds = MySqlHelper.GetDataSet(sql);
            if (ds == null) return newlist;
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

                };

                list.Add(dr["tag"].ToString(),bd);
            }

            //fill all the data
            
            foreach (string key in basicList.Keys)
            {
                if (list.ContainsKey(basicList[key].tag))
                {
                    newlist.Add(list[key]);
                }
                else
                {
                    newlist.Add(new AnalyzeData()
                    {
                        sid = basicList[key].sid,
                        rank = -100,
                        tag = basicList[key].tag,
                        month = month,
                        big = big
                    });
                }
            }
            newlist.Sort(new AnalyzeComparatorByTag());
            return newlist;
        }


        public static List<AnalyzeData> QueryAnalyzeDataByDate(string sid, DateTime start,DateTime end,int big,int month)
        {
            List<AnalyzeData> list = new List<AnalyzeData>();

            string sql = string.Format("select A.sid,A.name,A.value,A.firstlevel,A.secondlevel,A.enddate,A.rank,A.startdate,A.month from {0} A join {3} B on B.sid=A.sid and A.rank<{2} and A.sid='{1}'   and A.tag>'{4}' and A.tag<'{5}' and  A.big={6} and A.month={7} order by tag", string.Format("{0}_{1}_{2}", ANALYZE, month, big), sid, Constant.TOP, INFOEXT, BizCommon.ParseToString(start), BizCommon.ParseToString(end),big,month);
           
            DataSet ds = MySqlHelper.GetDataSet(sql);
            if (ds == null) return list;
            DataTable dt = ds.Tables[0];

            foreach (DataRow dr in dt.Rows)
            {
                AnalyzeData bd = new AnalyzeData()
                {
                    sid = dr["sid"].ToString(),
                    tag = BizCommon.ParseToString(DateTime.Parse(dr["startdate"].ToString())),
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

                };

                list.Add(bd);
            }

            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="level"></param>
        /// <param name="industry"></param>
        /// <param name="location"></param>
        /// <param name="type">CYB-创业板，ZXB-中小板，ZB-主板，SH-上海，SZ-深圳</param>
        /// <returns></returns>
        public static List<AnalyzeData> QueryAnalyzeData(string tag, int month, int level, string industry, string location, string type)
        {
            month=month<0?-month:month;
            string table = string.Format("{0}_{1}_{2}", ANALYZE, month, 500);

            string sql = "";
            if (string.IsNullOrEmpty(industry) && string.IsNullOrEmpty(location))
            {
                if (!string.IsNullOrEmpty(type))
                {
                    switch (type)
                    {
                        case "CYB":
                            sql = string.Format("select tag,level,sid,name,value,firstlevel,secondlevel,enddate,rank,startdate,big,month from {0} where  tag='{2}'  and  month={3}  and sid like 'sz3%' order by rank limit {1}", table, Constant.QUERY_TOP, tag, month);
                            break;
                        case "ZXB":
                            sql = string.Format("select A.tag tag,A.level level,A.sid sid,A.name name,A.value value,A.firstlevel firstlevel,A.secondlevel secondlevel,A.enddate enddate,A.rank rank,A.startdate startdate,A.big big, A.month as month from {0} A join {4} B on A.sid=B.sid  and A.tag='{2}'  and A.month={3}   and B.liutonggu<3  order by rank limit {1}", table,Constant.QUERY_TOP, tag, month, INFOEXT);
                            break;
                        case "XPG":
                            sql = string.Format("select A.tag tag,A.level level,A.sid sid,A.name name,A.value value,A.firstlevel firstlevel,A.secondlevel secondlevel,A.enddate enddate,A.rank rank,A.startdate startdate,A.big big , A.month as month from {0} A join {4} B on A.sid=B.sid  and A.tag='{2}'  and A.month={3} and B.liutonggu<1  order by rank limit {1}", table,  Constant.QUERY_TOP, tag, month, INFOEXT);
                            break;
                        case "ZB":
                            sql = string.Format("select A.tag tag,A.level level,A.sid sid,A.name name,A.value value,A.firstlevel firstlevel,A.secondlevel secondlevel,A.enddate enddate,A.rank rank,A.startdate startdate,A.big big, A.month as month from {0} A join {4} B on A.sid=B.sid  and A.tag='{2}'  and A.month={3} and B.liutonggu>20  order by rank limit {1}", table,  Constant.QUERY_TOP, tag, month, INFOEXT);
                            break;
                        case "SH":
                            sql = string.Format("select tag,level,sid,name,value,firstlevel,secondlevel,enddate,rank,startdate,big,month from {0} where  tag='{2}'  and month={3}  and sid like 'sh%' order by rank limit {1}", table, Constant.QUERY_TOP, tag,month);
                            break;

                        case "SZ":
                            sql = string.Format("select tag,level,sid,name,value,firstlevel,secondlevel,enddate,rank,startdate,big,month from {0} where  tag='{2}' and month={3} and sid like 'sz%' order by rank limit {1}", table, Constant.QUERY_TOP, tag,  month);
                            break;

                        case "ALL":
                            sql = string.Format("select tag,level,sid,name,value,firstlevel,secondlevel,enddate,rank,startdate,big,month from {0} where  tag='{2}' and month={3} order by rank limit {1}", table, Constant.QUERY_TOP, tag,  month);
                            break;
                        default:
                            sql = string.Format("select tag,level,sid,name,value,firstlevel,secondlevel,enddate,rank,startdate,big,month from {0} where  tag='{2}'  and month={3} order by rank limit {1}", table, Constant.QUERY_TOP, tag,  month);
                            break;

                    }
                }
                else
                {
                    sql = string.Format("select tag,level,sid,name,value,firstlevel,secondlevel,enddate,rank,startdate,big,month from {0} where  tag='{2}'  and month={3} order by rank limit {1}", table, Constant.QUERY_TOP, tag,  month);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(industry))
                    sql = string.Format("select tag,level,sid,name,value,firstlevel,secondlevel,enddate,rank,startdate,big,month from {0} where tag='{2}'  and month={3} and firstlevel='{4}' order by rank  limit {1}", ANALYZE, Constant.QUERY_TOP, tag,  month, industry);
                else if (!string.IsNullOrEmpty(location))
                    sql = string.Format("select A.tag as tag,A.level as level,A.sid as sid,A.name as name,A.value as value,A.firstlevel firstlevel,A.secondlevel as secondlevel,A.enddate as enddate,A.rank as rank,A.startdate as startdate,A.big as big, A.month as month from {0} A join {4} B  on A.sid=B.sid  and tag='{2}'  and month={3} and location='{5}'  order by rank  limit {1}", ANALYZE, Constant.QUERY_TOP, tag,  month, INFO, location);
            }
            return BuildAnalyzeData(sql);
        }

        public static List<AnalyzeData> QueryAnalyzeDataByFilter(string tag, int month, int level, decimal share, decimal shourutongbi, decimal jingzichan, string firstlevel)
        {
            string table = string.Format("{0}_{1}_{2}", ANALYZE, month, 500);
            string sql = string.Format("select tag,level,sid,name,value,firstlevel,secondlevel,enddate,rank,startdate,big,month from {0} where rank<{1} and tag='{2}'  and level={3} and and month={4}order by rank", table, Constant.TOP, tag, level, month);
            return BuildAnalyzeData(sql);
        }

        public static List<AnalyzeData> BuildAnalyzeData(string sql)
        {
            List<AnalyzeData> list = new List<AnalyzeData>();
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
                    value = Decimal.Parse(dr["value"].ToString()),
                    rank = Int32.Parse(dr["rank"].ToString()),
                    startdate = BizCommon.ParseToString(DateTime.Parse(dr["startdate"].ToString())),
                    level = Int32.Parse(dr["level"].ToString()),
                    big = Int32.Parse(dr["big"].ToString()),
                    month = Int32.Parse(dr["month"].ToString()),
                };

                list.Add(bd);
            }

            return list;
        }

        public static string QueryAnalyzeDataValue(string sid, string tag, int month, int level)
        {
            month = month < 0 ? -month : month;
            string table = string.Format("{0}_{1}_{2}", ANALYZE, month, 500);
            string sql = string.Format("select sid,name,value,firstlevel,secondlevel,enddate,rank,startdate,big from {0} where rank<{1} and tag='{2}'  and level={3} and  month={4} and sid='{5}' order by rank", table, Constant.TOP, tag, level,month, sid);
            DataSet ds = MySqlHelper.GetDataSet(sql);
            if (ds == null) return "----";
            DataTable dt = ds.Tables[0];
            if (dt.Rows.Count > 0)
            {
                string a = dt.Rows[0]["rank"].ToString();
                return P(a);
            }
            else
            {
                return "----";
            }
        }
        public static string P(string a)
        {
            string hh = "";
            for (int i = 0; i < 4 - a.Length; i++)
                hh = "0" + hh;

            return hh + a;
        }
        public static void InsertAnalyzeData(string tag, DateTime start, DateTime end,string month,int big)
        {
            InsertAnalyzeDataAll(BizApi.QueryInfoAll(), tag, start, end,month,big);
        }

        public static void InsertAnalyzeDataAll(List<InfoData> id_list, string tag, DateTime start, DateTime end,string month,int big)
        {
            //List<AnalyzeData> list0 = new List<AnalyzeData>();
            List<AnalyzeData> list1 = new List<AnalyzeData>();
            //List<AnalyzeData> list2 = new List<AnalyzeData>();

            foreach (InfoData id in id_list)
            {
                string[] bigs = id.list.Split(','); ;
                //list0.Add(ComputeSingle2(id.sid, 0, Int32.Parse(bigs[0]), start, end));
                list1.Add(ComputeSingle2(id.sid, Constant.ANALYZE_LEVEL, big, start, end));
                //list2.Add(ComputeSingle2(id.sid, 2, Int32.Parse(bigs[2]), start, end));
            }

            //sort
            //list0.Sort(new AnalyzeComparator());
            list1.Sort(new AnalyzeComparator());
            //list2.Sort(new AnalyzeComparator());
            //InsertAnalyzeData(list0, start, end, 0);
            InsertAnalyzeData(list1, tag, start, end, Constant.ANALYZE_LEVEL,month,big);
            //InsertAnalyzeData(list2, start, end, 2);
        }

        public static void DeleteAnalyzeData(string tag,int big)
        {
            string[] list = Constant.ANALYZE_TIME.Split('-');
            foreach (string month in list)
            {
                string sql1 = String.Format("delete from {0} where tag='{1}'", string.Format("{0}_{1}_{2}", ANALYZE, month, big),tag);
                MySqlHelper.ExecuteNonQuery(sql1);
            }
        }


        public static void InsertAnalyzeData(List<AnalyzeData> list, string table)
        {
            for (int i = 0; i < list.Count; i++)
            {
                InsertAnalyzeData(list[i], table);
            }
        }

        public static void InsertAnalyzeData(AnalyzeData ad, string table)
        {
                if (ad.value > 0)
                {
                    string sql = String.Format(
                    "INSERT INTO {0}(sid,value,tag,name,firstlevel,secondlevel,enddate,rank,startdate,big,level,month)VALUES('{1}',{2},'{3}','{4}','{5}','{6}','{7}',{8},'{9}',{10},{11},{12})",
                            table, ad.sid, ad.value, ad.tag, ad.name, ad.firstlevel, ad.secondlevel, ad.enddate, ad.rank, ad.startdate, ad.big, ad.level, ad.month);
                    MySqlHelper.ExecuteNonQuery(sql);
                }
        }

        public static void InsertAnalyzeData(List<AnalyzeData> list, string tag, DateTime start, DateTime end, int level,string month,int big)
        {
            
            //int index=50;
            //List<AnalyzeData> list = ComputeAll(id_list,start, end);
            for (int i = 0; i < list.Count; i++)
            {
                if (Constant.ONLY_TOP)
                {
                    if (i > (Constant.TOP - 1)) break;
                }

                AnalyzeData ad = list[i];
                //if (ad.value > 0)
                //{
                    string sql = String.Format(
                    "INSERT INTO {0}(sid,value,tag,name,firstlevel,secondlevel,enddate,rank,startdate,big,level,month)VALUES('{1}',{2},'{3}','{4}','{5}','{6}','{7}',{8},'{9}',{10},{11},{12})",
                            string.Format("{0}_{1}_{2}", ANALYZE, month, big), ad.sid, ad.value, tag, ad.name, ad.firstlevel, ad.secondlevel, BizCommon.ProcessSQLString(end), i, BizCommon.ProcessSQLString(start), ad.big, ad.level,month);
                    MySqlHelper.ExecuteNonQuery(sql);
               // }
            }
        }

        public static List<AnalyzeData> ComputeAll(int level, DateTime start, DateTime end)
        {
            List<AnalyzeData> a_list = new List<AnalyzeData>();
            List<InfoData> list = BizApi.QueryInfoAll();
            foreach (InfoData id in list)
            {
                a_list.Add(ComputeSingle2(id.sid, level, (int)(id.weight * 1000), start, end));
            }
            a_list.Sort(new AnalyzeComparator());
            return a_list;
        }

        public static AnalyzeData ComputeSingle2(string sid, int level, int big, DateTime start, DateTime end)
        {
            //decimal value = 0;
            string sql = string.Format("select name,firstlevel,secondlevel,format(sqrt(sum(((buyshare-sellshare)/A.totalshare)*DATEDIFF(now(),time)*(((close-(totalmoney/A.totalshare))*(close-(totalmoney/A.totalshare))+(open-(totalmoney/A.totalshare))*(open-(totalmoney/A.totalshare))+(high-(totalmoney/A.totalshare))*(high-(totalmoney/A.totalshare))+(low-(totalmoney/A.totalshare))*(low-(totalmoney/A.totalshare)))/((high-low)*(high-low)*4)))),3) as value from {0} A join {4} B on B.sid='{0}' and A.big={1} and A.time >='{2}' and A.time<='{3}'", sid, big, start, end, INFO);
            DataSet ds = MySqlHelper.GetDataSet(sql);
            if (ds == null)
                return new AnalyzeData()
                {
                    sid = sid,
                    value = 0,
                    level = level,
                    big = big
                };
            DataTable dt = ds.Tables[0];
            string[] list = new string[dt.Rows.Count];
            if (ds.Tables[0].Rows.Count > 0)
            {
                AnalyzeData cd = new AnalyzeData()
                {
                    sid = sid,
                    level = level,
                    big = big,
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

        //算法3
        public static List<AnalyzeData> ComputeAll_3(int days, int months)
        {
            List<InfoData> id_list = BizApi.QueryInfoAll();
            List<AnalyzeData> list = new List<AnalyzeData>();
            foreach (InfoData id in id_list)
            {
                list.Add(ComputeSingle3_1(id, days, months));
            }

            list.Sort(new AnalyzeComparator());


            foreach (AnalyzeData ad in list)
            {
                Console.WriteLine(ad.sid + " " + ad.name + " " + ad.value);
            }
            return list;
        }

        //算法3
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="days_before">开始计算的天数</param>
        /// <param name="months_before">提前多少个月</param>
        /// <returns></returns>
        public static AnalyzeData ComputeSingle3_1(InfoData id, int days_before, int months_before)
        {
            try
            {

                DateTime now = DateTime.Now;
                string tag = now.ToString("yyyyMMdd");
                DateTime end = now.AddDays(days_before);
                DateTime start = new DateTime();
                start = end.AddMonths(months_before);

                //InfoData id=BizApi.QueryInfoById(sid);

                AnalyzeData ad1 = BizApi.ComputeSingle3(id, 1, (int)(id.weight * 1000), months_before);

                //AnalyzeData ad2 = BizApi.ComputeSingle3(id, 1, (int)(id.weight * 1000), end, now);

                //ad1.value = Math.Round(ad1.value / ad2.value, 3);
                return ad1;
            }
            catch
            {
                Console.WriteLine(id.sid + " analyze fail");
            }

            return new AnalyzeData()
            {
                sid = id.sid,
                value = -1
            };
        }

        //算法3
        public static AnalyzeData ComputeSingle3(InfoData id, int level, int big,int month)
        {
            List<BasicData> bd_list = QueryBasicDataByRange(id.sid, month, "d", big);

            DateTime end = DateTime.Now;
            if (bd_list.Count == 0)
            {
                return new AnalyzeData()
                 {
                     sid = id.sid,
                     level = level,
                     big = big,
                     value = -1,
                     name = id.name
                 };
            }
            int count = bd_list.Count == 0 ? 10000 : bd_list.Count;

            decimal total_value = 0;
            foreach (BasicData bd in bd_list)
            {
                if (bd.totalshare == 0) { count = count - 1; continue; }
                decimal avg = bd.totalmoney / (decimal)bd.totalshare;
                double p0 = (bd.buyshare - bd.sellshare) / bd.totalshare;
                int day = (end - bd.time).Days;

                //处理一下涨停跌停的股票
                decimal index1 = bd.high == bd.low ? 1 : bd.high - bd.low;

                decimal p = ((bd.close - avg) * (bd.close - avg) + (bd.open - avg) * (bd.open - avg) + (bd.high - avg) * (bd.high - avg) + (bd.low - avg) * (bd.low - avg)) / (index1 * index1 * 4);
                //double p1 = Math.Sqrt((double)p);
                double p1 = (double)p;
                total_value += avg * day * (decimal)p1;
            }

            //TBD停盘了

            AnalyzeData cd = new AnalyzeData()
                 {
                     sid = id.sid,
                     level = level,
                     big = big,
                     value = Math.Round((decimal)(Math.Sqrt((double)(total_value / count))), 1),
                     name = id.name
                 };
            return cd;


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
                string sq3 = string.Format("insert {0}(sid,lastupdate)values('{1}','{2}')", EXTRACT_TABLE_STATUS, sid, Constant.ANALYZE_START_DATE);
                MySqlHelper.ExecuteNonQuery(sq3);
            }
            //if (create_table == 1)
            //{
            //    MySqlHelper.ExecuteNonQuery(string.Format("update {0} set lastupdate='{2}' where tablename='{1}'", CREATE_TABLE_STATUS, newname, DateTime.Now.ToString()));
            //    create_table = 2;
            //}
        }


        #endregion
        #region 插入更新
        /// <summary>
        /// if table exist,update status, if not exist, create table
        /// </summary>
        /// <param name="sid"></param>
        public static void CreateTable(string table,string template)
        {

            //if (create_table == -1)
            //{
            try {
            string sql0 = String.Format("select 1 as Count from {0} ", table);
            DataSet ds = MySqlHelper.GetDataSet(sql0);
            }
            catch
            {
                string sql = String.Format("create table {0} like {1}", table, template);
                MySqlHelper.ExecuteNonQuery(sql);
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
            string sq3 = string.Format("update {0} set lastupdate='{2}' where sid='{1}'", EXTRACT_TABLE_STATUS, sid, Constant.ANALYZE_START_DATE);
            MySqlHelper.ExecuteNonQuery(sq3);
            Console.WriteLine("update extract date :" + sid);
        }


        //插入分析好的数据
        public static void InsertBasicData(BasicData bd)
        {
            string sid = bd.sid;

            ValidateBasicData(bd);
            string delete_sql = String.Format(
                "delete from {0} where time='{1}' and big={2}", sid, bd.time, bd.big);
            MySqlHelper.ExecuteNonQuery(delete_sql);
            
            //CreateDataTable(sid);
            string sql = String.Format(
                "INSERT INTO {0}(time,c_type,big,buyshare,buymoney,sellshare,sellmoney,totalshare,totalmoney,open,close,high,low,bigdetail)VALUES('{1}','{2}',{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},'{14}')",
                        sid, bd.time, bd.c_type, bd.big, bd.buyshare, bd.buymoney, bd.sellshare, bd.sellmoney, bd.totalshare, bd.totalmoney, bd.open, bd.close, bd.high, bd.low,bd.bigdetail);
            MySqlHelper.ExecuteNonQuery(sql);
            UpdateExtractStatus(bd);
        }
        #region basicinfor
        public static void InsertInfo(InfoData id)
        {

            DataSet ds = MySqlHelper.GetDataSet(String.Format("select 1 as Count from {0} where sid='{1}' ", INFO, id.sid));
            //如果没有表格，建立一个新表
            if (ds == null) return;

            if (ds.Tables[0].Rows.Count == 0)
            {
                string sql =
                    string.Format("insert {0}(sid,name,lastupdate,totalshare,top10total,floatshare,top10float,list,weight,firstlevel,secondlevel,location,valid) value('{1}','{2}','{3}',{4},{5},{6},{7},'{8}',{9},'{10}','{11}','{12}',{13})", INFO, id.sid, id.name, BizCommon.ProcessSQLString(DateTime.Now), id.totalshare, id.top10total, id.floatshare, id.top10float, id.list, id.weight, id.firstlevel, id.secondlevel, id.location, id.valid);

                MySqlHelper.ExecuteNonQuery(sql);
                BizApi.CreateDataTable(id.sid);
            }
            else
            {
                //不能更新列表
                string sql =
                    string.Format("update {0} set name='{2}',lastupdate='{3}',totalshare={4},top10total={5},floatshare={6},top10float={7},weight={8},firstlevel='{9}',secondlevel='{10}',location='{11}',valid={12} where sid='{1}'", INFO, id.sid, id.name, BizCommon.ProcessSQLString(DateTime.Now), id.totalshare, id.top10total, id.floatshare, id.top10float, id.weight, id.firstlevel, id.secondlevel, id.location, id.valid);
                MySqlHelper.ExecuteNonQuery(sql);
            }

        }


        #endregion
        //更新数据抽取的时间戳
        public static void UpdateExtractStatus(BasicData bd)
        {
            ValidateBasicData(bd);
            string sid = bd.sid;


            string sql = String.Format("select 1 from {0} where sid='{1}'", EXTRACT_TABLE_STATUS, sid);
            DataSet ds = MySqlHelper.GetDataSet(sql);
            if (ds == null) return;

            if (ds.Tables[0].Rows.Count == 0)
            {
                sql = string.Format("insert {0}(sid,lastupdate)values('{1}','{2}')", EXTRACT_TABLE_STATUS, sid, bd.time);
                MySqlHelper.ExecuteNonQuery(sql);
            }
            else
            {
                sql = string.Format("update {0} set lastupdate='{2}' where sid='{1}'", EXTRACT_TABLE_STATUS, sid, bd.time);
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
            List<BasicData> list = new List<BasicData>();
            string sql = string.Format("select sum(buyshare) as buyshare,sum(buymoney) as buymoney,sum(sellshare) as sellshare,sum(sellmoney) as sellmoney,sum(totalshare) as totalshare,sum(totalmoney) as totalmoney,DATE_FORMAT(time ,'{4}') as tag,avg(close) as close,avg(open) as open,max(high) as high,min(low) as low from {0} where big={1} and time >='{2}' and time<='{3}' GROUP BY tag ORDER BY tag", sid, big, start, end, searchTag);

            DataSet ds = MySqlHelper.GetDataSet(sql);
            if (ds == null) return list;
            DataTable dt = ds.Tables[0];
            
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
            if (ds == null) return null;
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
            return BuildInfoData(sql);
        }

        public static InfoData QueryInfoById(string sid)
        {
            string sql = string.Format("select sid,name,lastupdate,totalshare,floatshare,location,firstlevel,secondlevel,weight,list from {0} where sid='{1}' ", INFO, sid);

            return BuildInfoData(sql)[0];
        }

        public static List<InfoData> QueryInfoByIndustry(string insutry)
        {
            string sql = string.Format("select sid,name,lastupdate,totalshare,floatshare,location,firstlevel,secondlevel,weight,list from {0} where firstlevel='{1}' ", INFO, insutry);
            return BuildInfoData(sql);
        }

        public static List<InfoData> QueryInfoByIndustry2(string insutry, string industry2)
        {

            string sql = string.Format("select sid,name,lastupdate,totalshare,floatshare,location,firstlevel,secondlevel,weight,list from {0} where firstlevel='{1}' and secondlevel='{2}' ", INFO, insutry, industry2);
            return BuildInfoData(sql);
        }


        public static string[] QueryAllLocation()
        {
            string sql = string.Format("select distinct location as location from {0} where location!=''", INFO);
            DataSet ds = MySqlHelper.GetDataSet(sql);
            if (ds == null) return null;
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
            return BuildInfoData(sql);
        }

        //public static List<InfoData> QueryInfoByLocation(string location)
        //{
        //    string sql = string.Format("select sid,name,lastupdate,totalshare,floatshare,location,firstlevel,secondlevel,weight,list from {0} where location='{1}' ", INFO, location);
        //    DataSet ds = MySqlHelper.GetDataSet(sql);
        //    //DataTable dt = ds.Tables[0];
        //    return BuildInfoData(ds.Tables[0]);
        //}

        public static List<InfoData> BuildInfoData(string sql)
        {
            List<InfoData> list = new List<InfoData>();
            DataSet ds = MySqlHelper.GetDataSet(sql);
            if (ds == null) return list;
            DataTable dt = ds.Tables[0];
            
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

        #region basicdata

        public static List<BasicData> QueryBasicDataByRange(string sid, int month, string type, int big)
        {
            string sql = string.Format("select big,time,c_type,close,open,low,high,totalshare,totalmoney,buyshare,buymoney,sellshare,sellmoney from {0} where c_type='{1}' and  big={2} and time>='{3}' and time<='{4}'", sid, type, big, BizCommon.ParseToString(DateTime.Now.AddMonths(-month)), BizCommon.ParseToString(DateTime.Now));
            return BuildBasicData(sql);
        }

        public static List<BasicData> BuildBasicData(string sql)
        {
            List<BasicData> list = new List<BasicData>();
            DataSet ds = MySqlHelper.GetDataSet(sql);
            if (ds == null) return list;
            DataTable dt = ds.Tables[0];
            

            foreach (DataRow dr in dt.Rows)
            {
                BasicData ied = new BasicData()
                {
                    //sid = dr["sid"].ToString(),
                    c_type = dr["c_type"].ToString(),
                    big = int.Parse(dr["big"].ToString()),
                    close = decimal.Parse(dr["close"].ToString()),
                    open = decimal.Parse(dr["open"].ToString()),
                    high = decimal.Parse(dr["high"].ToString()),
                    low = decimal.Parse(dr["low"].ToString()),
                    totalshare = double.Parse(dr["totalshare"].ToString()),
                    buyshare = double.Parse(dr["buyshare"].ToString()),
                    sellshare = double.Parse(dr["sellshare"].ToString()),
                    totalmoney = decimal.Parse(dr["totalmoney"].ToString()),
                    sellmoney = decimal.Parse(dr["sellmoney"].ToString()),
                    buymoney = decimal.Parse(dr["buymoney"].ToString()),
                    time = DateTime.Parse(dr["time"].ToString()),
                    tag=BizCommon.ParseToString(DateTime.Parse(dr["time"].ToString()))
                };

                list.Add(ied);
            }

            return list;
        }


        public static Dictionary<string, BasicData> QueryBasicDataDicByRange(string sid, int month, string type, int big)
        {
            string sql = string.Format("select big,time,c_type,close,open,low,high,totalshare,totalmoney,buyshare,buymoney,sellshare,sellmoney from {0} where c_type='{1}' and  big={2} and time>='{3}' and time<='{4}'", sid, type, big, BizCommon.ParseToString(DateTime.Now.AddMonths(-month)), BizCommon.ParseToString(DateTime.Now));
            return BuildBasicDataDic(sql);
        }

        public static Dictionary<string,BasicData> BuildBasicDataDic(string sql)
        {
            Dictionary<string, BasicData> dic = new Dictionary<string, BasicData>();
            DataSet ds = MySqlHelper.GetDataSet(sql);
            if (ds == null) return dic;
            DataTable dt = ds.Tables[0];


            foreach (DataRow dr in dt.Rows)
            {
                BasicData ied = new BasicData()
                {
                    //sid = dr["sid"].ToString(),
                    c_type = dr["c_type"].ToString(),
                    big = int.Parse(dr["big"].ToString()),
                    close = decimal.Parse(dr["close"].ToString()),
                    open = decimal.Parse(dr["open"].ToString()),
                    high = decimal.Parse(dr["high"].ToString()),
                    low = decimal.Parse(dr["low"].ToString()),
                    totalshare = double.Parse(dr["totalshare"].ToString()),
                    buyshare = double.Parse(dr["buyshare"].ToString()),
                    sellshare = double.Parse(dr["sellshare"].ToString()),
                    totalmoney = decimal.Parse(dr["totalmoney"].ToString()),
                    sellmoney = decimal.Parse(dr["sellmoney"].ToString()),
                    buymoney = decimal.Parse(dr["buymoney"].ToString()),
                    time = DateTime.Parse(dr["time"].ToString()),
                    tag = BizCommon.ParseToString(DateTime.Parse(dr["time"].ToString()))
                };

                dic.Add(BizCommon.ParseToString(DateTime.Parse(dr["time"].ToString())),ied);
            }

            return dic;
        }
        #endregion

        #region k-line

        public static List<LineData> QueryLineByLimit(string sid, int limit)
        {
            string sql = string.Format("select open,close,high,low, totalshare,totalmoney, DATE_FORMAT(time ,'%X%m%d') as tag from {0}  GROUP BY tag  order by time desc limit {1}", sid, limit);
            List<LineData> list = BuildLineData(sql);
            list.Reverse();
            return list;
        }

        public static List<LineData> QueryLineByDay(string sid, DateTime start, DateTime end)
        {
            string sql = string.Format("select open,close,high,low, totalshare,totalmoney,DATE_FORMAT(time ,'%X%m%d') as tag from {0} where time >='{1}' and time<='{2}' GROUP BY tag ORDER BY tag", sid, start, end);
            return BuildLineData(sql);
        }

        public static List<LineData> QueryLineByWeek(string sid, DateTime start, DateTime end)
        {
            //这里只能取到第一天的信息
            //TODO: 取区间的开始和最后值
            string sql = string.Format("select open,close,max(high) as high,min(low) as low, totalshare,totalmoney,DATE_FORMAT(time ,'%X_%V') as tag from {0} where time >='{1}' and time<='{2}' GROUP BY tag ORDER BY tag", sid, start, end);
            return BuildLineData(sql);
        }

        public static List<LineData> QueryLineByMonth(string sid, DateTime start, DateTime end)
        {
            //这里只能取到第一天的信息
            //TODO: 取区间的开始和最后值
            string sql = string.Format("select open,close,max(high) as high,min(low) as low, totalshare,totalmoney,DATE_FORMAT(time ,'%X_%m') as tag from {0} where time >='{1}' and time<='{2}' GROUP BY tag ORDER BY tag", sid, start, end);
            return BuildLineData(sql);
        }

        private static List<LineData> BuildLineData(string sql)
        {
            List<LineData> list = new List<LineData>();
            DataSet ds = MySqlHelper.GetDataSet(sql);
            if (ds == null) return list;
            DataTable dt = ds.Tables[0];
            
            foreach (DataRow dr in dt.Rows)
            {
                LineData bd = new LineData()
                {

                    open = (Decimal)dr["open"],
                    close = (Decimal)(dr["close"]),
                    high = (Decimal)dr["high"],
                    low = (Decimal)(dr["low"]),
                    tag = (string)dr["tag"],
                    totalshare = (Double)(dr["totalshare"]),
                    totalmoney = (Double)(dr["totalmoney"])
                };

                list.Add(bd);
            }

            //list.Reverse();
            return list;
        }
        #endregion

        #region query price

        public static string QueryLatestPrice(string sid, string tag)
        {
            if (!string.IsNullOrEmpty(tag)) tag = BizCommon.ProcessWeekend(tag);
            string sql = "";
            if (string.IsNullOrEmpty(tag))
            {
                sql = string.Format("select  close from {0} where big=0 order by id desc limit 1", sid);
            }
            else
            {
                string time = BizCommon.ProcessSQLString(BizCommon.ParseToDate(tag));
                sql = string.Format("select  close from {0} where time='{1}' and big=0 order by id desc limit 1", sid, time);
            }
            DataSet ds = MySqlHelper.GetDataSet(sql);
            //DataTable dt = ds.Tables[0];
            //DataTable dt = ds.Tables[0];
            if (ds==null|| ds.Tables[0].Rows.Count == 0)
            {
                //取前一天的收盘价
                string time1 = BizCommon.ProcessSQLString(BizCommon.ParseToDate(tag).AddDays(-1));
                string sql1 = string.Format("select  close from {0} where time='{1}' and big=0 order by id desc limit 1", sid, time1);
                DataSet ds1 = MySqlHelper.GetDataSet(sql1);
                if (ds == null) return "--";
                //DataTable dt = ds.Tables[0];
                DataTable dt1 = ds1.Tables[0];
                if (dt1.Rows.Count == 0)
                {
                    return "--";
                }
                else
                {
                    return ds1.Tables[0].Rows[0]["close"].ToString();
                }
            }
            else
            {
                return ds.Tables[0].Rows[0]["close"].ToString();
            }
        }

        public static string QueryMaxMinPriceByRange(string sid, int months)
        {

            DateTime now = DateTime.Now;
            DateTime start = now.AddMonths(-months);
            string sql = string.Format("select  max(high) as high,min(low) as low from {0} where time>='{1}' and time <='{2}'", sid, BizCommon.ProcessSQLString(start), BizCommon.ProcessSQLString(now));
            DataSet ds = MySqlHelper.GetDataSet(sql);
            //DataTable dt = ds.Tables[0];
            return ds.Tables[0].Rows[0]["low"].ToString() + "-" + ds.Tables[0].Rows[0]["high"].ToString();
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

    public class AnalyzeComparatorByTag : IComparer<AnalyzeData>
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

            if (BizCommon.ParseToDate(obj1.tag) < BizCommon.ParseToDate(obj2.tag)) return -1;
            else return 1;
        }
    }
}


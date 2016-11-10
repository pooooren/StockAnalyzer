using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace big.entity
{
    [Serializable]
    [DataContract]
    public class BasicData
    {
        
        //public int id;
        public string sid;
        public int big;

        //group by week/month
        public DateTime time;
        public string c_type;

        [DataMember]
        public string tag;
        [DataMember]
        public double buyshare;
        [DataMember]
        public decimal buymoney;
        [DataMember]
        public double sellshare;
        [DataMember]
        public decimal sellmoney;
        [DataMember]
        public double totalshare;
        [DataMember]
        public decimal totalmoney;

        [DataMember]
        public double incrementalTotalShare;
        [DataMember]
        public double incrementalBuyShare;
        [DataMember]
        public double incrementalSellShare;
        [DataMember]
        public double incrementalTotalMoney;
        [DataMember]
        public double incrementalBuyMoney;
        [DataMember]
        public double incrementalSellMoney;

        [DataMember]
        public decimal open;
        [DataMember]
        public decimal close;
        [DataMember]
        public decimal high;
        [DataMember]
        public decimal low;
        [DataMember]
        public string detail;
    }
}

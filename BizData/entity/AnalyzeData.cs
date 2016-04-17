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
    public class AnalyzeData
    {
        [DataMember]
        public string sid;

        [DataMember]
        public string tag;

        [DataMember]
        public decimal value;

        [DataMember]
        public string name;

        [DataMember]
        public string firstlevel;

        [DataMember]
        public string secondlevel;

        [DataMember]
        public string enddate;

        [DataMember]
        public string startdate;

        [DataMember]
        public int rank=-1;

        [DataMember]
        public int level;

        [DataMember]
        public int big;

        [DataMember]
        public int month;
        //public decimal liutonggu;

        //public string location;
    }
}

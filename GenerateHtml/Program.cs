using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using big;
using Common;

namespace GenerateHtml
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string path = ""; string time = "";
            if (args.Length == 2)
            {
                path = args[0];
                time = args[1];
            }
            else
            {
                path = @"C:\work\web\";
                time = BizCommon.ParseToString(DateUtil.GetCeilingWorkDay(DateTime.Now));
            }
            Generate.GenerateAll(path, time);
        }
    }
}

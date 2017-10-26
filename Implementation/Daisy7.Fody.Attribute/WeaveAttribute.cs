using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Daisy7.Fody.Demo.Attribute
{
    public class WeaveType : System.Attribute
    {
    }
    public class WeaveMethod : System.Attribute
    {
    }
    public class Log : WeaveMethod
    {
        public static void OnActionBefore(MethodBase mbBase, object[] args)
        {
            for (int i = 1; i <= args.Length; i++)
            {
                Console.WriteLine(string.Format("{0}方法，第{1}参数是：{2}", mbBase.Name, i, args[i - 1]));
            }
        }
    }
}

using Daisy7.Fody.Demo.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daisy7.Fody.Demo.Test
{
    public class NamedWeaver
    {
        [WeaveType]
        public static class UserManager
        {
            public static void Main(string[] args)
            {

                string str = GetUserName(1, "v123465");
                str += "\r\n";
                str += GetPoint(2, 3);
                Console.WriteLine(str);
                Console.ReadLine();
            }
            [Log]
            public static string GetUserName(int userId, string memberid)
            {
                return "成功";
            }
            [Log]
            public static string GetPoint(int x, int y)
            {
                var sum = x + y;

                return "用户积分: " + sum;
            }
        }
    }
}

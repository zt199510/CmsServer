using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class Utils
    { /// <summary>
      /// 将字符串转换为int类型数组
      /// </summary>
      /// <param name="str">如1,2,3,4,5</param>
      /// <returns></returns>
        public static List<string> StrToListString(string str)
        {
            var list = new List<string>();
            if (!str.Contains(","))
            {
                list.Add(str);
                return list;
            }
            var slist = str.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in slist)
            {
                list.Add(item);
            }
            return list;
        }
    }
}

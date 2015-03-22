using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.ORM;
using System.ORM.Operation;
using System.ORM.Caching;
namespace System {

    /// <summary>
    /// XCenter Json转换工具。
    /// </summary>
    public class Json
    {
        /// <summary>
        /// 获取键值对中某一字段的值
        /// </summary>
        /// <param name="map">键值对</param>
        /// <param name="field">字段名称</param>
        /// <returns></returns>
        public static Object GetField(Dictionary<String, object> map, String field)
        {
            foreach (KeyValuePair<String, object> pair in map)
            {
                if (pair.Key == field)
                {
                    return pair.Value;
                }
            }
            return null;
        }
        /// <summary>
        /// 获取键值对中某一字段的值
        /// </summary>
        /// <param name="map">键值对</param>
        /// <param name="field">字段名称</param>
        /// <returns></returns>
        public static String GetFieldStr(Dictionary<String, object> map, String field)
        {
            foreach (KeyValuePair<String, object> pair in map)
            {
                if (pair.Key == field)
                {
                    return pair.Value.ToString();
                }
            }
            return null;
        }
        /// <summary>
        /// 获取 json 字符串中某一字段的值
        /// </summary>
        /// <param name="oneJsonString">json 字符串</param>
        /// <param name="field">字段名称</param>
        /// <returns></returns>
        public static Object GetField(String oneJsonString, String field)
        {

            Dictionary<String, object> map = System.Serialization.JsonParser.Parse(oneJsonString) as Dictionary<String, object>;
            foreach (KeyValuePair<String, object> pair in map)
            {
                if (pair.Key == field)
                {
                    return pair.Value;
                }
            }
            return null;
        }
        /// <summary>
        /// 获取 json 字符串中某一字段的值
        /// </summary>
        /// <param name="oneJsonString">json 字符串</param>
        /// <param name="field">字段名称</param>
        /// <returns></returns>
        public static String GetFieldStr(String oneJsonString, String field)
        {

            Dictionary<String, object> map = System.Serialization.JsonParser.Parse(oneJsonString) as Dictionary<String, object>;
            foreach (KeyValuePair<String, object> pair in map)
            {
                if (pair.Key == field)
                {
                    return pair.Value.ToString();
                }
            }
            return null;
        }
        /// <summary>
        /// 将 json 字符串反序列化为对象
        /// </summary>
        /// <param name="oneJsonString">json 字符串</param>
        /// <param name="t">目标类型</param>
        /// <returns></returns>
        public static Object ToObject(String oneJsonString, Type t)
        {

            Dictionary<String, object> map = System.Serialization.JsonParser.Parse(oneJsonString) as Dictionary<String, object>;
            return System.Serialization.JSON.setValueToObject(t, map);
        }

        /// <summary>
        /// 将 json 字符串反序列化为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString">json 字符串</param>
        /// <returns></returns>
        public static T ToObject<T>(String jsonString)
        {
            Object result = ToObject(jsonString, typeof(T));
            return (T)result;
        }
        /// <summary>
        /// 将对象序列化为json字符串,不支持子对象(即属性为对象)的序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">序列化的对象</param>
        /// <returns></returns>
        public static String ToString(Object obj)
        {
            return System.Serialization.SimpleJsonString.ConvertObject(obj);
        }
        /// <summary>
        /// 将对象序列化为json字符串,支持子对象(即属性为对象)的序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">序列化的对象</param>
        /// <returns></returns>
        public static String ToStringEx(Object obj)
        {
            return System.Serialization.JsonString.Convert(obj);
        }
        /// <summary>
        /// 将对象序列化为json字符串,支持子对象(即属性为对象)的序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">序列化的对象</param>
        /// <returns></returns>
        public static String ToStringEx(Object obj, params System.Data.KeyValueData[] kvs)
        {
            string s = System.Serialization.JsonString.Convert(obj);
            if (kvs != null && kvs.Length > 0)
            {
                string strPx = "";
                foreach (System.Data.KeyValueData kv in kvs)
                {
                    strPx += "\"" + kv.KeyName + "\":\"" + kv.KeyValue + "\",";
                }
                string temp = s.Substring(1).Trim();
                if (temp == "}")
                {
                    strPx = strPx.Substring(0, strPx.Length - 1);
                }
                s = "{" + strPx + temp;
            }
            return s;
        }
        /// <summary>
        /// 将对象集合序列化为json字符串,不支持子对象(即属性为对象)的序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">序列化的对象</param>
        /// <returns></returns>
        public static String ToStringList(IList list)
        {
            return System.Serialization.SimpleJsonString.ConvertList(list);
        }
        /// <summary>
        /// 将对象集合序列化为json字符串,不支持子对象(即属性为对象)的序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">序列化的对象</param>
        /// <returns></returns>
        public static String ToStringListEx(IList list)
        {
            return System.Serialization.SimpleJsonString.ConvertListEx(list);
        }

        /// <summary>
        /// 将 json 字符串反序列化为对象列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString">json 字符串</param>
        /// <returns>返回对象列表</returns>
        public static List<T> ToList<T>(String jsonString)
        {

            List<T> list = new List<T>();
            if (System.strUtil.IsNullOrEmpty(jsonString)) return list;
            List<object> lists = System.Serialization.JsonParser.Parse(jsonString) as List<object>;
            if (lists != null)
            {
                foreach (Dictionary<String, object> map in lists)
                {
                    if (typeof(T) == map.GetType())
                    {
                        T t = (T)Convert.ChangeType(map, typeof(T));
                        list.Add(t);
                    }
                    else
                    {
                        Object result = System.Serialization.JSON.setValueToObject(typeof(T), map);
                        list.Add((T)result);
                    }
                }
            }
            return list;
        }
    }
    public class NetTool
    {        //得到网关地址
        public static string GetGateway()
        {
            //网关地址
            string strGateway = "";
            //获取所有网卡
            System.Net.NetworkInformation.NetworkInterface[] nics = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
            //遍历数组
            foreach (var netWork in nics)
            {
                //单个网卡的IP对象
                System.Net.NetworkInformation.IPInterfaceProperties ip = netWork.GetIPProperties();
                //获取该IP对象的网关
                System.Net.NetworkInformation.GatewayIPAddressInformationCollection gateways = ip.GatewayAddresses;
                foreach (var gateWay in gateways)
                {
                    //如果能够Ping通网关
                    if (IsPingIP(gateWay.Address.ToString()))
                    {
                        //得到网关地址
                        strGateway = gateWay.Address.ToString();
                        //跳出循环
                        break;
                    }
                }
                //如果已经得到网关地址
                if (strGateway.Length > 0)
                {
                    //跳出循环
                    break;
                }
            }
            //返回网关地址
            return strGateway;
        }/// <summary>
        /// 尝试Ping指定IP是否能够Ping通
        /// </summary>
        /// <param name="strIP">指定IP</param>
        /// <returns>true 是 false 否</returns>
        public static bool IsPingIP(string strIP)
        {
            try
            {
                //创建Ping对象
                System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
                //接受Ping返回值
                System.Net.NetworkInformation.PingReply reply = ping.Send(strIP, 1000);
                //Ping通
                return true;
            }
            catch
            {
                //Ping失败
                return false;
            }
        }
    }

    public class Tool
    {
        /// <summary>
        /// 获取Configer内容（先查找KeyValue内容，再查找Web.Config）
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConfiger(string key)
        {
            string str = KvTableUtil.GetString(key);
            if (string.IsNullOrEmpty(key))
            {
                str = System.Configuration.ConfigurationManager.AppSettings[key];
            }
            return str;
        }
    }

    /// <summary>
    /// 生成随机内容
    /// </summary>
    public class Rand
    {
        /// <summary>
        /// 生成随机数字
        /// </summary>
        /// <param name="length">生成长度</param>
        /// <returns></returns>
        public static string Number(int Length)
        {
            return Number(Length, false);
        }
        /// <summary>
        /// 生成随机数字
        /// </summary>
        /// <param name="Length">生成长度</param>
        /// <param name="Sleep">是否要在生成前将当前线程阻止以避免重复</param>
        /// <returns></returns>
        public static string Number(int Length, bool Sleep)
        {
            if (Sleep)
                System.Threading.Thread.Sleep(3);
            string result = "";
            System.Random random = new Random();
            for (int i = 0; i < Length; i++)
                result += random.Next(10).ToString();
            return result;
        }
        /// <summary>
        /// 生成随机字母与数字
        /// </summary>
        /// <param name="IntStr">生成长度</param>
        /// <returns></returns>
        public static string Str(int Length)
        {
            return Str(Length, false);
        }
        /// <summary>
        /// 生成随机字母与数字
        /// </summary>
        /// <param name="Length">生成长度</param>
        /// <param name="Sleep">是否要在生成前将当前线程阻止以避免重复</param>
        /// <returns></returns>
        public static string Str(int Length, bool Sleep)
        {
            if (Sleep)
                System.Threading.Thread.Sleep(3);
            char[] Pattern = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            string result = "";
            int n = Pattern.Length;
            System.Random random = new Random(~unchecked((int)DateTools.GetNow().Ticks));
            for (int i = 0; i < Length; i++)
            {
                int rnd = random.Next(0, n);
                result += Pattern[rnd];
            }
            return result;
        }
        /// <summary>
        /// 生成随机纯字母随机数
        /// </summary>
        /// <param name="IntStr">生成长度</param>
        /// <returns></returns>
        public static string Str_char(int Length)
        {
            return Str_char(Length, false);
        }
        /// <summary>
        /// 生成随机纯字母随机数
        /// </summary>
        /// <param name="Length">生成长度</param>
        /// <param name="Sleep">是否要在生成前将当前线程阻止以避免重复</param>
        /// <returns></returns>
        public static string Str_char(int Length, bool Sleep)
        {
            if (Sleep)
                System.Threading.Thread.Sleep(3);
            char[] Pattern = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            string result = "";
            int n = Pattern.Length;
            System.Random random = new Random(~unchecked((int)DateTools.GetNow().Ticks));
            for (int i = 0; i < Length; i++)
            {
                int rnd = random.Next(0, n);
                result += Pattern[rnd];
            }
            return result;
        }
    }

    public class DateTools
    {
        private static int timezone = 8;
        public static DateTime GetNow()
        {
            if (timezone == 0)
            {
                try
                {
                    if (file.Exists(System.Data.DbConfig.getConfigPath()))
                    {
                        string _timezone = Data.KvTableUtil.GetString("TimeZone");
                        if (string.IsNullOrEmpty(_timezone))
                        {
                            timezone = 8;
                            Data.KvTableUtil.Save("TimeZone", "8");
                        }
                        else
                        {
                            timezone = Convert.ToInt16(_timezone);
                        }
                    }
                }
                catch { }
            }
            return DateTime.UtcNow.AddHours(timezone);// 以UTC时间为准的时间戳
        }
        public static Int64 GetValidityNum()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);// 以UTC时间为准的时间戳
        }
        public static Int64 GetValidityNum(DateTime now)
        {
            TimeSpan ts = now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);// 以UTC时间为准的时间戳
        }

        /// <summary>
        /// 将nuix中的日期格式转换成正常日期格式，前提传入的格式正确
        /// </summary>
        /// <param name="timestampString">传入的时间戳</param>
        /// <returns></returns>
        public static String ConvertToWin(String timestampString)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timestampString + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime dtResult = dtStart.Add(toNow);
            return dtResult.ToString("yyyy-MM-dd HH:mm:ss");
        }
        /// <summary>
        /// 将nuix中的日期格式转换成日期时间
        /// </summary>
        /// <param name="timestampString">传入的时间戳</param>
        /// <returns></returns>
        public static DateTime ConvertToDateTime(long timestamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return dtStart.Add(new TimeSpan(timestamp * 10000000));
        }
        /// <summary>
        /// 将nuix中的日期格式转换成日期时间
        /// </summary>
        /// <param name="timestampString">传入的时间戳</param>
        /// <returns></returns>
        public static DateTime ConvertToDateTime(String timestampString)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timestampString + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }
        /// <summary>
        /// 将当前日期时间转换成unix日期时间戳格式
        /// </summary>
        /// <returns>unix时间</returns>
        public static string ConvertToUnix()
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            TimeSpan toNow =DateTime.Now.Subtract(dtStart);
            string timeStamp = toNow.Ticks.ToString();
            timeStamp = timeStamp.Substring(0, timeStamp.Length - 7);
            return timeStamp;
        }
        /// <summary>
        /// 将当前日期时间转换成unix日期时间戳格式
        /// </summary>
        /// <returns>unix时间</returns>
        public static long ConvertToUnixofLong()
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            TimeSpan toNow = DateTime.Now.Subtract(dtStart);
            return toNow.Ticks / 10000000;
        }
        /// <summary>
        /// 将当前日期时间转换成unix日期时间戳格式
        /// </summary>
        /// <returns>unix时间</returns>
        public static long ConvertToUnixofDetail()
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            TimeSpan toNow = DateTime.Now.Subtract(dtStart);
            return toNow.Ticks / 10000;
        }
        /// <summary>
        /// 将正常的日期转换成unix日期时间戳格式
        /// </summary>
        /// <param name="datetime">正常日期转换成的字符串格式如：yyyy-MM-dd HH:mm:ss</param>
        /// <returns>unix时间</returns>
        public static string ConvertToUnix(DateTime datetime)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            TimeSpan toNow = datetime.Subtract(dtStart);
            string timeStamp = toNow.Ticks.ToString();
            timeStamp = timeStamp.Substring(0, timeStamp.Length - 7);
            return timeStamp;
        }
        public static String GetDayOfWeek(DateTime now)
        {
            switch (Convert.ToInt32(now.DayOfWeek))
            {
                case 0: return "周日";
                case 1: return "周一";
                case 2: return "周二";
                case 3: return "周三";
                case 4: return "周四";
                case 5: return "周五";
                case 6: return "周六";
            }
            return string.Empty;
        }
    }
}

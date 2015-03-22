using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace System.Caching {

    /// <summary>
    /// .net 自带的 InMemory 缓存
    /// </summary>
    public class IndexCache
    {

        /// <summary>
        /// 从缓存中获取值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static String Get(String space, String index)
        {
            var obj = HttpRuntime.Cache[space + "_" + index];
            if (obj != null)
            {
                return obj.ToString();
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 将对象放入缓存，如果缓存中已有此项，则替换。a)永不过期，b)优先级为 Normal，c)没有缓存依赖项
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public static void Put(String space, String index, String val)
        {
            HttpRuntime.Cache[space + "_" + index] = val;
        }

        /// <summary>
        /// 将对象放入缓存，在参数 seconds 指定的秒数之后过期
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="seconds"></param>
        public static void Put(String space, String index, String val, int seconds)
        {
            HttpRuntime.Cache.Insert(space + "_" + index, val, null, DateTime.UtcNow.AddSeconds((double)seconds), Cache.NoSlidingExpiration);
        }

        /// <summary>
        /// 将对象放入缓存，在最后一次访问之后的 seconds 秒数之后过期（弹性过期）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="seconds"></param>
        public static void PutSliding(String space, String index, String val, int seconds)
        {
            HttpRuntime.Cache.Insert(space + "_" + index, val, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, 0, seconds));
        }

        /// <summary>
        /// 从缓存中移除某项
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(String space, String index)
        {
            HttpRuntime.Cache.Remove(space + "_" + index);
        }


    }
}

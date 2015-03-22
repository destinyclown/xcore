using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace System.Caching {

    /// <summary>
    /// .net �Դ��� InMemory ����
    /// </summary>
    public class IndexCache
    {

        /// <summary>
        /// �ӻ����л�ȡֵ
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
        /// ��������뻺�棬������������д�����滻��a)�������ڣ�b)���ȼ�Ϊ Normal��c)û�л���������
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public static void Put(String space, String index, String val)
        {
            HttpRuntime.Cache[space + "_" + index] = val;
        }

        /// <summary>
        /// ��������뻺�棬�ڲ��� seconds ָ��������֮�����
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="seconds"></param>
        public static void Put(String space, String index, String val, int seconds)
        {
            HttpRuntime.Cache.Insert(space + "_" + index, val, null, DateTime.UtcNow.AddSeconds((double)seconds), Cache.NoSlidingExpiration);
        }

        /// <summary>
        /// ��������뻺�棬�����һ�η���֮��� seconds ����֮����ڣ����Թ��ڣ�
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="seconds"></param>
        public static void PutSliding(String space, String index, String val, int seconds)
        {
            HttpRuntime.Cache.Insert(space + "_" + index, val, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, 0, seconds));
        }

        /// <summary>
        /// �ӻ������Ƴ�ĳ��
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(String space, String index)
        {
            HttpRuntime.Cache.Remove(space + "_" + index);
        }


    }
}

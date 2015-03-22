using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace System.ORM
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CommonBase<T> where T : IEntity
    {
        /// <summary>
        /// 根据ID获取对象
        /// </summary>
        /// <param name="Id">Id</param>
        /// <returns></returns>
        public static T Get(int Id)
        {
            return db.findById<T>(Id);
        }
        /// <summary>
        /// 根据Guid获取对象
        /// </summary>
        /// <param name="Guid">Guid</param>
        /// <returns></returns>
        public static T Get(string Guid)
        {
            try
            {
                return db.find<T>(string.Format("Guid='{0}'", Guid)).first();
            }
            catch { return default(T); }
        }
        /// <summary>
        /// 根据指定字段获取对象
        /// </summary>
        /// <param name="Field">字段名（varchar类型）</param>
        /// <param name="Value">值</param>
        /// <returns></returns>
        public static T GetBy(string Field, string Value)
        {
            try
            {
                return db.find<T>(string.Format("{0}='{1}'", Field, Value)).first();
            }
            catch { return default(T); }
        }

        /// <summary>
        /// 根据指定字段列表获取对象
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="isLike"></param>
        /// <returns></returns>
        public static T GetBy(bool isOr, bool isLike,Hashtable ht)
        {
            try
            {
                StringBuilder where;
                if (isOr)
                {
                    where = new StringBuilder("1=0");
                }
                else
                {
                    where = new StringBuilder("1=1");
                }
                IDictionaryEnumerator ienum = ht.GetEnumerator();
                while (ienum.MoveNext())
                {
                    if (isOr)
                    {
                        where.AppendFormat(" or {0}='{1}'", ienum.Key, ienum.Value);
                    }
                    else
                    {
                        where.AppendFormat(" and {0}='{1}'", ienum.Key, ienum.Value);
                    }
                }
                return db.find<T>(where.ToString()).first();
            }
            catch { return default(T); }
        }
        /// <summary>
        /// 根据指定字段列表获取对象
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static T GetBy(bool isOr, bool isLike, params KeyValueData[] kvs)
        {
            try
            {
                StringBuilder where;
                if (isOr)
                {
                    where = new StringBuilder("1=0");
                }
                else
                {
                    where = new StringBuilder("1=1");
                }
                if (kvs == null)
                {
                    kvs = new KeyValueData[] { };
                }
                foreach (KeyValueData kv in kvs)
                {
                    if (isOr)
                    {
                        where.AppendFormat(" or {0}='{1}'", kv.KeyName, kv.KeyValue);
                    }
                    else
                    {
                        where.AppendFormat(" and {0}='{1}'", kv.KeyName, kv.KeyValue);
                    }
                }
                return db.find<T>(where.ToString()).first();
            }
            catch { return default(T); }
        }

        /// <summary>
        /// 根据指定字段获取对象列表
        /// </summary>
        /// <param name="Field">字段名（varchar类型）</param>
        /// <param name="Value">值</param>
        /// <returns></returns>
        public static List<T> GetsBy(string Field, string Value)
        {
            try
            {
                return db.find<T>(string.Format("{0}='{1}'", Field, Value)).list();
            }
            catch { return new List<T>(); }
        }
    }
}

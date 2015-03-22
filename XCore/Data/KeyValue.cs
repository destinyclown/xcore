//------------------------------------------------------------------------------
//	文件名称：WlniaoCMS\CMS.Model\News\NewsClass.cs
//	运 行 库：2.0.50727.1882
//	代码功能：栏目分类Model类
//	最后修改：2011年12月7日 23:35:52
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.ORM;
using System.IO;
namespace System.Data
{
    /// <summary>
    /// KeyValue
    /// </summary>
    public class KeyValueData
    {
        private String _Key;
        private Object _Value;
        private String _Description;
        private DateTime _UpdateTime;
        /// <summary>
        /// 键
        /// </summary>
        public string KeyName { get { return _Key; } set { _Key = value; } }
        /// <summary>
        /// 值
        /// </summary>
        public string KeyValue
        {
            get
            {
                if (_Value is Array)
                    return ConvertArrayToString(_Value as Array);
                else
                    return _Value.ToString();
            }
            set
            {
                this._Value = value;
            }
        }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get { return _Description; } set { _Description = value; } }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get { return _UpdateTime; } set { _UpdateTime = value; } }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public KeyValueData()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public KeyValueData(string key, Object value)
        {
            KeyName = key;
            if (value == null)
            {
                _Value = "";
            }
            else
            {
                _Value = value;
            }
        }



        public override string ToString()
        {
            return string.Format("{0}={1}", KeyName, KeyValue);
        }
        /// <summary>
        /// 参数值
        /// </summary>
        public void SetValue(object val)
        {
            this._Value = val;
        }
        /// <summary>
        /// 创建参数对象
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static KeyValueData Create(string key, string value)
        {
            return new KeyValueData(key, value);
        }
        /// <summary>
        /// 创建参数对象
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static KeyValueData Create(string key, object value)
        {
            return new KeyValueData(key, value);
        }
        /// <summary>
        /// 根据Key排列
        /// </summary>
        /// <param name="kv1"></param>
        /// <param name="kv2"></param>
        /// <returns></returns>
        public static int CompareByKey(KeyValueData kv1, KeyValueData kv2)
        {
            return String.Compare(kv1.KeyName, kv2.KeyName);
        }
        /// <summary>
        /// 根据Key倒序排列
        /// </summary>
        /// <param name="kv1"></param>
        /// <param name="kv2"></param>
        /// <returns></returns>
        public static int CompareByKeyDesc(KeyValueData kv1, KeyValueData kv2)
        {
            return String.Compare(kv2.KeyName, kv1.KeyName);
        }
        /// <summary>
        /// 根据Value排列
        /// </summary>
        /// <param name="kv1"></param>
        /// <param name="kv2"></param>
        /// <returns></returns>
        public static int CompareByValue(KeyValueData kv1, KeyValueData kv2)
        {
            return String.Compare(kv1.KeyValue, kv2.KeyValue);
        }
        /// <summary>
        /// 根据Value倒序排列
        /// </summary>
        /// <param name="kv1"></param>
        /// <param name="kv2"></param>
        /// <returns></returns>
        public static int CompareByValueDesc(KeyValueData kv1, KeyValueData kv2)
        {
            return String.Compare(kv2.KeyValue, kv1.KeyValue);
        }
        /// <summary>
        /// 根据Key进行比较
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (!(obj is KeyValueData))
                return -1;
            return this._Key.CompareTo((obj as KeyValueData)._Key);
        }
        /// <summary>
        /// 将数组转为字符串
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        private static string ConvertArrayToString(Array a)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < a.Length; i++)
            {
                if (i > 0)
                    builder.Append(",");
                builder.Append(a.GetValue(i).ToString());
            }
            return builder.ToString();
        }
        /// <summary>
        /// 获取参数值
        /// </summary>
        [NotSerialize]
        public string EncodedValue
        {
            get
            {
                if (_Value is Array)
                    return System.Web.HttpUtility.UrlEncode(ConvertArrayToString(_Value as Array));
                else
                    return System.Web.HttpUtility.UrlEncode(_Value.ToString());
            }
        }
        /// <summary>
        /// 生成encode字符串
        /// </summary>
        /// <returns></returns>
        public string ToEncodedString()
        {
            return string.Format("{0}={1}", KeyName, EncodedValue);
        }


    }
    public class KvTableUtil
    {
		private static string GetPath()
        {
            return PathHelper.Map(cfgHelper.FrameworkRoot + "data/KeyValue.xml");
		}
        public static void Add(string key, string value)
        {
            Add(key, value, "");
        }
        public static void Add(string key, string value, string description)
        {
            if (GetByKey(key) == null)
            {
                List<XmlParamter> xplist = new List<XmlParamter>();
                xplist.Add(new XmlParamter("Key", key));
                xplist.Add(new XmlParamter("Value", value));
                xplist.Add(new XmlParamter("Description", ""));
                xplist.Add(new XmlParamter("UpdateTime", DateTools.GetNow().ToString("yyyy-MM-dd HH:mm:ss")));
                XMLHelper.AddData(GetPath(), "KeyValue", xplist.ToArray());
            }
        }
        public static void Edit(string key, string value)
        {
            Edit(key, value);
        }
        public static void Edit(string key, string value, string description)
        {
            KeyValueData kv = GetByKey(key);
            if (kv != null)
            {
                XmlParamter xpKey = new XmlParamter("Key", key);
                xpKey.Direction = System.IO.ParameterDirection.Equal;
                XmlParamter xpValue = new XmlParamter("Value", value);
                xpValue.Direction = System.IO.ParameterDirection.Update;
                XmlParamter xpDescription = new XmlParamter("Description", description);
                xpDescription.Direction = System.IO.ParameterDirection.Update;
                XmlParamter xpUpdateTime = new XmlParamter("UpdateTime", DateTools.GetNow().ToString("yyyy-MM-dd HH:mm:ss"));
                xpUpdateTime.Direction = System.IO.ParameterDirection.Update;
                XMLHelper.UpdateData(GetPath(), "KeyValue", xpKey, xpValue, xpDescription, xpUpdateTime);
            }
        }
        public static void Save(string key, string value)
        {
            if (GetByKey(key) != null)
            {
                Edit(key, value, "");
            }
            else
            {
                Add(key, value, "");
            }
        }
        public static Data.KeyValueData GetByKey(string key)
        {
            Data.KeyValueData kv = null;
            try
            {
                kv = new Data.KeyValueData();
                XmlParamter xpKey = new XmlParamter("Key", key);
                xpKey.Direction = System.IO.ParameterDirection.Equal;
                Xml.XmlNode xn = XMLHelper.GetDataOne(GetPath(), "KeyValue", xpKey);
                if (xn == null)
                {
                    return null;
                }
                else
                {
                    kv.KeyName = xn.Attributes["Key"].Value;
                    kv.KeyValue = xn.Attributes["Value"].Value;
                    kv.Description = xn.Attributes["Description"].Value;
                    kv.UpdateTime = Convert.ToDateTime(xn.Attributes["UpdateTime"].Value);
                }
            }
            catch { }
            return kv;
        }
        public static String GetString(string key)
        {
            var kv = GetByKey(key);
            if (kv != null && !string.IsNullOrEmpty(kv.KeyValue))
            {
                return kv.KeyValue;
            }
            return "";
        }
        public static Int32 GetInt(string key)
        {
            try
            {
                return Convert.ToInt32(GetString(key));
            }
            catch
            {
                return 0;
            }
        }
        public static Boolean GetBool(string key)
        {
            try
            {
                return Convert.ToBoolean(GetString(key));
            }
            catch
            {
                return false;
            }
        }
    }
}
/*
 * Copyright 2012 www.xcenter.cn
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.ORM;

namespace System.Serialization {

    /// <summary>
    /// 封装了 json 反序列化中的常见操作：将 json 字符串反序列化为对象、对象列表、字典等。
    /// 序列化工具见 JsonString
    /// </summary>
    public partial class JSON {

        //----------------------------------------------------- String to obj ---------------------------------------------------------------------

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

            Dictionary<String, object> map = JsonParser.Parse(oneJsonString) as Dictionary<String, object>;
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

            Dictionary<String, object> map = JsonParser.Parse(oneJsonString) as Dictionary<String, object>;
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
        public static Object ToObject( String oneJsonString, Type t ) {

            Dictionary<String, object> map = JsonParser.Parse( oneJsonString ) as Dictionary<String, object>;
            return setValueToObject( t, map );
        }

        /// <summary>
        /// 将 json 字符串反序列化为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString">json 字符串</param>
        /// <returns></returns>
        public static T ToObject<T>( String jsonString ) {
            Object result = ToObject( jsonString, typeof( T ) );
            return (T)result;
        }

        /// <summary>
        /// 将 json 字符串反序列化为对象列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString">json 字符串</param>
        /// <returns>返回对象列表</returns>
        public static List<T> ToList<T>( String jsonString ) {

            List<T> list = new List<T>();
            if (strUtil.IsNullOrEmpty( jsonString )) return list;

            List<object> lists = JsonParser.Parse( jsonString ) as List<object>;

            foreach (Dictionary<String, object> map in lists) {
                Object result = setValueToObject( typeof( T ), map );
                list.Add( (T)result );
            }

            return list;
        }

        internal static Object setValueToObject( Type t, Dictionary<String, object> map ) {
            Object result = rft.GetInstance( t );

            PropertyInfo[] properties = t.GetProperties( BindingFlags.Public | BindingFlags.Instance );


            foreach (KeyValuePair<String, object> pair in map) {

                String pName = pair.Key;
                String pValue = pair.Value.ToString();


                PropertyInfo info = getPropertyInfo(properties, pName);
                //if ((info != null) && !info.IsDefined(typeof(NotSaveAttribute), false))
                if (info != null) 
                {
                    Object objValue=null;

                    if (ReflectionUtil.IsBaseType( info.PropertyType )) {
                        objValue = Convert.ChangeType(pValue, info.PropertyType);
                        ReflectionUtil.SetPropertyValue(result, pName, objValue);
                    }
                    else if (info.PropertyType == typeof(Dictionary<String, object>))
                    {
                        objValue = pair.Value;
                        ReflectionUtil.SetPropertyValue(result, pName, objValue);
                    }
                    else if (info.PropertyType == typeof(Dictionary<String, String>))
                    {
                        Dictionary<String, String> dic = new Dictionary<string, string>();
                        try
                        {
                            var keys = (Dictionary<String, object>.KeyCollection)ReflectionUtil.GetPropertyValue(pair.Value, "Keys");
                            var values = (Dictionary<String, object>.ValueCollection)ReflectionUtil.GetPropertyValue(pair.Value, "Values");
                            if (keys != null && values != null && keys.Count == values.Count)
                            {
                                string[] _keys = new string[keys.Count];
                                string[] _values = new string[values.Count];
                                int k = 0;
                                foreach (var key in keys)
                                {
                                    _keys[k] = key;
                                    k++;
                                }
                                int v = 0;
                                foreach (var value in values)
                                {
                                    _values[v] = value.ToString();
                                    v++;
                                }
                                for (int i = 0; i < keys.Count; i++)
                                {
                                    dic.Add(_keys[i], _values[i]);
                                }
                            }
                        }
                        catch { }
                        objValue = dic;
                        ReflectionUtil.SetPropertyValue(result, pName, objValue);
                    }
                    else if (rft.IsInterface( info.PropertyType, typeof( IList ) )) {
                        try
                        {
                            List<Object> list = (List<Object>)pair.Value;

                            //取得List对象包含项的实际类型（可用）
                            //Type type = l.GetType().GetGenericArguments()[0];
                            Type type = null;
                                                        
                            type = info.PropertyType.GetGenericArguments()[0];

                            if (type == null)
                            {
                                //取得List对象包含项的实际类型（可用）
                                string tfullname = info.PropertyType.FullName;
                                tfullname = System.Text.RegularExpressions.Regex.Match(tfullname.Replace("[[", "[").Replace("]]", "]"), @"\[(?<type>[^\]]+)\]").Groups["type"].Value;
                                type = Type.GetType(tfullname);
                            }

                            //根据Type对象反射创建List<T>
                            Type listType = typeof(List<>).MakeGenericType(type);
                            object listnew = Activator.CreateInstance(listType);

                            //反射调用List的Add方法
                            System.Reflection.MethodInfo addMethod = listType.GetMethod("Add");
                            try
                            {
                                foreach (Object obj in list)
                                {
                                    addMethod.Invoke((object)listnew, new object[] { obj });
                                }
                                object count = listType.GetProperty("Count").GetValue(listnew, null);
                                result.GetType().GetProperty(pName).SetValue(result, listnew, null);
                            }
                            catch
                            {
                                foreach (Dictionary<String, object> maps in list)
                                {
                                    Object objT = System.Serialization.JSON.setValueToObject(type, maps);
                                    //list.Add(objT);
                                    addMethod.Invoke((object)listnew, new object[] { objT });
                                }
                                result.GetType().GetProperty(pName).SetValue(result, listnew, null);
                            }
                        }
                        catch (Exception ex){}
                    }
                    else {
                        try
                        {
                            objValue = rft.GetInstance(info.PropertyType);
                            if (pair.Key == "Id")
                            {
                                ReflectionUtil.SetPropertyValue(result, "Id", cvt.ToInt(pValue));
                            }
                            else
                            {
                                try
                                {
                                    var keys = (Dictionary<String, object>.KeyCollection)ReflectionUtil.GetPropertyValue(pair.Value, "Keys");
                                    var values = (Dictionary<String, object>.ValueCollection)ReflectionUtil.GetPropertyValue(pair.Value, "Values");
                                    if (keys != null && values != null && keys.Count == values.Count)
                                    {
                                        string[] _keys = new string[keys.Count];
                                        string[] _values = new string[values.Count];
                                        int k = 0;
                                        foreach (var key in keys)
                                        {
                                            _keys[k] = key;
                                            k++;
                                        }
                                        int v = 0;
                                        foreach (var value in values)
                                        {
                                            _values[v] = value.ToString();
                                            v++;
                                        }
                                        for (int i = 0; i < keys.Count; i++)
                                        {
                                            ReflectionUtil.SetPropertyValue(objValue, _keys[i], _values[i]);
                                        }
                                    }
                                }
                                catch { }
                                ReflectionUtil.SetPropertyValue(result, pair.Key, objValue);
                            }
                        }
                        catch { }
                    }
                }

            }
            return result;
        }

        //--------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// 将 json 字符串反序列化为字典对象的列表
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static List<Dictionary<String, object>> ToDictionaryList( String jsonString ) {


            List<object> list = JsonParser.Parse( jsonString ) as List<object>;

            List<Dictionary<String, object>> results = new List<Dictionary<String, object>>();
            foreach (Object obj in list) {

                Dictionary<String, object> item = obj as Dictionary<String, object>;
                results.Add( item );
            }

            return results;
        }

        /// <summary>
        /// 将 json 字符串反序列化为字典对象
        /// </summary>
        /// <param name="oneJsonString"></param>
        /// <returns></returns>
        public static Dictionary<String, object> ToDictionary( String oneJsonString ) {
            String str = trimBeginEnd( oneJsonString, "[", "]" );

            if (strUtil.IsNullOrEmpty( str )) return new Dictionary<String, object>();

            return JsonParser.Parse( str ) as Dictionary<String, object>;
        }


        private static String trimBeginEnd( String str, String beginStr, String endStr ) {
            str = str.Trim();
            str = strUtil.TrimStart( str, beginStr );
            str = strUtil.TrimEnd( str, endStr );
            str = str.Trim();
            return str;
        }

        private static PropertyInfo getPropertyInfo( PropertyInfo[] propertyList, String pName ) {
            foreach (PropertyInfo info in propertyList) {
                if (info.Name.Equals( pName )) {
                    return info;
                }
            }
            return null;
        }

        /// <summary>
        /// 将引号、冒号、逗号进行编码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static String Encode( String str ) {
            return str.Replace( "\"", "&quot;" ).Replace( ":", "&#58;" ).Replace( ",", "&#44;" ).Replace( "'", "\\'" );
        }

        /// <summary>
        /// 将引号、冒号、逗号进行解码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static String Decode( String str ) {
            return str.Replace( "&quot;", "\"" ).Replace( "&#58;", ":" ).Replace( "&#44;", "," ).Replace( "\\'", "'" );
        }

        /// <summary>
        /// 将字典序列化为 json 字符串
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static String DicToString(Dictionary<String, object> dic)
        {
            return JsonString.ConvertDictionary(dic, false,"");
        }

        public static IEntity ToEntity(String jsonString, Type t)
        {

            Dictionary<String, object> map = JsonParser.Parse(jsonString) as Dictionary<String, object>;

            return toEntityByMap(t, map, null);
        }
        private static IEntity toEntityByMap(Type t, Dictionary<String, object> map, Type parentType)
        {
            if (map == null) return null;
            IEntity result = Entity.New(t.FullName);
            EntityInfo ei = Entity.GetInfo(t);



            foreach (KeyValuePair<String, object> pair in map)
            {

                String pName = pair.Key;
                object pValue = pair.Value;


                EntityPropertyInfo p = ei.GetProperty(pName);

                Object objValue = null;

                if (ReflectionUtil.IsBaseType(p.Type))
                {
                    objValue = Convert.ChangeType(pValue, p.Type);
                }
                else if (p.IsList)
                {
                    continue;
                }

                else if (pValue is Dictionary<String, object>)
                {
                    if (p.Type == parentType) continue;

                    Dictionary<String, object> dic = pValue as Dictionary<String, object>;
                    if (dic != null && dic.Count > 0)
                    {
                        objValue = toEntityByMap(p.Type, dic, t);
                    }
                }
                else
                    continue;

                p.SetValue(result, objValue);

            }
            return result;
        }
    }
}


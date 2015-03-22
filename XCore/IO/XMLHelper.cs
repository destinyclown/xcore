//------------------------------------------------------------------------------
//	文件名称：WlniaoCMS\CMS.DALFactory\IDAL\XMLHelper.cs
//	运 行 库：2.0.50727.1882
//	代码功能：XML辅助程序
//	最后修改：2011年12月7日 23:35:52
//------------------------------------------------------------------------------
using System;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Collections;
using System.Data;
namespace System.IO
{
    public class XMLHelper
    {
        public static DataTable GetData(string path, string tableName)
        {
            XmlDocument doc = new XmlDocument();
            try
            {   ///导入XML文档
                doc.Load(path);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            DataTable dt = new DataTable();
            ///获取根节点
            XmlNode rootNode = doc.SelectSingleNode("/XmlData");
            ///节点无内容时，返回空
            if (rootNode == null || rootNode.ChildNodes.Count <= 0)
            {
                return null;
            }
            ///创建保存记录的数据列
            foreach (XmlAttribute attr in rootNode.ChildNodes[0].Attributes)
            {
                dt.Columns.Add(new DataColumn(attr.Name, typeof(string)));
            }
            ///创新获取数据节点的XPath
            string xmlPath = "/XmlData/" + tableName;
            XmlNodeList nodeList = doc.SelectNodes(xmlPath);
            ///添加节点的数据
            foreach (XmlNode node in nodeList)
            {
                DataRow row = dt.NewRow();
                foreach (DataColumn column in dt.Columns)
                {   ///读取每一个属性
                    row[column.ColumnName] = node.Attributes[column.ColumnName].Value;
                }
                dt.Rows.Add(row);
            }
            return dt;
        }
        public static DataTable GetData(string path, string tableName, params XmlParamter[] param)
        {
            XmlDocument doc = new XmlDocument();
            try
            {   ///导入XML文档
                doc.Load(path);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            DataTable dt = new DataTable();
            ///获取根节点
            XmlNode rootNode = doc.SelectSingleNode("/XmlData");
            ///节点无内容时，返回空
            if (rootNode == null || rootNode.ChildNodes.Count <= 0)
            {
                return null;
            }
            ///创建保存记录的数据列
            foreach (XmlAttribute attr in rootNode.ChildNodes[0].Attributes)
            {
                dt.Columns.Add(new DataColumn(attr.Name, typeof(string)));
            }
            ///创建获取数据节点的XPath
            string xmlPath = "/XmlData/" + tableName;
            int operationCount = 0;
            StringBuilder operation = new StringBuilder();
            foreach (XmlParamter p in param)
            {
                if (p.Direction == ParameterDirection.Insert|| p.Direction == ParameterDirection.Update)
                {
                    continue;
                }
                ///创建条件表达式
                switch (p.Direction)
                {
                    case ParameterDirection.Equal:
                        operation.Append("@" + p.Name + "='" + p.Value + "'");
                        break;
                    case ParameterDirection.NotEqual:
                        operation.Append("@" + p.Name + "<>'" + p.Value + "'");
                        break;
                    case ParameterDirection.Little:
                        operation.Append("@" + p.Name + "<'" + p.Value + "'");
                        break;
                    case ParameterDirection.Great:
                        operation.Append("@" + p.Name + ">'" + p.Value + "'");
                        break;
                    case ParameterDirection.Like:
                        operation.Append("contains(@" + p.Name + ",'" + p.Value + "')");
                        break;
                    default: break;
                }
                operationCount++;
                operation.Append(" and ");
            }
            if (operationCount > 0)
            {   ///修正XPath
                operation.Remove(operation.Length - 5, 5);
                xmlPath += "[" + operation.ToString() + "]";
            }
            XmlNodeList nodeList = doc.SelectNodes(xmlPath);
            ///添加节点的数据
            foreach (XmlNode node in nodeList)
            {
                DataRow row = dt.NewRow();
                foreach (DataColumn column in dt.Columns)
                {   ///读取每一个属性
                    row[column.ColumnName] = node.Attributes[column.ColumnName].Value;
                }
                dt.Rows.Add(row);
            }
            return dt;
        }
        public static XmlNodeList GetDataList(string path, string tableName, params XmlParamter[] param)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                try
                {   ///导入XML文档
                    doc.Load(path);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                ///获取根节点
                XmlNode rootNode = doc.SelectSingleNode("/XmlData");

                ///节点无内容时，返回空
                if (rootNode == null || rootNode.ChildNodes.Count <= 0)
                {
                    return null;
                }
                ///创建获取数据节点的XPath
                string xmlPath = "/XmlData/" + tableName;
                int operationCount = 0;
                StringBuilder operation = new StringBuilder();
                foreach (XmlParamter p in param)
                {
                    if (p.Direction == ParameterDirection.Insert
                     || p.Direction == ParameterDirection.Update)
                    {
                        continue;
                    }
                    ///创建条件表达式
                    switch (p.Direction)
                    {
                        case ParameterDirection.Equal:
                            operation.Append("@" + p.Name + "='" + p.Value + "'");
                            break;
                        case ParameterDirection.NotEqual:
                            operation.Append("@" + p.Name + "<>'" + p.Value + "'");
                            break;
                        case ParameterDirection.Little:
                            operation.Append("@" + p.Name + "<'" + p.Value + "'");
                            break;
                        case ParameterDirection.Great:
                            operation.Append("@" + p.Name + ">'" + p.Value + "'");
                            break;
                        case ParameterDirection.Like:
                            operation.Append("contains(@" + p.Name + ",'" + p.Value + "')");
                            break;
                        default: break;
                    }
                    operationCount++;
                    operation.Append(" and ");
                }
                if (operationCount > 0)
                {   ///修正XPath
                    operation.Remove(operation.Length - 5, 5);
                    xmlPath += "[" + operation.ToString() + "]";
                }
                XmlNodeList nodeList = doc.SelectNodes(xmlPath);
                return nodeList;
            }
            catch
            {
                return null;
            }
        }
        public static XmlNode GetDataOne(string path, string tableName, params XmlParamter[] param)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                try
                {   ///导入XML文档
                    doc.Load(path);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                ///获取根节点
                XmlNode rootNode = doc.SelectSingleNode("/XmlData");

                ///节点无内容时，返回空
                if (rootNode == null || rootNode.ChildNodes.Count <= 0)
                {
                    return null;
                }
                ///创建获取数据节点的XPath
                string xmlPath = "/XmlData/" + tableName;
                int operationCount = 0;
                StringBuilder operation = new StringBuilder();
                foreach (XmlParamter p in param)
                {
                    if (p.Direction == ParameterDirection.Insert
                     || p.Direction == ParameterDirection.Update)
                    {
                        continue;
                    }
                    ///创建条件表达式
                    switch (p.Direction)
                    {
                        case ParameterDirection.Equal:
                            operation.Append("@" + p.Name + "='" + p.Value + "'");
                            break;
                        case ParameterDirection.NotEqual:
                            operation.Append("@" + p.Name + "<>'" + p.Value + "'");
                            break;
                        case ParameterDirection.Little:
                            operation.Append("@" + p.Name + "<'" + p.Value + "'");
                            break;
                        case ParameterDirection.Great:
                            operation.Append("@" + p.Name + ">'" + p.Value + "'");
                            break;
                        case ParameterDirection.Like:
                            operation.Append("contains(@" + p.Name + ",'" + p.Value + "')");
                            break;
                        default: break;
                    }
                    operationCount++;
                    operation.Append(" and ");
                }
                if (operationCount > 0)
                {   ///修正XPath
                    operation.Remove(operation.Length - 5, 5);
                    xmlPath += "[" + operation.ToString() + "]";
                }
                XmlNodeList nodeList = doc.SelectNodes(xmlPath);
                return nodeList[0];
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 添加数据（以ID作为自增长字段）
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="tableName">表名称</param>
        /// <param name="param">字段列表</param>
        /// <returns>返回identity值</returns>
        public static Result AddData(string path, string tableName, params XmlParamter[] param)
        {
            return AddData("Id", path, tableName, param);
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="identity">自动增长的字段</param>
        /// <param name="path">文件路径</param>
        /// <param name="tableName">表名称</param>
        /// <param name="param">字段列表</param>
        /// <returns>返回identity值</returns>
        public static Result AddData(string identity, string path, string tableName, params XmlParamter[] param)
        {
            Result result = new Result();
            XmlDocument doc = new XmlDocument();
            try
            {
                if (System.IO.File.Exists(path))
                {
                    doc.Load(path); ///导入XML文档
                }
                else
                {
                    ///XML文档未创建时，自动创建新的XML文档
                    doc.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\"?><XmlData></XmlData>");
                }
            }
            catch (Exception ex)
            {
                result.Add("错误：" + ex.Message);
                return result;
            }
            ///选择根节点
            XmlNode node = doc.SelectSingleNode("/XmlData");
            if (node == null)
            {
                result.Add("Sorry,XML文档根节点找不到");
                return result;
            }
            int newID = 1;
            if (node.ChildNodes.Count > 0)
            {
                ///创建新记录的ID值
                newID = DataTypeConvert.ConvertToInt(node.LastChild.Attributes[identity].Value) + 1;
                if (newID < 1)
                {
                    result.Add("Sorry,生成XML自增长字段错误");
                    return result;
                }
            }
            ///创建一个新节点
            XmlNode newNode = doc.CreateNode(XmlNodeType.Element, tableName, null);
            if (newNode == null)
            {
                result.Add("Sorry,为XML创建新的子节点出错");
                return result;
            }
            ///添加ID的值
            newNode.Attributes.Append(CreateNodeAttribute(doc, identity, newID.ToString()));
            ///添加新节点的属性
            foreach (XmlParamter p in param)
            {
                newNode.Attributes.Append(CreateNodeAttribute(doc, p.Name, p.Value));
            }
            ///将新节点追加到根节点中
            node.AppendChild(newNode);
            try
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
            }
            catch { }
            try
            {   ///保存XML文档
                doc.Save(path);
            }
            catch (Exception ex)
            {
                result.Add("错误：" + ex.Message);
                return result;
            }
            return result;
        }
        public static Result UpdateData(string path, string tableName, params XmlParamter[] param)
        {
            Result result = new Result();
            XmlDocument doc = new XmlDocument();
            try
            {   ///导入XML文档
                doc.Load(path);
            }
            catch (Exception ex)
            {
                result.Add("错误：" + ex.Message);
                return result;
            }
            ///创新选择被修改节点的XPath
            string xmlPath = "/XmlData/" + tableName;
            int operationCount = 0;
            StringBuilder operation = new StringBuilder();
            foreach (XmlParamter p in param)
            {
                if (p.Direction == ParameterDirection.Insert
                 || p.Direction == ParameterDirection.Update)
                {
                    continue;
                }
                switch (p.Direction)
                {
                    case ParameterDirection.Equal:
                        operation.Append("@" + p.Name + "='" + p.Value + "'");
                        break;
                    case ParameterDirection.NotEqual:
                        operation.Append("@" + p.Name + "<>'" + p.Value + "'");
                        break;
                    case ParameterDirection.Little:
                        operation.Append("@" + p.Name + "<'" + p.Value + "'");
                        break;
                    case ParameterDirection.Great:
                        operation.Append("@" + p.Name + ">'" + p.Value + "'");
                        break;
                    case ParameterDirection.Like:
                        operation.Append("contains(@" + p.Name + ",'" + p.Value + "')");
                        break;
                    default: break;
                }
                operationCount++;
                operation.Append(" and ");
            }
            if (operationCount > 0)
            {
                operation.Remove(operation.Length - 5, 5);
                xmlPath += "[" + operation.ToString() + "]";
            }
            XmlNodeList nodeList = doc.SelectNodes(xmlPath);
            if (nodeList == null)
            {
                result.Add("需要修改的XML节点不存在！");
                return result;
            }
            ///修改节点的属性
            foreach (XmlNode node in nodeList)
            { ///修改单个节点的属性
                foreach (XmlParamter p in param)
                {
                    if (p.Direction == ParameterDirection.Update)
                    {
                        node.Attributes[p.Name].Value = p.Value;
                    }
                }
            }
            try
            {   ///保存XML文档
                doc.Save(path);
            }
            catch (Exception ex)
            {
                result.Add("错误：" + ex.Message);
                return result;
            }
            return result;
        }
        public static Result DeleteData(string path, string tableName, params XmlParamter[] param)
        {
            Result result = new Result();
            XmlDocument doc = new XmlDocument();
            try
            {   ///导入XML文档
                doc.Load(path);
            }
            catch (Exception ex)
            {
                result.Add("错误：" + ex.Message);
                return result;
            }
            ///创新选择被修改节点的XPath
            string xmlPath = "/XmlData/" + tableName;
            int operationCount = 0;
            StringBuilder operation = new StringBuilder();
            foreach (XmlParamter p in param)
            {
                if (p.Direction == ParameterDirection.Insert|| p.Direction == ParameterDirection.Update)
                {
                    continue;
                }
                switch (p.Direction)
                {
                    case ParameterDirection.Equal:
                        operation.Append("@" + p.Name + "='" + p.Value + "'");
                        break;
                    case ParameterDirection.NotEqual:
                        operation.Append("@" + p.Name + "<>'" + p.Value + "'");
                        break;
                    case ParameterDirection.Little:
                        operation.Append("@" + p.Name + "<'" + p.Value + "'");
                        break;
                    case ParameterDirection.Great:
                        operation.Append("@" + p.Name + ">'" + p.Value + "'");
                        break;
                    case ParameterDirection.Like:
                        operation.Append("contains(@" + p.Name + ",'" + p.Value + "')");
                        break;
                    default: break;
                }
                operationCount++;
                operation.Append(" and ");
            }
            if (operationCount > 0)
            {
                operation.Remove(operation.Length - 5, 5);
                xmlPath += "[" + operation.ToString() + "]";
            }
            XmlNodeList nodeList = doc.SelectNodes(xmlPath);
            if (nodeList == null)
            {
                result.Add("需要删除的XML节点不存在！");
                return result;
            }
            ///删除被选择的节点
            foreach (XmlNode node in nodeList)
            { ///删除单个节点
                XmlNode parentNode = node.ParentNode;
                parentNode.RemoveChild(node);
            }
            try
            {   ///保存XML文档
                doc.Save(path);
            }
            catch (Exception ex)
            {
                result.Add("错误：" + ex.Message);
                return result;
            }
            return result;
        }
        private static XmlAttribute CreateNodeAttribute(XmlDocument doc, String name, String value)
        {
            XmlAttribute attribute = doc.CreateAttribute(name, null);
            attribute.Value = value;
            return attribute;
        }
        private static XmlParamter CreateParameter(string name, string value, ParameterDirection direciton)
        {
            XmlParamter p = new XmlParamter();
            p.Name = name;
            p.Value = value;
            p.Direction = direciton;
            return p;
        }
        public static XmlParamter CreateInsertParameter(string name, string value)
        {
            return CreateParameter(name, value, ParameterDirection.Insert);
        }
        public static XmlParamter CreateUpdateParameter(string name, string value)
        {
            return CreateParameter(name, value, ParameterDirection.Update);
        }
        public static XmlParamter CreateEqualParameter(string name, string value)
        {
            return CreateParameter(name, value, ParameterDirection.Equal);
        }
        public static XmlParamter CreateGreatParameter(string name, string value)
        {
            return CreateParameter(name, value, ParameterDirection.Great);
        }
        public static XmlParamter CreateLittleParameter(string name, string value)
        {
            return CreateParameter(name, value, ParameterDirection.Little);
        }
        public static XmlParamter CreateNotEqualParameter(string name, string value)
        {
            return CreateParameter(name, value, ParameterDirection.NotEqual);
        }
        public static XmlParamter CreateLikeParameter(string name, string value)
        {
            return CreateParameter(name, value, ParameterDirection.Like);
        }
    }
    /// <summary>
    /// 数据类型转换
    /// </summary>
    public class DataTypeConvert
    {
        public DataTypeConvert() { }
        /// <summary>
        /// 字符串转换为整数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ConvertToInt(string value)
        {   ///数据为空，返回-1
            if (string.IsNullOrEmpty(value)) return -1;
            int result = -1;
            ///执行转换操作
            Int32.TryParse(value, out result);
            return result;
        }
        /// <summary>
        /// 字符串转换为时间
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime ConvertToDateTime(string value)
        {   ///定义初始化值
            DateTime result = DateTime.Parse("1900-01-01");
            if (string.IsNullOrEmpty(value)) return result;
            ///执行转换操作
            DateTime.TryParse(value, out result);
            return result;
        }
        /// <summary>
        /// 字符串转换为实数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal ConvertToDecimal(string value)
        {   ///定义初始化值
            decimal result = 0.0M;
            if (string.IsNullOrEmpty(value)) return result;
            ///执行转换操作
            decimal.TryParse(value, out result);
            return result;
        }
        /// <summary>
        /// 字符串转换为布尔类型
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ConvertToBoolean(string value)
        {   ///定义初始化值
            bool result = false;
            if (string.IsNullOrEmpty(value)) return result;
            ///执行转换操作
            bool.TryParse(value, out result);
            return result;
        }
    }
    public enum ParameterDirection
    {
        Insert,
        Update,
        Equal,
        NotEqual,
        Little,
        Great,
        Like
    }
    public sealed class XmlParamter //定义该类为密封类，阻止该类被继承
    {
        public XmlParamter() { } //实例构造函数
        public XmlParamter(string name, string value) //实例构造函数
        {
            this.name = name;
            this.value = value;
        }
        private string name;
        /// <summary>
        /// 属性名称
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }
        private string value;
        /// <summary>
        /// 属性值
        /// </summary>
        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        private ParameterDirection direction;
        public ParameterDirection Direction
        {
            get { return this.direction; }
            set { this.direction = value; }
        }
    }
}

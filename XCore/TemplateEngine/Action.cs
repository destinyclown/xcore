//------------------------------------------------------------------------------
//	文件名称：WlniaoCMS\Mobirds.TemplateEngine\LabelParameter.cs
//	运 行 库：2.0.50727.1882
//	代码功能：标签参数
//	最后修改：2011年12月7日 23:35:52
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
namespace System.TemplateEngine
{
    /// <summary>
    /// 标签参数
    /// </summary>
    public struct LabelParameter
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        public string LPName;
        /// <summary>
        /// 参数值
        /// </summary>
        public string LPValue;
    }
    public class Action : System.Web.UI.Page
    {
        private System.Data.KeyValueData[] _Params;
        /// <summary>
        /// 传递的参数集合
        /// </summary>
        public System.Data.KeyValueData[] Params { get { return _Params; } set { _Params = value; } }
		private HttpContext _ThisContext;
        /// <summary>
        /// 当前请求
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
		public HttpContext ThisContext { get{return _ThisContext;} set{_ThisContext=value;} }
		private String _TagContent;
        /// <summary>
        /// 标签内容
        /// </summary>
		public string TagContent { get{return _TagContent;} set{_TagContent=value;} }
		private String _SkinRoot;
        /// <summary>
        /// 模版路径
        /// </summary>
		public string SkinRoot { get{return _SkinRoot;} set{_SkinRoot=value;} }
        /// <summary>
        /// 标签所有参数
        /// </summary>
        protected LabelParameter[] LblParams = null;
        /// <summary>
        /// 标签名称
        /// </summary>
        protected string LabelName = null;
		private String _LabelType;
        /// <summary>
        /// 标签种类
        /// </summary>
		public string LabelType { get{return _LabelType;} set{_LabelType=value;} }
        /// <summary>
        /// 最终的HTML代码
        /// </summary>
        protected string _FinalHtmlCode = string.Empty;
		private String _CurrentClassID;
        /// <summary>
        /// 当前的栏目ID
        /// </summary>
		public string CurrentClassID { get{return _CurrentClassID;} set{_CurrentClassID=value;} }
		private String _CurrentNewsID;
        /// <summary>
        /// 当前新闻ID
        /// </summary>
		public string CurrentNewsID { get{return _CurrentNewsID;} set{_CurrentNewsID=value;} }
		private String _CurrentPath;
        /// <summary>
        /// 当前访问路径
        /// </summary>
		public string CurrentPath { get{return _CurrentPath;} set{_CurrentPath=value;} }
        /// <summary>
        /// 标签的主体部份
        /// </summary>
        public string Primary = string.Empty;
        /// <summary>
        /// 标签中间插入代码部份或模版文件内容
        /// </summary>
        public string Inserted = string.Empty;
        /// <summary>
        /// 标签中间插入图片集代码的部份
        /// </summary>
        public string InsertedPic = string.Empty;
        /// <summary>
        /// 循环次数
        /// </summary>
        protected int Loop = 0;
        /// <summary>
        /// 标签的验证信息
        /// </summary>
        protected string _VerifyInfo = string.Empty;
        /// <summary>
        /// 解析标签内容
        /// </summary>
        public bool ParseContent()
        {
            int pos1 =TagContent.IndexOf(']');
            int pos2 = TagContent.LastIndexOf("[/XCY");
            int tempi = TagContent.IndexOf("[");
            if (TagContent.Length > 0 && pos1 > 1 && pos2 > 1)
            {
                Primary = TagContent.Substring(tempi + 1, pos1 - 1);
                int n = pos2 - pos1 - 1;
                if (n > 0)
                    Inserted = TagContent.Substring(pos1 + 1, n);
            }
            pos1 = TagContent.IndexOf("PIC]");
            pos2 = TagContent.LastIndexOf("[/PIC");
            tempi = TagContent.IndexOf("[PIC");
            if (TagContent.Length > 0 && pos1 > 1 && pos2 > 1)
            {
                int n = pos2 - pos1 - 4;
                if (n > 0)
                    InsertedPic = TagContent.Substring(pos1 + 4, n);
            }
           return AnalyzeLabel();
        }
        /// <summary>
        /// 解析标签参数
        /// </summary>
        /// <returns></returns>
        private bool AnalyzeLabel()
        {
            bool result = true;
            if (!string.IsNullOrEmpty(GetParam("classid")))
                CurrentClassID = GetParam("classid");
            if (Primary.IndexOf(",") > 0)
            {
                IList<LabelParameter> l = new List<LabelParameter>();
                l.Clear();
                string[] _mass_p = Primary.Split(',');
                foreach (string param in _mass_p)
                {
                    try
                    {
                        string[] param_s = param.Split(new char[] { '=' });
                        LabelParameter p;
                        try
                        {
                            p.LPName = param_s[0].Trim();
                            p.LPValue = param_s[1].Trim();
                        }
                        catch
                        {
                            p.LPName = "";
                            p.LPValue = "";
                        }
                        #region 对标签的必要参数进行一些处理
                        switch (p.LPName)
                        {
                            case "XCY:Number":
                            case "XCY:PageSize":
                                try
                                {
                                    Loop = int.Parse(p.LPValue);
                                }
                                catch
                                {
                                    result = false;
                                    _VerifyInfo = "XCY:Number的值不是有效的数字";
                                }
                                break;
                            case "XCY:ClassId":
                                if (p.LPValue == "0")
                                    CurrentClassID = GetParam("classid");
                                if (!string.IsNullOrEmpty(CurrentClassID) && p.LPValue == "0")
                                    p.LPValue = CurrentClassID;
                                AddParameter(p, ref l);
                                break;
                            case "XCY:ContentId":
                                if (p.LPValue == "0")
                                    CurrentNewsID = GetParam("id");
                                else
                                    CurrentNewsID = p.LPValue;
                                AddParameter(p, ref l);
                                break;
                            case "XCY:LabelType":
                                LabelType = p.LPValue;
                                break;
                            default:
                                AddParameter(p, ref l);
                                break;
                        }
                        #endregion 对标签的必要参数进行一些处理
                    }
                    catch
                    {
                        result = false;
                        break;
                    }
                }
                if (result && l.Count > 0)
                {
                    LblParams = new LabelParameter[l.Count];
                    l.CopyTo(LblParams, 0);
                }
            }
            return result;
        }
        /// <summary>
        /// 将一个参数加入参数队列
        /// </summary>
        /// <param name="lp">标签参数</param>
        /// <param name="list">列表</param>
        private void AddParameter(LabelParameter lp, ref IList<LabelParameter> list)
        {
            bool flag = true;
            foreach (LabelParameter p in list)
            {
                if (p.LPName.Equals(lp.LPName))
                {
                    flag = false; break;
                }
            }
            if (flag)
                list.Add(lp);
        }
        /// <summary>
        /// 查找某一参数的值
        /// </summary>
        /// <param name="key">参数的名称</param>
        /// <returns></returns>
        public string GetParam(string key)
        {
            try
            {
                foreach (LabelParameter p in LblParams)
                {
                    if (p.LPName == key)
                        return p.LPValue;
                }
            }
            catch { }
            try
            {
                foreach (System.Data.KeyValueData p in Params)
                {
                    if (p.KeyName.ToLower() == key.ToLower())
                        return p.KeyValue;
                }
            }
            catch { }
            return "";
        }
        /// <summary>
        /// 获得整形参数
        /// </summary>
        /// <param name="key">参数名称</param>
        /// <returns>参数值</returns>
        public Int32 GetIntParam(string key)
        {
            return Convert.ToInt32(GetParam(key));
        }
        public virtual String TagConvert()
        {
            return TagContent;
        }
    }
    public class ActionBase : Action
    {
        /// <summary>
        /// 获取文章信息
        /// </summary>
        /// <returns></returns>
        public virtual String GetArticle()
        {
            return "";
        }
        /// <summary>
        /// 获取栏目信息
        /// </summary>
        /// <returns></returns>
        public virtual String GetNewsClass()
        {
            return "";
        }
    }
    /// <summary>
    /// 文章栏目基础数据
    /// </summary>
    public class ClassBase
    {
		private String _ClassId;
		private String _ClassPath;
		private String _ClassType;
		private String _ClassTemplate;
		private String _ContentTemplate;
		public string ClassId { get{return _ClassId;} set{_ClassId=value;} }
		public string ClassPath { get{return _ClassPath;} set{_ClassPath=value;} }
		public string ClassType { get{return _ClassType;} set{_ClassType=value;} }
		public string ClassTemplate { get{return _ClassTemplate;} set{_ClassTemplate=value;} }
		public string ContentTemplate { get{return _ContentTemplate;} set{_ContentTemplate=value;} }
    }
    /// <summary>
    /// 文章信息基础数据
    /// </summary>
    public class ArticleBase
    {
		private String _ArticleId;
		private String _ClassId;
		private String _ContentTemplate;
		public string ArticleId { get{return _ArticleId;} set{_ArticleId=value;} }
		public string ClassId { get{return _ClassId;} set{_ClassId=value;} }
		public string ContentTemplate { get{return _ContentTemplate;} set{_ContentTemplate=value;} }
    }
}

//------------------------------------------------------------------------------
//	文件名称：WlniaoCMS\Core\AshxHelper.cs
//	运 行 库：2.0.50727.1882
//	代码功能：请求处理的辅助类
//	最后修改：2012年3月26日 12:30:27
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Xml;
namespace System.Web
{
    public class AshxHelper
    {
        HttpContext _context = null;
        System.Collections.Hashtable ht;
        System.Collections.IDictionary htKeyValue = null;
        System.Collections.Generic.List<ArrayList> arraylist = null;

        private Result _Result = new Result();
        public Result Result
        {
            get { return _Result; }
            set { _Result.Join(value); }
        }
        public HttpContext Context
        {
            get
            {
                return _context;
            }
        }
        public AshxHelper()
        {
            _context = HttpContext.Current;
            ht = new System.Collections.Hashtable();
            arraylist = new List<ArrayList>();
        }
        public AshxHelper(HttpContext context)
        {
            _context = context;
            ht = new System.Collections.Hashtable();
            arraylist = new List<ArrayList>();
        }
        public void Add(String key, Object value)
        {
            if (ht[key] == null)
            {
                if (value == null)
                    ht.Add(key, "");
                else
                    ht.Add(key, value);
            }
        }
        public void Add(ArrayList array)
        {
            arraylist.Add(array);
        }
        public void Join(Result result)
        {
            _Result.Join(result);
        }
        public void InitParam(IDictionary ht)
        {
            htKeyValue = ht;
        }
        public string GetParam(string key)
        {
            string var = string.Empty;
            if (htKeyValue == null)
            {
                if (string.IsNullOrEmpty(_context.Request.Form[key]))
                {
                    var = System.Web.HttpUtility.HtmlDecode(_context.Request.QueryString[key]);
                }
                else
                {
                    var = System.Web.HttpUtility.HtmlDecode(_context.Request.Form[key]);
                }
            }
            else
            {
                try { var = htKeyValue[key].ToString(); }catch { }
            }
            if (string.IsNullOrEmpty(var))
            {
                return "";
            }
            return var;
        }
        public int GetParamInt(string key)
        {
            try
            {
                return int.Parse(GetParam(key));
            }
            catch
            {
                return 0;
            }
        }
        public float GetParamFloat(string key)
        {
            try
            {
                return float.Parse(GetParam(key));
            }
            catch
            {
                return 0;
            }
        }
        public override string ToString()
        {
            return Json.ToStringEx(ht);
        }
        public void Response()
        {
            _context.Response.ContentType = "text/plain";
            _context.Response.Clear();
            try
            {
                ht.Add("success", _Result.IsValid);
                ht.Add("msg", _Result.Errors[0]);
            }
            catch { }
            _context.Response.Write(this.ToString());
            _context.Response.End();
            //_context.ApplicationInstance.CompleteRequest();
        }
        public void Response(String sEcho, Int32 pageCount)
        {
            _context.Response.ContentType = "text/plain";
            _context.Response.Clear();
            string temp = Json.ToStringEx(arraylist);
            string json = string.Format("\"sEcho\":\"{0}\",\"iTotalRecords\":\"{1}\",\"iTotalDisplayRecords\":\"{2}\",\"aaData\":{3}", sEcho, pageCount, pageCount, temp);
            _context.Response.Write("{" + json + "}");
            _context.Response.End();
            //_context.ApplicationInstance.CompleteRequest();
        }
        public void Response(String str)
        {
            _context.Response.ContentType = "text/plain";
            _context.Response.Clear();
            _context.Response.Write(str);
            _context.Response.End();
            //_context.ApplicationInstance.CompleteRequest();
        }
        public void Response(Object obj)
        {
            _context.Response.ContentType = "text/plain";
            _context.Response.Clear();
            _context.Response.Write(Json.ToStringEx(obj));
            _context.Response.End();
            //_context.ApplicationInstance.CompleteRequest();
        }
        public void ResponseResult()
        {
            _context.Response.ContentType = "text/plain";
            _context.Response.Clear();
            ht.Add("success", _Result.IsValid);
            if (_Result.IsValid)
            {
                if (!ht.ContainsKey("msg"))
                {
                    ht.Add("msg", "success");
                }
            }
            else
            {
                ht.Add("msg", _Result.Errors[0]);
            }
            _context.Response.Write(this.ToString());
            _context.Response.End();
            //_context.ApplicationInstance.CompleteRequest();
        }
        public string ResponseString()
        {
            ht.Add("success", _Result.IsValid);
            if (_Result.IsValid)
            {
                ht.Add("msg", "success");
            }
            else
            {
                ht.Add("msg", _Result.Errors[0]);
            }
            return this.ToString();
        }
    }
}

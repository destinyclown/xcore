using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Reflection;
namespace System
{
    /// <summary>
    /// 请求处理程序，请在WebConfig中设置由本程序接管请求
    /// 如
    /// <httpModules>
    ///     <add type="System.Rest" name="XCoreRest"/>
    /// </httpModules>
    /// 或在Global.asax中设置
    /// protected void Application_BeginRequest(object sender, EventArgs e)
    /// {
    ///     System.Rest.RunAction(Context);
    /// }
    /// </summary>
    public class Rest : IHttpModule, System.Web.SessionState.IRequiresSessionState
    {
        public void Dispose() { }
        private static string _DllName;
        /// <summary>
        /// Rest接口的DLL名称，不带后缀名（默认为Wlniao）
        /// cfgHelper.GetAppSettings("ActionDllName")
        /// </summary>
        internal static string DllName
        {
            get
            {
                if (string.IsNullOrEmpty(_DllName))
                {
                    _DllName = cfgHelper.GetAppSettings("ActionDllName").Replace(".DLL", "").Replace(".dll", "");
                    if (string.IsNullOrEmpty(_DllName))
                    {
                        _DllName = "Wlniao";
                    }
                } 
                return _DllName;
            }
        }
        private static string _NameSpace;
        /// <summary>
        /// Rest接口的命名空间（默认为空）
        /// cfgHelper.GetAppSettings("ActionNameSpace")
        /// </summary>
        internal static string NameSpace
        {
            get
            {
                if (string.IsNullOrEmpty(_NameSpace))
                {
                    _NameSpace = cfgHelper.GetAppSettings("ActionNameSpace");
                    if (string.IsNullOrEmpty(_NameSpace))
                    {
                        _NameSpace = ".";
                    }
                }
                return _NameSpace;
            }
        }
        public static void RunAction(HttpContext ctx)
        {
            if (ctx.Request.Path.ToLower().StartsWith("/rest"))
            {
                String method = ctx.Request.Path.Remove(0, 6).Replace('/', '\n').Trim().Replace('\n', '.');
                if (NameSpace == ".")
                {
                    if (!method.StartsWith(DllName) && method.Split('.').Length == 2)
                    {
                        method = DllName + "." + method;
                    }
                }
                else
                {
                    method = NameSpace + "." + method;
                }
                String classname = method.Substring(0, method.LastIndexOf('.'));        //获取类名
                String methodname = method.Substring(method.LastIndexOf('.') + 1);      //获取方法名
                RestAction action;          //声明一个方法
                try
                {
                    Type type = Type.GetType(String.Format("{0}, {1}", classname, DllName), false, true);
                    action = (RestAction)Activator.CreateInstance(type);
                    action.ctx = ctx;
                    type.InvokeMember(methodname, BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.IgnoreCase, null, action, new object[] { }).ToString();
                }
                catch(Exception ex)
                {
                    //System.Collections.Hashtable ht = new Collections.Hashtable();
                    //ht.Add("success", false);
                    //if (ex.InnerException != null)
                    //{
                    //    ht.Add("msg", ex.InnerException.Message);
                    //}
                    //else
                    //{
                    //    ht.Add("msg", ex.Message);
                    //}
                    //ctx.Response.Write(Json.ToStringEx(ht));
                }
                ctx.Response.End();
            }
        }


        public new void Init(HttpApplication application)
        {
            application.BeginRequest += new EventHandler(application_BeginRequest);
        }

        void application_BeginRequest(object sender, EventArgs e)
        {
            RunAction(HttpContext.Current);
        }


        /// <summary>
        /// 获取API提交的参数
        /// </summary>
        /// <param name="request">Request对象</param>
        /// <returns>参数数组</returns>
        private static System.Data.KeyValueData[] GetParamsFromRequest(HttpRequest request)
        {
            List<System.Data.KeyValueData> list = new List<System.Data.KeyValueData>();
            foreach (string key in request.QueryString.AllKeys)
            {
                list.Add(System.Data.KeyValueData.Create(key, request.QueryString[key]));
            }
            foreach (string key in request.Form.AllKeys)
            {
                list.Add(System.Data.KeyValueData.Create(key, request.Form[key]));
            }
            return list.ToArray();
        }
    }

    /// <summary>
    /// 需要通过统一Rest调用的方法的基类
    /// </summary>
    public class RestAction : System.Web.SessionState.IRequiresSessionState
    {
        Hashtable ht = new Hashtable();
        IDictionary htKeyValue = null;
        List<ArrayList> arraylist = new List<ArrayList>();

        private Result _Result = new Result();
        public Result Result
        {
            get { return _Result; }
            set { _Result.Join(value); }
        }
        private HttpContext _ctx;
        /// <summary>
        /// 参数集合
        /// </summary>
        public HttpContext ctx
        {
            get
            {
                if (_ctx == null)
                {
                    _ctx = HttpContext.Current;
                }
                return _ctx;
            }
            set { _ctx = value; }
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
        public void InitParam(IDictionary ht)
        {
            htKeyValue = ht;
        }
        public string GetParam(string key)
        {
            string var = string.Empty;
            if (htKeyValue == null)
            {
                if (string.IsNullOrEmpty(ctx.Request[key]))
                {
                    var = System.Web.HttpUtility.HtmlDecode(ctx.Request.QueryString[key]);
                }
                else
                {
                    var = System.Web.HttpUtility.HtmlDecode(ctx.Request[key]);
                }
            }
            else
            {
                try { var = htKeyValue[key].ToString(); }
                catch { }
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
            ctx.Response.ContentType = "text/plain";
            ctx.Response.Clear();
            ctx.Response.Write(this.ToString());
            ctx.Response.End();
        }
        public void Response(String sEcho, Int32 pageCount)
        {
            ctx.Response.ContentType = "text/plain";
            ctx.Response.Clear();
            string temp = Json.ToStringEx(arraylist);
            string json = string.Format("\"sEcho\":\"{0}\",\"iTotalRecords\":\"{1}\",\"iTotalDisplayRecords\":\"{2}\",\"aaData\":{3}", sEcho, pageCount, pageCount, temp);
            ctx.Response.Write("{" + json + "}");
            ctx.Response.End();
        }
        public void Response(String str)
        {
            ctx.Response.ContentType = "text/plain";
            ctx.Response.Clear();
            ctx.Response.Write(str);
            ctx.Response.End();
        }
        public void Response(Object obj)
        {
            ctx.Response.ContentType = "text/plain";
            ctx.Response.Clear();
            ctx.Response.Write(Json.ToStringEx(obj));
            ctx.Response.End();
        }
        public void ResponseResult()
        {
            ctx.Response.ContentType = "text/plain";
            ctx.Response.Clear();
            ht.Add("success", _Result.IsValid);
            ht.Add("msg", _Result.ErrorsText);
            ctx.Response.Write(this.ToString());
            ctx.Response.End();
        }
    }
}

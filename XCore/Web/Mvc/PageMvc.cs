//------------------------------------------------------------------------------
//	文件名称：WlniaoCMS\Mobirds.TemplateEngine\PageBase.cs
//	运 行 库：2.0.50727.1882
//	代码功能：页面基类
//	最后修改：2011年12月7日 23:35:52
//------------------------------------------------------------------------------
using System;
using System.IO;
using System.Collections.Generic;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Security.Cryptography;
namespace System.TemplateEngine
{
    /// <summary>
    /// 页面基类
    /// </summary>
    public class PageMvc : System.Web.Mvc.ViewPage, System.Web.SessionState.IRequiresSessionState
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(PageBase));
        private System.Data.KeyValueData[] parameters;
        /// <summary>
        /// 传递的参数集合
        /// </summary>
        internal System.Data.KeyValueData[] Params
        {
            get { return parameters; }
            set { parameters = value; }
        }
        /// <summary>
        /// 当前路径
        /// </summary>
        private string _currentroot;
        /// <summary>
        /// 模板文件名称
        /// </summary>
        private string _filename;
        /// <summary>
        /// 风格名称
        /// </summary>
        private static string _skinname;
        /// <summary>
        /// 风格名称
        /// </summary>
        private static string _skinroot;
        /// <summary>
        /// 当前请求
        /// </summary>
        private HttpContext _context;
        /// <summary>
        /// 当前请求
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal HttpContext ThisContext
        {
            get { return _context; }
            set { _context = value; }
        }
        private static RegexOptions options = RegexOptions.None;
        private static Regex regexTemplate = new Regex(@"<%template ([^\[\]\{\}\s]+)%>", options);
        private static Regex regexUIConfig = new Regex(@"{UI\.([^\[\]\{\}\s]+)}", options);
        private static Regex regexTagConfig = new Regex(@"{Tag\.([^\[\]\{\}\s]+)}", options);
        private static Regex regexCookiesConfig = new Regex(@"{Cookies\.([^\[\]\{\}\s]+)}", options);
        /// <summary>
        /// 风格名称
        /// </summary>
        internal String SkinName
        {
            get { return _skinname; }
            set { _skinname = value; }
        }
        /// <summary>
        /// 风格目录
        /// </summary>
        internal String SkinRoot
        {
            get { return _skinroot; }
            set { _skinroot = value; }
        }

        /// <summary>
        /// 获得参数对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal String ReadTemplate()
        {
            try
            {
                return ConvertUIConfig(GetTemplate(_currentroot, _filename), parameters);
            }
            catch { return "对不起，模板解析错误！"; }
        }
        /// <summary>
        /// 获取模板内容
        /// </summary>
        /// <param name="skinName">模板名</param>
        /// <param name="fileName">模板文件的文件名称</param>
        /// <returns></returns>
        public static string GetTemplate(string skinroot, string fileName)
        {
            using (StreamReader sr = new StreamReader(string.Format(@"{0}\{1}{2}", skinroot, fileName, TeConfig.Instance.TemplatePageSuffix), Encoding.UTF8))
            {
                StringBuilder temp = new StringBuilder(80000);
                temp.Append(sr.ReadToEnd());
                sr.Dispose();
                sr.Close();
                return IncludeTemplate(skinroot, temp.ToString());
            }
        }
        /// <summary>
        /// 获取模板内容
        /// </summary>
        /// <param name="fileName">模板文件的文件名称</param>
        /// <returns></returns>
        public static string GetTemplate(string fileName)
        {
            try
            {
                _skinname = HttpContext.Current.Application[HttpContext.Current.Request.UserHostAddress].ToString();
            }
            catch
            {
                _skinname = TeConfig.Instance.CurrentSkin;
                HttpContext.Current.Application.Add(HttpContext.Current.Request.UserHostAddress, _skinname);
            }
            string skinroot = PathHelper.Map(cfgHelper.FrameworkRoot) + "/" + TeConfig.Instance.TemplateFolder + "/" + _skinname + "/";
            using (StreamReader sr = new StreamReader(string.Format(@"{0}\{1}{2}", skinroot, fileName, TeConfig.Instance.TemplatePageSuffix), Encoding.UTF8))
            {
                StringBuilder temp = new StringBuilder(80000);
                temp.Append(sr.ReadToEnd());
                sr.Dispose();
                sr.Close();
                return ConvertUIConfig(IncludeTemplate(skinroot, temp.ToString()));
            }
        }
        /// <summary>
        /// 转唤标签
        /// </summary>
        /// <param name="skinName">模板目录</param>
        /// <param name="inputStr">模板内容</param>
        /// <param name="Inherits">引用的类名称</param>
        /// <returns></returns>
        private static string IncludeTemplate(string skinroot, string inputStr)
        {
            string strTemplate = inputStr;
            foreach (Match m in regexTemplate.Matches(strTemplate))
            {
                strTemplate = strTemplate.Replace(m.Groups[0].ToString(), GetTemplate(skinroot, m.Groups[1].ToString()));
            }
            return strTemplate.Replace("{FrameworkRoot}", cfgHelper.FrameworkRoot).Replace("{WebRoot}", cfgHelper.WebRoot);
        }
        private static Dictionary<String, LanguageSetting> getLangList(String path)
        {
            String[] files = Directory.GetFiles(path);
            if (files.Length == 0)
            {
                System.IO.FileEx.Write(path + "/core.config", "default=Hello World\n");
                files = Directory.GetFiles(path);
            }
            Dictionary<String, LanguageSetting> results = new Dictionary<String, LanguageSetting>();
            foreach (String file in files)
            {
                if (Path.GetExtension(file) != ".config") continue;
                String fileName = Path.GetFileNameWithoutExtension(file);
                Dictionary<String, String> _lang = cfgHelper.Read(file, '=');
                LanguageSetting lbl = new LanguageSetting(fileName, _lang);
                results.Add(fileName, lbl);
            }
            return results;
        }
        /// <summary>
        /// 转换UI标签
        /// </summary>
        /// <param name="inputStr">模板内容</param>
        /// <param name="Inherits">引用的类名称</param>
        /// <returns></returns>
        private static string ConvertUIConfig(string inputStr, params System.Data.KeyValueData[] pars)
        {
            string strTemplate = inputStr;
            foreach (Match m in regexUIConfig.Matches(strTemplate))
            {
                try
                {
                    string uivalue = UIConfig(m.Groups[1].ToString());
                    strTemplate = strTemplate.Replace(m.Groups[0].ToString(), uivalue);
                }
                catch
                {
                    logger.Info(string.Format("UI标签“{0}”未找到", m.Groups[1].ToString()));
                }
            }
            foreach (Match m in regexTagConfig.Matches(strTemplate))
            {
                try
                {
                    strTemplate = strTemplate.Replace(m.Value, ConverLable(m.Value, pars));
                }
                catch {
                    logger.Info(string.Format("标签“{Tag.{0}}”解析错误", m.Groups[1].ToString()));
                }
            }
            foreach (Match m in regexCookiesConfig.Matches(strTemplate))
            {
                try
                {
                    if (HttpContext.Current.Request.Cookies[m.Groups[1].ToString()] != null)
                    {
                        strTemplate = strTemplate.Replace(m.Value, strUtil.UrlDecode(HttpContext.Current.Request.Cookies[m.Groups[1].ToString()].Value));
                    }
                    else
                    {
                        strTemplate = strTemplate.Replace(m.Value, "");
                    }
                }
                catch {
                    logger.Info(string.Format("标签“{Cookies.{0}}”解析错误", m.Groups[1].ToString()));
                }
            }
            try
            {
                strTemplate = strTemplate.Replace("{PagerUI}", HttpContext.Current.Items["PagerUI"].ToString());
            }
            catch
            {
                strTemplate = strTemplate.Replace("{PagerUI}", string.Empty);
            }
            strTemplate = strTemplate.Replace("{WebRoot}", cfgHelper.WebRoot);
            _skinroot = string.Format(@"{0}{1}/{2}/", cfgHelper.FrameworkRoot, TeConfig.Instance.TemplateFolder, _skinname);
            strTemplate = strTemplate.Replace("{SkinRoot}", _skinroot);
            return strTemplate;
        }
        /// <summary>
        /// 转换UI标签
        /// </summary>
        /// <param name="inputStr">模板内容</param>
        /// <param name="Inherits">引用的类名称</param>
        /// <returns></returns>
        public static string UIConfig(string inputStr)
        {
            switch (inputStr)
            {
                case "SkinRoot":
                    return string.Format(@"{0}{1}/{2}/", cfgHelper.FrameworkRoot, TeConfig.Instance.TemplateFolder, _skinname);
                case "PagerUI":
                    return HttpContext.Current.Items["PagerUI"].ToString();
                default:
                    return lang.getBySkin(inputStr);;
            }
        }
        public string GetLabelType(string tagcontent)
        {
            int pos1 = tagcontent.IndexOf(']');
            int pos2 = tagcontent.LastIndexOf('[');
            int tempi = tagcontent.IndexOf("[");
            if (tagcontent.Length > 0 && pos1 > 1 && pos2 > 1)
            {
                string _Primary = tagcontent.Substring(tempi + 1, pos1 - 1);
                if (_Primary.IndexOf(",") > 0)
                {
                    string[] _mass_p = _Primary.Split(',');
                    int n = _mass_p.Length;
                    for (int i = 1; i < n; i++)
                    {
                        string s = _mass_p[i];
                        int pos = s.IndexOf('=');
                        if (pos < 0)
                            continue;
                        if (s.Substring(0, pos).Trim() == "XCY:LabelType")
                            return s.Substring(pos + 1).Trim();
                    }
                }
            }
            logger.Info("标签无有效的XCY:LabelType标记");
            throw new Exception("标签无有效的XCY:LabelType标记");
        }
        /// <summary>
        /// 转换Tag标签
        /// </summary>
        /// <param name="inputStr">模板内容</param>
        /// <param name="Inherits">引用的类名称</param>
        /// <returns></returns>
        private string ConverLable(HttpContext context, string inputStr)
        {
            string strTemplate = inputStr;            
            foreach (Match m in regexTagConfig.Matches(strTemplate))
            {
                try
                {
                    String tagcontent = TempInfo.Tags[m.Groups[1].ToString()];
                    String content="";             //方法执行结果
                    try
                    {
                        //Action action = new Action();
                        //action.Params = GetParamsFromRequest(context.Request);  //Post或Get的参数
                        //action.TagContent = tagcontent; //标签的完整内容
                        //action.ThisContext = context;   //传递当前请求以备用
                        Type type = Type.GetType(String.Format("XCenter.TemplateEngine.Builder.{0}, XCenter.Code", GetLabelType(tagcontent)), false, true);
                        Action builder = (Action)Activator.CreateInstance(type);
                        builder.Params = parameters;  //Post或Get的参数
                        builder.TagContent = tagcontent; //标签的完整内容
                        builder.ThisContext = context;   //传递当前请求以备用
                        builder.SkinRoot = _skinroot;    //传递当前模板资源的根目录
                        if (builder.ParseContent())      //解析标签内容
                            content = type.InvokeMember("TagConvert", BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.IgnoreCase, null, builder, new object[] { }).ToString();
                    }
                    catch{ }
                    strTemplate = strTemplate.Replace(m.Groups[0].ToString(), content);
                }
                catch {
                    logger.Info(string.Format("标签“{Tag.{0}}”解析错误", m.Groups[1].ToString()));
                }
            }
            try
            {
                string module = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(GetRequestParam("module")).Replace("Classcontent", "ClassContent");
                Type type = Type.GetType(String.Format("XCenter.TemplateEngine.Builder.{0}, XCenter.Code", module), false, true);
                Action builder = (Action)Activator.CreateInstance(type);
                builder.Params = parameters;  //Post或Get的参数
                builder.TagContent = strTemplate; //标签的完整内容
                builder.ThisContext = context;   //传递当前请求以备用
                builder.SkinRoot = _skinroot;    //传递当前模板资源的根目录
                if (builder.ParseContent())      //解析标签内容
                    strTemplate = type.InvokeMember("TagConvert", BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.IgnoreCase, null, builder, new object[] { }).ToString();
            }
            catch{}
            try
            {
                strTemplate = strTemplate.Replace("{PagerUI}", HttpContext.Current.Items["PagerUI"].ToString());
            }
            catch
            {
                strTemplate = strTemplate.Replace("{PagerUI}", string.Empty);
            }
            return strTemplate;
        }
        /// <summary>
        /// 获得参数对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public String GetRequestParam(string key)
        {
            if (parameters == null)
                return null;
            foreach (System.Data.KeyValueData p in parameters)
            {
                if (p.KeyName.ToLower() == key.ToLower())
                    return p.KeyValue;
            }
            return null;
        }
        /// <summary>
        /// 获取模板内容
        /// </summary>
        /// <param name="fileName">模板文件的文件名称</param>
        /// <returns></returns>
        public static string ConverLable(string inputStr, params System.Data.KeyValueData[] pars)
        {
            if (inputStr.IndexOf("Tag") > 0)
            {
                inputStr = inputStr.Substring(inputStr.IndexOf('.') + 1, inputStr.IndexOf('}') - inputStr.IndexOf('.') - 1);
                string skinroot = PathHelper.Map(cfgHelper.FrameworkRoot) + "/" + TeConfig.Instance.TemplateFolder + "/" + _skinname + "/";
                String content = "";             //方法执行结果
                try
                {
                    inputStr = TempInfo.Tags[inputStr];
                    string labeltype=string.Empty;
                    try
                    {
                        labeltype = new PageBase().GetLabelType(inputStr);
                    }
                    catch
                    {
                        return inputStr;
                    }
                    Type type = Type.GetType(String.Format("XCenter.TemplateEngine.Builder.{0}, XCenter.Code", labeltype), false, true);
                    Action builder = (Action)Activator.CreateInstance(type);
                    if (pars != null)
                    {
                        builder.Params = pars;
                    }
                    else
                    {
                        List<System.Data.KeyValueData> list = new List<System.Data.KeyValueData>();
                        foreach (string key in HttpContext.Current.Request.QueryString.AllKeys)
                        {
                            list.Add(System.Data.KeyValueData.Create(key, HttpContext.Current.Request.QueryString[key]));
                        }
                        foreach (string key in HttpContext.Current.Request.Form.AllKeys)
                        {
                            list.Add(System.Data.KeyValueData.Create(key, HttpContext.Current.Request.Form[key]));
                        }
                        builder.Params = list.ToArray();
                    }
                    builder.TagContent = inputStr; //标签的完整内容
                    builder.ThisContext = HttpContext.Current;   //传递当前请求以备用
                    builder.SkinRoot = _skinroot;    //传递当前模板资源的根目录
                    if (builder.ParseContent())      //解析标签内容
                    {
                        content = type.InvokeMember("TagConvert", BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.IgnoreCase, null, builder, new object[] { }).ToString();
                        return content;
                    }
                }
                catch{ }
            }
            return string.Empty;
        }
        /// <summary>
        /// 传入模板文件的地址
        /// </summary>
        /// <param name="templatefile"></param>
        /// <returns></returns>
        public string GetHtml(string templatefile)
        {
            _currentroot = System.IO.Path.GetDirectoryName(templatefile);
            _filename = System.IO.Path.GetFileNameWithoutExtension(templatefile);
            return ConverLable(_context, ReadTemplate());
        }
        protected String GetKeyValue(string key)
        {
            String value = System.Data.KvTableUtil.GetString(key);
            if (string.IsNullOrEmpty(value))
            {
                value = UIConfig(key);
            }
            return value;
        }


        #region 页面缓存
        /// <summary>
        /// 缓存文件的路径
        /// </summary>
        private string cacheFile = null;
        /// <summary>
        /// 缓存的配置内容
        /// </summary>
        private static string _cachePage;
        /// <summary>
        /// 是否缓存页面
        /// </summary>
        private static bool CachePage
        {
            get
            {
                if (string.IsNullOrEmpty(_cachePage))
                {
                    _cachePage = cfgHelper.GetAppSettings("CachePage").ToLower();
                    if (string.IsNullOrEmpty(_cachePage))
                    {
                        _cachePage = "false";
                    }
                }
                return _cachePage == "true";
            }
        }
        /// <summary>
        /// 页面是否强制关闭缓存功能
        /// </summary>
        protected bool ThisPageCache = CachePage;
        /// <summary>
        /// 当前页面缓存时间（单位：秒；默认：3600秒）
        /// </summary>
        protected int PageCacheTime = 3600;

        /// <summary>
        /// 将页面代码写入缓存文件
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            using (System.IO.StringWriter html = new System.IO.StringWriter())
            {
                base.Render(new System.Web.UI.HtmlTextWriter(html));
                string _html = html.ToString();
                if (!string.IsNullOrEmpty(cacheFile))
                {
                    file.Write(cacheFile, _html);
                }
                Response.Write(_html);
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            if (string.IsNullOrEmpty(Request["do"]) && ThisPageCache)
            {
                if (Request.QueryString.Count > 0)
                {
                    cacheFile = PathHelper.Map("~/xcenter/cache/" + (Request.Path + "_" + Request.QueryString[0]).Replace('/', '_'));
                }
                else
                {
                    cacheFile = PathHelper.Map("~/xcenter/cache/" + Request.Path.Replace('/', '_'));
                }
                System.IO.FileInfo fi = new System.IO.FileInfo(cacheFile);
                if (fi.Exists && fi.LastWriteTime.AddSeconds(PageCacheTime) > DateTime.Now)
                {
                    Response.Write(file.Read(cacheFile));
                    Response.End();
                }
            }
            base.OnLoad(e);
        }
        #endregion
    }
}
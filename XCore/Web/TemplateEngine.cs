//------------------------------------------------------------------------------
//	文件名称：WlniaoCMS\Core\TemplateEngine\PageBase.cs
//	运 行 库：2.0.50727.1882
//	代码功能：页面基类
//	最后修改：2011年10月11日 23:00:06
//------------------------------------------------------------------------------
using System;
using System.IO;
using System.Collections.Generic;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Security.Cryptography;
namespace System.Web
{
    public class PageBase : System.TemplateEngine.PageBase
    {
        /// <summary>
        /// AshxHelper对象
        /// </summary>
        protected AshxHelper helper = new AshxHelper(HttpContext.Current);
    }
    public class BaseHandle : System.Web.UI.Page
    {
        /// <summary>
        /// AshxHelper对象
        /// </summary>
        protected AshxHelper helper = new AshxHelper(HttpContext.Current);
    }
}
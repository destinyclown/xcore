//------------------------------------------------------------------------------
//	文件名称：XCore\Web\Mvc\Attribute.cs
//  版权所有：未来鸟软件
//  官方网站：http://www.wlniao.com

//	运 行 库：3.5
//	代码功能：Mvc属性扩展
//	最后修改：2015年3月11日 斜阳草树 QQ：63380609
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Web;
namespace System.Web.Mvc
{
    /// <summary>
    /// 自适应手机和PC网站
    /// PC访问的根路径为/，手机访问的根路径为/m
    /// </summary>
    public class Adapting : ActionFilterAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string path = HttpContext.Current.Request.RawUrl.ToLower();
            if (System.Runtime.Web.IsMobile)
            {
                if (path.StartsWith("/m/") || path.StartsWith("/m?") || path == "/m")
                {
                    base.OnActionExecuting(filterContext);
                }
                else
                {
                    filterContext.Result = new RedirectResult("/m" + path);
                }
            }
            else
            {
                if (path.StartsWith("/m/"))
                {
                    filterContext.Result = new RedirectResult(path.Replace("/m/", "/"));
                }
                else if (path.StartsWith("/m?"))
                {
                    filterContext.Result = new RedirectResult(path.Replace("/m?", "/?"));
                }
                else if (path == "/m")
                {
                    filterContext.Result = new RedirectResult("/");
                }
                else
                {
                    base.OnActionExecuting(filterContext);
                }
            }
        }
    }
    /// <summary>
    /// 显示正在维护中页面
    /// 如需自定义，请添加或修改根目录下的maintain.html文件
    /// </summary>
    public class Maintain : ActionFilterAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.Result = new MaintainResult();
        }
    }
}
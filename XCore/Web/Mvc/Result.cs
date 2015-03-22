//------------------------------------------------------------------------------
//	文件名称：XCore\Web\Mvc\Result.cs
//  版权所有：未来鸟软件
//  官方网站：http://www.wlniao.com

//	运 行 库：3.5
//	代码功能：Mvc返回的内容类型扩展
//	最后修改：2015年3月11日 斜阳草树 QQ：63380609
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace System.Web.Mvc
{
    /// <summary>
    /// 错误页面内容
    /// </summary>
    public class ErrorMsgResult : ActionResult
    {
        /// <summary>
        /// 页面标题
        /// </summary>
        private string Title;
        /// <summary>
        /// 页面内容
        /// </summary>
        private string Msg;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="title"></param>
        public ErrorMsgResult(string msg, string title = "错误!!!")
        {
            this.Msg = msg;
            this.Title = title;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var httpContext = HttpContext.Current;

            // MVC 3 running on IIS 7+
            if (HttpRuntime.UsingIntegratedPipeline)
            {
                httpContext.Response.Write(Runtime.Web.Page.Error(this.Title, this.Msg));
                httpContext.Response.End();
            }
            else
            {
                // Pre MVC 3
                httpContext.Response.Write(Runtime.Web.Page.Error(this.Title, this.Msg));
                httpContext.Response.End();
                //IHttpHandler httpHandler = new MvcHttpHandler();
                //httpHandler.ProcessRequest(httpContext);
            }
        }
    }
    /// <summary>
    /// Javascript跨域访问输出内容
    /// </summary>
    public class CallBackResult : ActionResult
    {
        private string Str;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        public CallBackResult(string str)
        {
            this.Str = str;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var httpContext = HttpContext.Current;

            // MVC 3 running on IIS 7+
            if (HttpRuntime.UsingIntegratedPipeline)
            {
                string callback = httpContext.Request.QueryString.Get("callback");
                httpContext.Response.Write(callback + "(" + this.Str + ")");
                httpContext.Response.End();
            }
            else
            {
                // Pre MVC 3
                string callback = httpContext.Request.QueryString.Get("callback");
                httpContext.Response.Write(callback + "(" + this.Str + ")");
                httpContext.Response.End();
            }
        }
    }
    /// <summary>
    /// 服务端跳转页面
    /// </summary>
    public class TransferResult : ActionResult
    {
        private string Url;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">目标地址</param>
        public TransferResult(string url)
        {
            this.Url = url;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var httpContext = HttpContext.Current;

            // MVC 3 running on IIS 7+
            if (HttpRuntime.UsingIntegratedPipeline)
            {
                httpContext.Server.TransferRequest(this.Url, true);
            }
            else
            {
                // Pre MVC 3
                httpContext.RewritePath(this.Url, false);

                IHttpHandler httpHandler = new MvcHttpHandler();
                httpHandler.ProcessRequest(httpContext);
            }
        }
    }

    /// <summary>
    /// 显示正在维护中页面
    /// 如需自定义，请添加或修改根目录下的maintain.html文件
    /// </summary>
    public class MaintainResult : ActionResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            var httpContext = HttpContext.Current;
            if (file.Exists(PathHelper.Map("/maintain.html")))
            {
                httpContext.Response.Clear();
                httpContext.Response.Write(file.Read(PathHelper.Map("/maintain.html")));
                httpContext.Response.ContentEncoding = System.Text.UTF8Encoding.UTF8;
                httpContext.Response.ContentType = "text/html";
                httpContext.Response.End();
            }
            else
            {
                httpContext.Response.Clear();
                httpContext.Response.Write(Runtime.Web.Page.Error("维护中", "页面正在维护中"));
                httpContext.Response.ContentEncoding = System.Text.UTF8Encoding.UTF8;
                httpContext.Response.ContentType = "text/html";
                httpContext.Response.End();
            }
        }
    }
}
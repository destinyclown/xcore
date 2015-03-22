//------------------------------------------------------------------------------
//	文件名称：XCore\Web\Mvc\ControllerExtend.cs
//  版权所有：未来鸟软件
//  官方网站：http://www.wlniao.com

//	运 行 库：3.5
//	代码功能：Mvc控制器扩展
//	最后修改：2015年3月11日 斜阳草树 QQ：63380609
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Web.Mvc
{
    /// <summary>
    /// 扩展后的Mvc控制器
    /// </summary>
    public class ControllerExtend : Controller
    {
        /// <summary>
        /// XCore：将一个对象输出为Json字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected ContentResult Json(Object obj)
        {
            return Content(System.Json.ToStringEx(obj));
        }
        /// <summary>
        /// XCore：将一个集合输出为Json字符串
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        protected ContentResult Json(Collections.IList list)
        {
            return Content(System.Json.ToStringListEx(list));
        }
        /// <summary>
        /// XCore：将一个集合输出为Json字符串
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        protected ContentResult ApiResult<T>(T t)
        {
            var rlt = new ApiResult<T>();
            rlt.success = true;
            rlt.message = "数据返回成功";
            rlt.data = t;
            return Content(System.Json.ToStringEx(rlt));
        }
        /// <summary>
        /// XCore：跨域请求时，快捷输出
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        protected CallBackResult CallBack(string content)
        {
            return new CallBackResult(content);
        }
        /// <summary>
        /// XCore：服务端重定向
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected TransferResult Transfer(String url)
        {
            return new TransferResult(url);
        }
    }
}

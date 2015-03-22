//------------------------------------------------------------------------------
//	文件名称：System\Web\CurrentRequest.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
using System.Web;
using System.Threading;
using System.Collections;
using System.Collections.Specialized;
namespace System.Web {
    /// <summary>
    /// 当前请求范围中的数据，方便静态方法调用
    /// </summary>
    public partial class CurrentRequest
    {
        public static object getItem(string key)
        {
            if (SystemInfo.IsWeb && HttpContext.Current != null)
                return HttpContext.Current.Items[key];
            else
                return Thread.GetData(Thread.GetNamedDataSlot(key));
        }
        public static void setItem( String key, object obj ) {
            if (SystemInfo.IsWeb && HttpContext.Current != null)
                HttpContext.Current.Items[key] = obj;
            else
                Thread.SetData( Thread.GetNamedDataSlot( key ), obj );
        }
        public IDictionary items;
        public String rawUrl;
        public String[] userLanguages;
        public String httpMethod;
        public NameValueCollection form;
        public static String getRawUrl() {
            if (getItem( "_RawUrl" ) != null) return getItem( "_RawUrl" ).ToString();
            if (HttpContext.Current == null) return "";
            return HttpContext.Current.Request.RawUrl;
        }
        public static String[] getUserLanguages() {
            if (getItem( "_UserLanguages" ) != null) return (String[])getItem( "_UserLanguages" );
            if (HttpContext.Current == null) return new String[] { };
            return HttpContext.Current.Request.UserLanguages;
        }
        public static String getHttpMethod() {
            if (getItem( "_HttpMethod" ) != null) return getItem( "_HttpMethod" ).ToString();
            if (HttpContext.Current == null) return "";
            return HttpContext.Current.Request.HttpMethod;
        }
        public static String getForm( String key ) {
            if (getItem( "_Form" ) != null) return ((NameValueCollection)getItem( "_Form" ))[key];
            if (HttpContext.Current == null) return "";
            return HttpContext.Current.Request.Form[key];
        }
        public static String getCookieKey() {
            return "XCenterCookie";
        }
        public static String getLangCookie() {
            return CookieGet( "lang" );
        }
        private static String CookieGet( String key ) {
            if (HttpContext.Current == null) return "";
            HttpCookie cookie = HttpContext.Current.Request.Cookies.Get( getCookieKey() );
            if (cookie == null) return "";
            return cookie[key];
        }
        public static void setCurrentPage( int current ) {
            setItem( "pageNumber", current );
        }
        public static int getCurrentPage( ) {
            return cvt.ToInt( getItem( "pageNumber" ) );
        }
    }
}
//------------------------------------------------------------------------------
//	文件名称：System\Result.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
using System.Web;
namespace System.Web {
    /// <summary>
    /// 系统的固定数据，比如网站根路径、app路径、主机名称等。
    /// 1) 可以在非web请求时候使用，比如定时器的新线程中使用;
    /// 2) 获取不同域名级别下的的统一主机值
    /// </summary>
    public partial class SystemInfo
    {
        public static SystemInfo Instance = loadSystemInfo();
        private SystemInfo() { }
        /// <summary>
        /// 比 ApplicationPath 多一个斜杠
        /// </summary>
        public static String RootPath { get { return SystemInfo.Instance.rootPath; } }
        /// <summary>
        /// 比如 /myapp 或者 /
        /// </summary>
        public static String ApplicationPath { get { return SystemInfo.Instance.applicationPath; } }
        /// <summary>
        /// 主机名 www.xiechaoyi.com 或 localhost 或 127.0.0.1
        /// </summary>
        public static String Host { get { return SystemInfo.Instance.host; } }
        /// <summary>
        /// 网站首页，普通模式是"/"，二级域名下是http加主机名
        /// </summary>
        public static String SiteRoot { get { return SystemInfo.Instance.siteRoot; } }
        /// <summary>
        /// 不带二级域名的 Host 名称，比如 abc.com 或 xcenter.cn
        /// </summary>
        public static String HostNoSubdomain { get { return SystemInfo.Instance.hostNoSubdomain; } }
        /// <summary>
        /// Host 是否是ip地址
        /// </summary>
        public static Boolean HostIsIp {
            get { return SystemInfo.Instance.hostIsIp; }
        }
        /// <summary>
        /// Host 是否等于 localhost
        /// </summary>
        public static Boolean HostIsLocalhost {
            get { return SystemInfo.Instance.hostIsLocalhost; }
        }
        internal static string OrmConfigPath = null;

        /// <summary>
        /// 主机名(或ip地址)+端口号
        /// </summary>
        public static String Authority { get { return SystemInfo.Instance.authority; } }
        private String rootPath;
        private String applicationPath;
        private String host;
        private String authority;
        private Boolean hostIsIp;
        private Boolean hostIsLocalhost;
        private String hostNoSubdomain;
        private String siteRoot;
        private static Boolean _initialized = false;
        private static Object _objLock = new Object();
        private static SystemInfo loadSystemInfo() {
            SystemInfo obj = new SystemInfo();
            if (IsWeb) {
                obj.applicationPath = HttpContext.Current.Request.ApplicationPath;
                obj.rootPath = addEndSlash( obj.applicationPath );
                obj.authority = HttpContext.Current.Request.Url.Authority;
                obj.host = HttpContext.Current.Request.Url.Host;
                obj.hostIsLocalhost = strUtil.EqualsIgnoreCase( obj.host, "localhost" );
                try
                {
                    Convert.ToInt32(obj.host.Split('.')[0]);
                    obj.hostIsIp = true;
                }
                catch
                {
                    obj.hostIsIp = false;
                }
                obj.hostNoSubdomain = getHostNoSubdomain( obj );
            }
            else {
                obj.applicationPath = "/";
                obj.rootPath = "/";
                obj.host = "localhost";
            }
            return obj;
        }
        private static String getHostNoSubdomain( SystemInfo obj ) {
            if (obj.hostIsIp || obj.hostIsLocalhost) {
                return obj.host;
            }
            else {
                return getHostNoSubdomain( obj.host );
            }
        }
        private static String getHostNoSubdomain( String host ) {
            int firstDotIndex = host.IndexOf( '.' );
            String result = host.Substring( firstDotIndex + 1, host.Length - firstDotIndex - 1 );
            if (result.IndexOf( '.' ) < 0) return host;
            return result;
        }
        private static String addEndSlash( String appPath ) {
            if (!appPath.EndsWith( "/" )) return appPath + "/";
            return appPath;
        }
        private static String _ProcessName = null;
        /// <summary>
        /// 是否Web应用
        /// </summary>
        public static Boolean IsWeb
        {
            get
            {
                try
                {
                    if (string.IsNullOrEmpty(_ProcessName))
                    {
                        _ProcessName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
                        if (_ProcessName == "WebDev.WebServer20")
                        {
                            _ProcessName = "w3wp";
                        }
                    }
                    return _ProcessName.Contains("w3wp");   //是否在IIS进程中运行
                }
                catch
                {
                    return true;
                }
            }
        }
        /// <summary>
        /// 当前是否为Windows系统
        /// </summary>
        public static Boolean IsWindows {
            get { try { return Environment.OSVersion.VersionString.ToLower().IndexOf("windows") >= 0; } catch { return true; } }
        }
        public static void UpdateSessionId() {
            String sessionId = getSessionId();
            if (sessionId != null) updateCookie( sessionId );
        }
        private static String getSessionId() {
            String sessionId = "ASPNET_SESSIONID";
            HttpRequest req = HttpContext.Current.Request;
            if (req.Form[sessionId] != null) return req.Form[sessionId];
            if (req.QueryString[sessionId] != null) return req.QueryString[sessionId];
            return null;
        }
        private static void updateCookie( String sessionId ) {
            String cookieName = "ASP.NET_SESSIONID";
            HttpRequest req = HttpContext.Current.Request;
            HttpResponse res = HttpContext.Current.Response;
            HttpCookie cookie = req.Cookies.Get( cookieName );
            if (cookie == null) {
                res.Cookies.Add( new HttpCookie( cookieName, sessionId ) );
            }
            else {
                cookie.Value = sessionId;
                req.Cookies.Set( cookie );
            }
        }
    }
}
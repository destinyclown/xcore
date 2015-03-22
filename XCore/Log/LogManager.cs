//------------------------------------------------------------------------------
//	文件名称：System\Log\LogManager.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
using System.Reflection;
using System.Web;
using System.IO;
using System.Log;
namespace System
{
    /// <summary>
    /// 日志管理对象，通常用于获取日志工具
    /// </summary>
    /// <example>
    /// 一般在类的第一行定义
    /// <code>
    /// private static readonly ILog logger = LogManager.GetLogger( typeof( ObjectBase ) );
    /// </code>
    /// 然后可以在其他方法中使用
    /// <code>
    /// logger.Info( "your message" );
    /// </code>
    /// </example>
    public class LogManager
    {
        private LogManager() { }
        /// <summary>
        /// 获取一个日志工具
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <returns>返回日志工具</returns>
        public static ILog GetLogger(Type type)
        {
            return GetLogger(type.FullName);
        }
        /// <summary>
        /// 获取一个日志工具
        /// </summary>
        /// <param name="typeName">对象类型</param>
        /// <returns>返回日志工具</returns>
        public static ILog GetLogger(String typeName)
        {
            ILog log = getLogger();
            log.TypeName = typeName;
            return log;
        }
        /// <summary>
        /// 获取一个日志工具
        /// </summary>
        /// <param name="typeName">对象类型</param>
        /// <returns>返回日志工具</returns>
        public static ILog GetLogger()
        {
            ILog log = getLogger();
            log.TypeName = "Unknown Class";
            return log;
        }
        private static ILog getLogger()
        {
            if (LogConfig.Instance.Level == LogLevel.None)
                return new NullLogger();
            if (strUtil.IsNullOrEmpty(LogConfig.Instance.LoggerImpl))
                return new FileLogger();
            ILog log = null;
            String loggerImpl = LogConfig.Instance.LoggerImpl;
            if (strUtil.HasText(loggerImpl))
            {
                String[] strArray = loggerImpl.Split(new char[] { ',' });
                if (strArray.Length == 1)
                {
                    Type type = Type.GetType(strArray[0].Trim());
                    if (type != null)
                    {
                        log = rft.GetInstance(type) as ILog;
                    }
                    return log;
                }
                if (strArray.Length == 2)
                {
                    log = Assembly.Load(strArray[1].Trim()).CreateInstance(strArray[0].Trim()) as ILog;
                }
            }
            return log;
        }
        /// <summary>
        /// 立即将日志内容写入磁盘(在 web 中，日志是在页面请求结束的时候才一起写入到磁盘的)
        /// </summary>
        public static void Flush()
        {
            if (!SystemInfo.IsWeb) return;
            LoggerUtil.Flush();
        }
    }
}


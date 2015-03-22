//------------------------------------------------------------------------------
//	文件名称：System\Log\LogConfig.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web;
namespace System.Log {
    /// <summary>
    /// 日志配置文件，默认配置文件在 /framework/config/log.config，日志文件在 /framework/log/log.txt 中
    /// </summary>
    public class LogConfig {
        /// <summary>
        /// 日志配置信息(全局缓存)
        /// <remarks>
        /// logLevel 的值(不区分大小写)：none, debug, info, warn, error, fatal, all；
        /// logFile 和 logProvider 通常不用填写
        /// </remarks>
        /// <example>
        /// 配置文件的格式(一行一条配置，键值之间用冒号分开)。
        /// <code>
        /// dirPath : log
        /// logLevel : info
        /// logProvider : System.Log.FileLogger
        /// inRealTime : true
        /// </code>
        /// </example>
        /// </summary>
        public static readonly LogConfig Instance = new LogConfig();
        private LogConfig() {
            String absPath = getConfigAbsPath();
            if (strUtil.IsNullOrEmpty( absPath )) {
                loadDefault();
                return;
            }
            Dictionary<String, String> dic = cfgHelper.Read(absPath);
            this.DirPath = getDirPath(dic);
            this.Level = getLevel( dic );
            this.LoggerImpl = getLoggerImpl( dic );
            this.InRealTime = getInRealTime( dic );

        }

        //--------------------------------------------------------------------------------------

        private LogLevel getLevel( Dictionary<String, String> dic ) {

            String level;
            dic.TryGetValue( "logLevel", out level );

            if (strUtil.IsNullOrEmpty( level )) return getDefaultLevel();

            try {
                return (LogLevel)Enum.Parse( typeof( LogLevel ), level, true );
            }
            catch { return getDefaultLevel(); }
        }

        private static LogLevel getDefaultLevel() {
            return LogLevel.None;
        }

        //--------------------------------------------------------------------------------------


        private void loadDefault()
        {
            String absPath = getConfigAbsPath();
            this.Level = LogLevel.Error;
            this.DirPath = getAbsoluteLogPath(getDefaultDirPath());
            this.InRealTime = true;
        }

        //----------------------------- 配置的路径 ----------------------------------------------------------------

        private static String getConfigAbsPath() {
            String absolutePath = PathHelper.Map(strUtil.Join(cfgHelper.ConfigRoot, "log.config"));
            if (!File.Exists(absolutePath))
            {
                Dictionary<String, String> dic = new Dictionary<string, string>();
                dic.Add("dirPath", "log");
                dic.Add("logLevel", "error");
                dic.Add("inRealTime", "true");
                cfgHelper.Write(dic, absolutePath);
            }
            return absolutePath;
        }


        //------------------------------ 日志的路径 ---------------------------------------------------------------

        private static String getDirPath(Dictionary<String, String> dic)
        {
            String dirPath;
            dic.TryGetValue("dirPath", out dirPath);
            if (strUtil.IsNullOrEmpty(dirPath)) dirPath = getDefaultDirPath();
            return getAbsoluteLogPath(dirPath);
        }

        private static String getDefaultDirPath()
        {
            return cfgHelper.FrameworkRoot + "log";
        }

        private static String getAbsoluteLogPath( String path ) {

            if (path.StartsWith( cfgHelper.FrameworkRoot ) == false)
                path = strUtil.Join( cfgHelper.FrameworkRoot, path );

            return PathHelper.Map( path );
        }

        //---------------------------------------------------------------------------------------------

        private static String getLoggerImpl( Dictionary<String, String> dic ) {
            String logProvider;
            dic.TryGetValue( "logProvider", out logProvider );
            return logProvider;
        }
        private static Boolean getInRealTime(Dictionary<String, String> dic)
        {
            String inRealTime;
            dic.TryGetValue("inRealTime", out inRealTime);
            return inRealTime == "true";
        }

        //---------------------------------------------------------------------------------------------
		
		private LogLevel _Level;
        /// <summary>
        /// 记录的层次，不区分大小写，有 none, debug, info, warn, error, fatal, all 这几种可选
        /// </summary>
		public LogLevel Level { get{return _Level;} set{_Level=value;} }
		
		private String _DirPath;
        /// <summary>
        /// 日志文件存储的路径
        /// </summary>
        public String DirPath { get { return _DirPath; } set { _DirPath = value; } }
		
		private String _LoggerImpl;
        /// <summary>
        /// 日志记录工具，默认是 FileLogger
        /// </summary>
		public String LoggerImpl { get{return _LoggerImpl;} set{_LoggerImpl=value;} }
		
		private Boolean _InRealTime;
        /// <summary>
        /// 是否实时输出日志信息
        /// </summary>
		public Boolean InRealTime { get{return _InRealTime;} set{_InRealTime=value;} }

    }

}

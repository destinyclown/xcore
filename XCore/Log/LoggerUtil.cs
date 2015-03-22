//------------------------------------------------------------------------------
//	文件名称：System\Log\LoggerUtil.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
using System.IO;
using System.Web;
using System.Collections.Generic;
using System.Text;
namespace System.Log {
    /// <summary>
    /// 日志处理工具
    /// </summary>
    public partial class LoggerUtil
    {
        private static Object objLock = new object();
        /// <summary>
        /// sql 日志的前缀
        /// </summary>
        public static readonly String SqlPrefix = "sql=";
        /// <summary>
        /// 将日志写入磁盘
        /// </summary>
        /// <param name="msg"></param>
        public static void WriteFile( ILogMsg msg ) {
            if (!SystemInfo.IsWeb || LogConfig.Instance.InRealTime)
            {
                writeFilePrivate(msg);
                return;
            }
            StringBuilder sb = CurrentRequest.getItem( "currentLogList" ) as StringBuilder;
            if (sb == null) {
                sb = new StringBuilder();
                CurrentRequest.setItem( "currentLogList", sb );
            }
            sb.AppendFormat( "{0} {1} {2} - {3} \r\n", msg.LogTime, msg.LogLevel, msg.TypeName, msg.Message );
        }
        /// <summary>
        /// 将日志写入磁盘
        /// </summary>
        /// <param name="msg"></param>
        public static void WriteFileNow(ILogMsg msg)
        {
            writeFilePrivate(msg);
            return;
        }
        /// <summary>
        /// 在 web 系统中，记录 sql 执行的次数
        /// </summary>
        public static void LogSqlCount()
        {
            if (CurrentRequest.getItem("sqlcount") == null)
            {
                CurrentRequest.setItem("sqlcount", 1);
            }
            else
            {
                CurrentRequest.setItem("sqlcount", ((int)CurrentRequest.getItem("sqlcount")) + 1);
            }
        }
        private static void writeFilePrivate( ILogMsg msg ) {
            String formatMsg = GetFormatMsg( msg );
            writeContentToFile(msg,formatMsg);
        }
        private static void writeContentToFile(String formatMsg ) {
            String logFilePath = LogConfig.Instance.DirPath;
            lock (objLock)
            {
                if (!System.IO.Directory.Exists(logFilePath))
                {
                    System.IO.Directory.CreateDirectory(logFilePath);
                }
                logFilePath = logFilePath + "log.txt";
                if (System.IO.File.Exists(logFilePath))
                {
                    DateTime lastAccessTime = System.IO.File.GetLastWriteTime(logFilePath);
                    DateTime now = DateTime.Now;
                    if (cvt.IsDayEqual(lastAccessTime, now))
                    {
                        System.IO.FileEx.Append(logFilePath, formatMsg);
                    }
                    else
                    {
                        String destFileName = getDestFileName(logFilePath);
                        System.IO.FileEx.Move(logFilePath, destFileName);
                        System.IO.FileEx.Write(logFilePath, formatMsg);
                    }
                }
                else
                {
                    System.IO.FileEx.Write(logFilePath, formatMsg);
                }
            }
        }
        private static void writeContentToFile(ILogMsg msg, String formatMsg ) {
            String logFilePath = PathTool.JoinPath(LogConfig.Instance.DirPath, strUtil.GetTitleCase(msg.LogLevel));
            lock (objLock)
            {
                if (!System.IO.Directory.Exists(logFilePath))
                {
                    System.IO.Directory.CreateDirectory(logFilePath);
                }
                logFilePath = logFilePath + "log.txt";
                if (System.IO.File.Exists(logFilePath))
                {
                    DateTime lastAccessTime = System.IO.File.GetLastWriteTime(logFilePath);
                    DateTime now = DateTime.Now;
                    if (cvt.IsDayEqual(lastAccessTime, now))
                    {
                        System.IO.FileEx.Append(logFilePath, formatMsg);
                    }
                    else
                    {
                        String destFileName = getDestFileName(logFilePath);
                        System.IO.FileEx.Move(logFilePath, destFileName);
                        System.IO.FileEx.Write(logFilePath, formatMsg);
                    }
                }
                else
                {
                    System.IO.FileEx.Write(logFilePath, formatMsg);
                }
            }
        }
        private static String getDestFileName( string logFilePath ) {
            String ext = Path.GetExtension( logFilePath );
            String pathWithoutExt = strUtil.TrimEnd( logFilePath, ext );
            return pathWithoutExt + "_" + DateTime.Now.Subtract( TimeSpan.FromDays( 1 ) ).ToString( "yyyy.MM.dd" ) + ext;
        }
        public static String GetFormatMsg( ILogMsg logMsg ) {
            return String.Format("{0} {1} {2} - {3} \r\n", logMsg.LogTime.ToString("HH:mm:ss"), strUtil.GetTitleCase(logMsg.LogLevel), logMsg.TypeName, logMsg.Message);
        }
        /// <summary>
        /// 将所有日志即可写入磁盘
        /// </summary>
        internal static void Flush() {
            StringBuilder sb = CurrentRequest.getItem( "currentLogList" ) as StringBuilder;
            if (sb != null)
                writeContentToFile( sb.ToString() );
        }

        //获取代码行号的函数
        public static int GetLineNum()
        {
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(1, true);
            return st.GetFrame(0).GetFileLineNumber();
        }
        //获取代码文件的函数
        public static string GetFileName()
        {
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(1, true);
            return st.GetFrame(0).GetFileName();
        }
        //获取代码方法的函数
        public static string GetMethod()
        {
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(1, true);
            return st.GetFrame(0).GetMethod().Name;
        }
    }
}
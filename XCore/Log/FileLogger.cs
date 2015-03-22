//------------------------------------------------------------------------------
//	文件名称：System\Log\FileLogger.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
using System.Diagnostics;
namespace System.Log {

    /// <summary>
    /// 文件日志工具，所有日志会被写入磁盘
    /// </summary>
    internal partial class FileLogger : ILog
    {

        private LogLevel _levelSetting;
        private LogMessage _msg;

        public FileLogger() {
            _levelSetting = LogConfig.Instance.Level;
            _msg = new LogMessage();
        }

        public void Debug( String message ) {
            if (_levelSetting >= LogLevel.Debug)
            {
                _msg.LogTime = DateTime.Now;
                _msg.Message = message;
                _msg.LogLevel = "debug";
                System.Diagnostics.Debug.Write(LoggerUtil.GetFormatMsg(_msg));
                LoggerUtil.WriteFile( _msg );
            }
        }

        public void Info(String message)
        {
            if (_levelSetting >= LogLevel.Info)
            {
                if (message.StartsWith(LoggerUtil.SqlPrefix))
                {
                    message = strUtil.TrimStart(message, LoggerUtil.SqlPrefix);
                    LoggerUtil.LogSqlCount();
                }
                _msg.LogTime = DateTime.Now;
                _msg.Message = message;
                _msg.LogLevel = "info";
                System.Diagnostics.Debug.Write(LoggerUtil.GetFormatMsg(_msg));
                LoggerUtil.WriteFile(_msg);
            }

        }
        public void Warn( String message ) {
            if (_levelSetting >= LogLevel.Warn)
            {
                _msg.LogTime = DateTime.Now;
                _msg.Message = message;
                _msg.LogLevel = "warn";
                System.Diagnostics.Debug.Write(LoggerUtil.GetFormatMsg(_msg));
                LoggerUtil.WriteFile( _msg );
            }
        }

        public void Error( String message ) {
            if (_levelSetting >= LogLevel.Error)
            {
                _msg.LogTime = DateTime.Now;
                _msg.Message = message;
                _msg.LogLevel = "error";
                System.Diagnostics.Debug.Write(LoggerUtil.GetFormatMsg(_msg));
                LoggerUtil.WriteFile( _msg );
            }
        }

        public void Fatal( String message ) {
            if (_levelSetting >= LogLevel.Fatal)
            {
                _msg.LogTime = DateTime.Now;
                _msg.Message = message;
                _msg.LogLevel = "fatal";
                System.Diagnostics.Debug.Write(LoggerUtil.GetFormatMsg(_msg));
                LoggerUtil.WriteFile( _msg );
            }
        }
        /// <summary>
        /// 记录SQL语句执行情况信息日志
        /// </summary>
        /// <param name="sql"></param>
        public void Sql(String message)
        {
            if (_levelSetting >= LogLevel.Debug)
            {
                _msg.LogTime = DateTime.Now;
                _msg.Message = message;
                _msg.LogLevel = "sql";
                System.Diagnostics.Debug.Write(LoggerUtil.GetFormatMsg(_msg));
                LoggerUtil.WriteFile(_msg);
            }
        }
        public void Code(string file, int line)
        {
            _msg.LogTime = DateTime.Now;
            _msg.Message = string.Format(" \r\nCodeFile:{0} \r\nCodeLine:{1} \r\n", file, line);
            _msg.LogLevel = "code";
            System.Diagnostics.Debug.Write(LoggerUtil.GetFormatMsg(_msg));
            LoggerUtil.WriteFileNow(_msg);
        }

        public String TypeName {
            set {
                _msg.TypeName = value;
            }
        }


    }
}


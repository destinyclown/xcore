//------------------------------------------------------------------------------
//	文件名称：System\Log\LogLevel.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
namespace System.Log {
    /// <summary>
    /// 日志的等级
    /// </summary>
    public enum LogLevel {
        /// <summary>
        /// 无等级
        /// </summary>
        None,
        /// <summary>
        /// 崩溃信息日志
        /// </summary>
        Fatal,
        /// <summary>
        /// 错误信息日志
        /// </summary>
        Error,
        /// <summary>
        /// 警告信息日志
        /// </summary>
        Warn,
        /// <summary>
        /// 调试信息日志
        /// </summary>
        Debug,
        /// <summary>
        /// 普通信息日志
        /// </summary>
        Info,
        /// <summary>
        /// 全部等级
        /// </summary>
        All
    }
}


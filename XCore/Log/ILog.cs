//------------------------------------------------------------------------------
//	�ļ����ƣ�System\Log\ILog.cs
//	�� �� �⣺2.0.50727.1882
//	����޸ģ�2012��9��8�� 22:15:20
//------------------------------------------------------------------------------
using System;
namespace System
{
    /// <summary>
    /// ��־�ӿ�
    /// </summary>
    public partial interface ILog
    {
        /// <summary>
        /// 0����ͨ��Ϣ��־
        /// </summary>
        /// <param name="message"></param>
        void Info(String message);
        /// <summary>
        /// 1��������Ϣ��־
        /// </summary>
        /// <param name="message"></param>
        void Debug(String message);
        /// <summary>
        /// 2��������Ϣ��־
        /// </summary>
        /// <param name="message"></param>
        void Warn(String message);
        /// <summary>
        /// 3��������Ϣ��־
        /// </summary>
        /// <param name="message"></param>
        void Error(String message);
        /// <summary>
        /// 4��������Ϣ��־
        /// </summary>
        /// <param name="message"></param>
        void Fatal(String message);
        /// <summary>
        /// ��¼����ִ�������Ϣ��־
        /// </summary>
        /// <param name="message"></param>
        void Code(String file, Int32 line);
        /// <summary>
        /// ��¼SQL���ִ�������Ϣ��־
        /// </summary>
        /// <param name="sql"></param>
        void Sql(String sql);
        /// <summary>
        /// �����־����������
        /// </summary>
        String TypeName { set; }
    }
}
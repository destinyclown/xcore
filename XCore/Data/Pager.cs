using System;
using System.Collections.Generic;
using System.Text;

namespace System.Data
{
    /// <summary>
    /// 分页数据模型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Pager<T>
    {

        /// <summary>
        /// 结果总数
        /// </summary>
        public int total { get; set; }

        /// <summary>
        /// 分页大小
        /// </summary>
        public int size { get; set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        public int index { get; set; }

        public List<T> rows { get; set; }
    }
}

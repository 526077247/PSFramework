using System;
using System.Collections.Generic;
using System.Text;

namespace service.core
{
    /// <summary>
    /// 数据列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataList<T>
    {
        /// <summary>
        /// 总数
        /// </summary>
        public int total { get; set; }
        /// <summary>
        /// 当前起始位置
        /// </summary>
        public int start { get; set; }
        /// <summary>
        /// 分页大小
        /// </summary>
        public int pageSize { get; set; }
        /// <summary>
        /// 列表
        /// </summary>
        public List<T> list { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public DataList()
        {
            list = new List<T>();
        }
    }
}

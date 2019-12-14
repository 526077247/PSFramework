using System;
using System.Collections.Generic;
using System.Text;

namespace service.core
{
    public class Result
    {
        /// <summary>
        /// 代码
        /// </summary>
        public int code { set; get; }
        /// <summary>
        /// 信息
        /// </summary>
        public string msg { set; get; }
        /// <summary>
        /// 数据
        /// </summary>
        public object data { set; get; }
    }
}

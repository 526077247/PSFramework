using System;
using System.Collections.Generic;
using System.Text;

namespace service.core
{
    public class PublishMethodAttribute : Attribute
    {

    }

    public class CheckLoginAttribute : Attribute
    {

    }

    public class AutoLogAttribute : Attribute
    {
        public string Level;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="level">"ALL,INFO,ERROR"</param>
        public AutoLogAttribute(string level)
        {
            Level = level;
        }
    }
    public class PublishName: Attribute
    {
        public string SvrId;
        public PublishName(string _svrId)
        {
            SvrId = _svrId;
        }
    }
}

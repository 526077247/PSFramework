using Service.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace sso.service
{
    public class BasicUserInfo:DataBase
    {
        
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}

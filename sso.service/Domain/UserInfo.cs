using System;
using System.Collections.Generic;
using System.Text;

namespace sso.service
{
    public class UserInfo: UserDetails
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Psw { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; } 

    }
}

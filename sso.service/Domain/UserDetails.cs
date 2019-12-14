using System;
using System.Collections.Generic;
using System.Text;

namespace sso.service
{
    public class UserDetails: BasicUserInfo
    {
        /// <summary>
        /// 用户登录名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 电话号码
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Mail { get; set; }
    }
}

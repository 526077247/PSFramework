using service.core;
using System;
using System.Collections.Generic;
using System.Text;

namespace sso.service
{
    public class LoginResult : DataBase
    {
        /// <summary>
        /// 会话标识
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 账号名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 上线时间
        /// </summary>
        public DateTime LoginTime { get; set; }
        /// <summary>
        /// 有效时间
        /// </summary>
        public int Effective { get; set; }
    }
}

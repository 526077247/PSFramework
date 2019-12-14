
using service.core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace sso.service
{
    public interface IUserInfoMgeSvr: IAppServiceBase
    {
        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nickName"></param>
        /// <param name="psw"></param>
        /// <returns></returns>
        [PublishMethod]
        LoginResult RegisterUser(string name, string nickName, string psw);

        /// <summary>
        /// 检测用户名是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [PublishMethod]
        bool CheckUserName(string name);

        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        [PublishMethod]
        LoginResult RegisterUserDetails(UserInfo userInfo);

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="user"></param>
        /// <param name="oldPsw"></param>
        /// <param name="newPsw"></param>
        /// <returns></returns>
        [PublishMethod]
        bool ChangePsw(string user, string oldPsw, string newPsw);

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [CheckLogin]
        [PublishMethod]
        UserDetails GetUser(string token);

        /// <summary>
        /// 修改昵称电话邮箱信息
        /// </summary>
        /// <param name="token"></param>
        /// <param name="tel"></param>
        /// <param name="mail"></param>
        /// <returns></returns>
        [CheckLogin]
        [PublishMethod]
        UserDetails UpdateUser(string token,string nickName="", string tel = "", string mail = "");
    }
}

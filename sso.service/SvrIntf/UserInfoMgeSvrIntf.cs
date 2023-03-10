
using Service.Core;
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
        /// 检测邮箱是否存在
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        [PublishMethod]
        bool CheckEMail(string mail);
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
        /// <param name="nickName"></param>
        /// <param name="tel"></param>
        /// <param name="mail"></param>
        /// <returns></returns>
        [CheckLogin]
        [PublishMethod]
        UserDetails UpdateUser(string token,string nickName="", string tel = "", string mail = "");

        /// <summary>
        /// 忘记密码通过邮箱找回
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type">0邮箱找回</param>
        /// <returns></returns>
        [PublishMethod]
        bool ForgetPswForEMail(string name, int type = 0);

        /// <summary>
        /// 通过ticket找回密码
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="newPsw"></param>
        /// <returns></returns>
        [PublishMethod]
        bool ChangePswByTicket(string ticket,string newPsw);

    }
}

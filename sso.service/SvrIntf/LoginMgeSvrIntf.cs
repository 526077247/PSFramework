using System;
using System.Collections.Generic;
using System.Text;
using Service.Core;
namespace sso.service
{
    public interface ILoginMgeSvr:IAppServiceBase
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="user"></param>
        /// <param name="psw"></param>
        /// <returns></returns>
        [PublishMethod]
        [AutoLog("ALL")]
        LoginResult Login(string user, string psw);

        /// <summary>
        /// 刷新会话
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [PublishMethod]
        LoginResult Refresh(string token);

        /// <summary>
        /// 下线
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [PublishMethod]
        bool Logout(string token);

        /// <summary>
        /// 获取一次性授权码
        /// </summary>
        /// <param name="user"></param>
        /// <param name="psw"></param>
        /// <returns></returns>
        [PublishMethod]
        string GetAuthorizationCode(string user, string psw);

        /// <summary>
        /// 通过授权码登录
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [PublishMethod]
        LoginResult LoginByCode(string code);

        /// <summary>
        /// 获取登录信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [PublishMethod]
        LoginResult GetLoginInfo(string token);
    }
}

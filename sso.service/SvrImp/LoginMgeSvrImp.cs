using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using service.core;
namespace sso.service
{
    public class LoginMgeSvr:AppServiceBase,ILoginMgeSvr
    {
        #region 服务描述

        private IDaoManager daoManager = null;
        private IUserInfoDao _UserInfoDao = null;
        private ICacheManager cacheManager = null;
        private ICacheMgeSvr _CacheMgeSvr = null;
        public LoginMgeSvr() : base()
        {
            daoManager = (IDaoManager)ServiceManager.GetService(typeof(IDaoManager));
            cacheManager = (ICacheManager)ServiceManager.GetService(typeof(ICacheManager));
            _UserInfoDao = (IUserInfoDao)daoManager.GetDao<IUserInfoDao>();
            _CacheMgeSvr = cacheManager.GetCache("LoginResult");
        }

        #endregion


        #region ILoginMgeSvr函数

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="user"></param>
        /// <param name="psw"></param>
        /// <returns></returns>
        [PublishMethod]
        public LoginResult Login(string user, string psw)
        {
            LoginResult result;
            Hashtable para = new Hashtable();
            para.Add("Name", user);
            para.Add("Psw", MD5Helper.GetMD5HashString(psw));
            object obj = _UserInfoDao.GetByPara(para);
            if (obj != null)
            {
                UserInfo userInfo = obj as UserInfo;
                result = new LoginResult
                {
                    Token = Utils.GuidToString(),
                    LoginTime = DateTime.Now,
                    Name = userInfo.Name,
                    NickName = userInfo.NickName
                };
                if(!_CacheMgeSvr.Put(result.Token, result))
                    throw new Exception("服务器繁忙请稍后再试");
            }
            else
                return new LoginResult();
            return result;
        }

        /// <summary>
        /// 刷新会话
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [PublishMethod]
        public LoginResult Refresh(string token)
        {
            LoginResult result;
            result = _CacheMgeSvr.Get<LoginResult>(token);
            if (result != null)
            {
                if(!_CacheMgeSvr.Put(token, result))
                    throw new Exception("服务器繁忙请稍后再试");
            }
            else
                return new LoginResult();
            return result;
        }

        /// <summary>
        /// 下线
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [PublishMethod]
        public bool Logout(string token)
        {
            object obj = _CacheMgeSvr.Get<LoginResult>(token);
            if (obj != null)
            {
                return _CacheMgeSvr.Delete(token);
            }
            return true;
        }

        /// <summary>
        /// 获取一次性授权码
        /// </summary>
        /// <param name="user"></param>
        /// <param name="psw"></param>
        /// <returns></returns>
        [PublishMethod]
        public string GetAuthorizationCode(string user, string psw)
        {
            string code;
            Hashtable para = new Hashtable();
            para.Add("Name", user);
            para.Add("Psw", MD5Helper.GetMD5HashString(psw));
            object obj = _UserInfoDao.GetByPara(para);
            if (obj != null)
            {
                UserInfo userInfo = obj as UserInfo;
                LoginResult result = new LoginResult
                {
                    Token = Utils.GuidToString(),
                    LoginTime = DateTime.Now,
                    Name = userInfo.Name,
                    NickName = userInfo.NickName
                };
                code = Utils.GuidToString();
                if(!_CacheMgeSvr.Put(code, result))
                    throw new Exception("服务器繁忙请稍后再试");
            }
            else
                return "";
            return code;
        }

        /// <summary>
        /// 通过授权码登录
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [PublishMethod]
        public LoginResult LoginByCode(string code)
        {
            LoginResult result= _CacheMgeSvr.Get<LoginResult>(code);
            if (result != null)
            {
                if (result.LoginTime.AddMinutes(5)<DateTime.Now)
                    throw new Exception("授权码已过期");
                else
                {
                    result.LoginTime = DateTime.Now;
                    if (!_CacheMgeSvr.Put(result.Token, result))
                        throw new Exception("服务器繁忙请稍后再试");
                }
                _CacheMgeSvr.Delete( code);
            }
            else
                throw new Exception("授权码已使用");
            return result;
        }

        /// <summary>
        /// 获取登录信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [PublishMethod]
        public LoginResult GetLoginInfo(string token)
        {
            LoginResult obj = _CacheMgeSvr.Get<LoginResult>(token);
            if (obj != null)
            {
                return obj;
            }
            else
                return new LoginResult();
        }

        #endregion


    }
}

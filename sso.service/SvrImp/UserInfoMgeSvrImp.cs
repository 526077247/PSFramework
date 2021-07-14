
using IBatisNet.DataAccess;
using service.core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace sso.service
{
    /// <summary>
    /// 用户信息管理服务
    /// </summary>
    public class UserInfoMgeSvr : AppServiceBase, IUserInfoMgeSvr
    {
        #region 服务描述
        private readonly IDaoManager daoManager = null;
        private readonly IUserInfoDao _UserInfoDao = null;
        private readonly ILoginMgeSvr _LoginMgeSvr = null;
        private readonly IEMailMgeSvr _EMailMgeSvr = null;
        private readonly ICacheManager cacheManager = null;
        private readonly ICacheMgeSvr _CacheMgeSvr = null;
        public UserInfoMgeSvr() : base()
        {
            daoManager = ServiceConfig.GetInstance().DaoManager;
            _UserInfoDao = (IUserInfoDao)daoManager.GetDao(typeof(IUserInfoDao));
            _LoginMgeSvr = ServiceManager.GetService<ILoginMgeSvr>("LoginMgeSvr");
            _EMailMgeSvr = ServiceManager.GetService<IEMailMgeSvr>("EMailMgeSvr");

            cacheManager = (ICacheManager)ServiceManager.GetService(typeof(ICacheManager));
            _CacheMgeSvr = cacheManager.GetCache("LoginResult");
        }
        #endregion

        #region IUserInfoMgeSvr函数
        /// <summary>
        /// 检测用户名是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [PublishMethod]
        public bool CheckUserName(string name)
        {
            return _UserInfoDao.CheckUser(name) <= 0;
        }
        /// <summary>
        /// 检测邮箱是否存在
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        [PublishMethod]
        public bool CheckEMail(string mail)
        {
            return _UserInfoDao.CheckMail(mail) <= 0;
        }

        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nickName"></param>
        /// <param name="psw"></param>
        /// <returns></returns>
        [PublishMethod]
        public LoginResult RegisterUser(string name, string nickName, string psw)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("用户名为空");
            }
            if (string.IsNullOrEmpty(psw))
            {
                throw new Exception("密码为空");
            }
            if (string.IsNullOrEmpty(nickName))
            {
                throw new Exception("昵称为空");
            }
            if (_UserInfoDao.CheckUser(name) > 0)
            {
                throw new Exception("用户名已存在");
            }
            UserInfo userInfo = new UserInfo();
            userInfo.CreateTime = DateTime.Now;
            userInfo.Status = (int)TYPE_OF_USER_STATUS.normal;
            userInfo.Id = Guid.NewGuid().ToString();
            userInfo.Psw = MD5Helper.GetMD5HashString(psw);
            userInfo.NickName = nickName;
            userInfo.Name = name;

            _UserInfoDao.Insert(userInfo);
            LoginResult result = _LoginMgeSvr.Login(name, psw);
            return result;
        }

        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        [PublishMethod]
        public LoginResult RegisterUserDetails(UserInfo userInfo)
        {
            if (string.IsNullOrEmpty(userInfo.Name))
            {
                throw new Exception("用户名为空");
            }
            if (string.IsNullOrEmpty(userInfo.Psw))
            {
                throw new Exception("密码为空");
            }
            if (string.IsNullOrEmpty(userInfo.NickName))
            {
                throw new Exception("昵称为空");
            }
            if (_UserInfoDao.CheckUser(userInfo.Name) > 0)
            {
                throw new Exception("用户名已存在");
            }
            string psw = userInfo.Psw;
            userInfo.CreateTime = DateTime.Now;
            userInfo.Status = (int)TYPE_OF_USER_STATUS.normal;
            userInfo.Id = Guid.NewGuid().ToString();
            userInfo.Psw = MD5Helper.GetMD5HashString(psw);
            _UserInfoDao.Insert(userInfo);
            LoginResult result = _LoginMgeSvr.Login(userInfo.Name, psw);
            return result;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="user"></param>
        /// <param name="oldPsw"></param>
        /// <param name="newPsw"></param>
        /// <returns></returns>
        [PublishMethod]
        public bool ChangePsw(string user, string oldPsw, string newPsw)
        {
            Hashtable para = new Hashtable();
            para.Add("Name", user);
            para.Add("Psw", MD5Helper.GetMD5HashString(oldPsw));
            object obj = _UserInfoDao.GetByPara(para);
            if (obj != null)
            {
                UserInfo userInfo = obj as UserInfo;
                userInfo.Psw = MD5Helper.GetMD5HashString(newPsw);
                return _UserInfoDao.Update(userInfo) != null;
            }
            return false;
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [CheckLogin]
        [PublishMethod]
        public UserDetails GetUser(string token)
        {
            UserDetails result = new UserDetails();
            LoginResult loginner = _LoginMgeSvr.GetLoginInfo(token);
            if (!string.IsNullOrEmpty(loginner.Name))
            {
                Hashtable para = new Hashtable();
                para.Add("Name", loginner.Name);
                object obj = _UserInfoDao.GetByPara(para);
                if (obj != null)
                {
                    result = obj as UserDetails;
                }
            }
            return result;
        }

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
        public UserDetails UpdateUser(string token, string nickName = "", string tel = "", string mail = "")
        {
            UserDetails result = new UserDetails();
            LoginResult loginner = _LoginMgeSvr.GetLoginInfo(token);
            if (!string.IsNullOrEmpty(loginner.Name))
            {
                Hashtable para = new Hashtable();
                para.Add("Name", loginner.Name);
                object obj = _UserInfoDao.GetByPara(para);
                if (obj != null)
                {
                    UserInfo userInfo = obj as UserInfo;
                    if (!string.IsNullOrEmpty(nickName))
                        userInfo.NickName = nickName;
                    if (!string.IsNullOrEmpty(tel))
                        userInfo.Tel = tel;
                    if (!string.IsNullOrEmpty(mail))
                        userInfo.Mail = mail;
                    obj = _UserInfoDao.Update(userInfo);
                    if (obj != null)
                        result = obj as UserDetails;
                }
            }
            return result;
        }

        /// <summary>
        /// 忘记密码通过邮箱找回
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type">0邮箱找回</param>
        /// <returns></returns>
        [PublishMethod]
        public bool ForgetPswForEMail(string name, int type = 0)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("用户名为空");
            }
            Hashtable para = new Hashtable();
            para.Add("Name", name);
            object obj = _UserInfoDao.GetByPara(para);
            if (obj != null)
            {
                UserInfo userInfo = obj as UserInfo;
                if (!string.IsNullOrEmpty(userInfo.Mail))
                {
                    string ticket = Utils.GuidToString(6);
                    if( _EMailMgeSvr.SendMail(userInfo.Mail, $"您好，您正在通过邮箱重置密码，若非本人操作请不必理会。点击下方链接进行下一步以重置密码。\r\nhttps://account.mayuntao.xyz/reset?ticket={ticket}&name={name}" , "密码找回", "Myt", "admin@mayuntao.xyz"))
                    {
                        _CacheMgeSvr.Put(ticket, name, 300);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    throw new Exception("没有绑定邮箱");
                }
            }
            return false;
        }

        /// <summary>
        /// 通过ticket找回密码
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="newPsw"></param>
        /// <returns></returns>
        [PublishMethod]
        public bool ChangePswByTicket(string ticket, string newPsw)
        {
            string oName = _CacheMgeSvr.Get<string>(ticket);
            if (!string.IsNullOrEmpty(oName))
            {
                Hashtable para = new Hashtable();
                para.Add("Name", oName);
                object obj = _UserInfoDao.GetByPara(para);
                if (obj != null)
                {
                    UserInfo userInfo = obj as UserInfo;
                    userInfo.Psw = MD5Helper.GetMD5HashString(newPsw);
                    _CacheMgeSvr.Delete(ticket);
                    return _UserInfoDao.Update(userInfo) != null;
                }    
            }
            return false;
        }
        #endregion
    }
}


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
        public UserInfoMgeSvr() : base()
        {
            daoManager = (IDaoManager)ServiceManager.GetService(typeof(IDaoManager));
            _UserInfoDao = (IUserInfoDao)daoManager.GetDao<IUserInfoDao>();
            _LoginMgeSvr = ServiceManager.GetService<ILoginMgeSvr>("LoginMgeSvr");
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
        #endregion
    }
}


using service.core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace sso.service
{

    public interface IUserInfoDao : IBaseDao, IDao
    {
        int CheckUser(string name);
    }

    public class UserInfoDao : BaseDao, IUserInfoDao
    {
        #region IUserInfoDao函数
        /// <summary>
        /// 检测Id和Name相同的用户个数
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public int CheckUser(string name)
        {
            Hashtable para = new Hashtable();
            para.Add("Name", name);
            return QueryCount(para, "CheckUser");
        }
        #endregion

        #region IDao函数
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int Delete(object obj)
        {
            return Delete(obj, "DeleteUserInfo");
        }
        /// <summary>
        /// 取
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public object Get(object obj)
        {
            return Get(obj, "GetUserInfo");
        }
        /// <summary>
        /// 条件取
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public object GetByPara(Hashtable para)
        {
            return Get(para, "GetUserInfoByPara");
        }
        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public object Insert(object obj)
        {
            return Insert(obj, "InsertUserInfo");
        }
        /// <summary>
        /// 条件查数量
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public int QueryCount(Hashtable map)
        {
            return QueryCount(map, "QueryUserInfoCount");
        }
        /// <summary>
        /// 条件查列表
        /// </summary>
        /// <param name="map"></param>
        /// <param name="start"></param>
        /// <param name="paseSize"></param>
        /// <returns></returns>
        public IList QueryList(Hashtable map, int start, int paseSize)
        {
            return QueryList(map, "QueryUserInfoList", start, paseSize);
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public object Update(object obj)
        {
            return Update(obj, "UpdateUserInfo");
        }

        #endregion
    }
}

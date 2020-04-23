using IBatisNet.DataAccess;
using IBatisNet.DataAccess.DaoSessionHandlers;
using System;
using System.Collections;
using IBatisNet.Common.Exceptions;
using IBatisNet.Common.Pagination;
using IBatisNet.DataAccess.Interfaces;
using IBatisNet.DataMapper;

namespace service.core
{
    public class BaseDao : IDao
    {
        #region 服务描述
        protected SqlMapper sqlMapper;
        protected SqlMapper GetLocalSqlMap()
        {
            if (sqlMapper == null)
            {
                IDaoManager daoManager = DaoManager.GetInstance("SqlMapDao");
                SqlMapDaoSession sqlMapDaoSession = (SqlMapDaoSession)daoManager.LocalDaoSession;
                sqlMapper = (SqlMapper)sqlMapDaoSession.SqlMap;
            }
            return sqlMapper;
        }
        #endregion
        #region IBaseDao函数

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="sqlmap"></param>
        /// <returns></returns>
        protected object Insert(object obj, string sqlmap)
        {
            ISqlMapper localSqlMap = GetLocalSqlMap();
            object result;
            try
            {
                result = localSqlMap.Insert(sqlmap, obj);
            }
            catch (Exception ex)
            {
                throw new IBatisNetException("Error executing query '" + sqlmap + "' for insert.  Cause: " + ex.Message, ex);
            }
            return result;
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="sqlmap"></param>
        /// <returns></returns>
        protected int Delete(object obj, string sqlmap)
        {
            ISqlMapper localSqlMap = GetLocalSqlMap();
            int result;
            try
            {
                result = localSqlMap.Delete(sqlmap, obj);
            }
            catch (Exception ex)
            {
                throw new IBatisNetException("Error executing query '" + sqlmap + "' for delete.  Cause: " + ex.Message, ex);
            }
            return result;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="sqlmap"></param>
        /// <returns></returns>
        protected object Update(object obj, string sqlmap)
        {
            ISqlMapper localSqlMap = GetLocalSqlMap();
            int result;
            try
            {
                result = localSqlMap.Update(sqlmap, obj);
            }
            catch (Exception ex)
            {
                throw new IBatisNetException("Error executing query '" + sqlmap + "' for update.  Cause: " + ex.Message, ex);
            }
            return result;
        }

        /// <summary>
        /// 取
        /// </summary>
        /// <param name="para"></param>
        /// <param name="sqlmap"></param>
        /// <returns></returns>
        protected object Get(object para, string sqlmap)
        {
            ISqlMapper localSqlMap = GetLocalSqlMap();
            object result;
            try
            {
                result = localSqlMap.QueryForObject(sqlmap, para);
            }
            catch (Exception ex)
            {
                throw new IBatisNetException("Error executing query '" + sqlmap + "' for object.  Cause: " + ex.Message, ex);
            }
            return result;
        }


        /// <summary>
        /// 查数量
        /// </summary>
        /// <param name="para"></param>
        /// <param name="sqlmap"></param>
        /// <returns></returns>
        protected int QueryCount(object para, string sqlmap)
        {
            return (int)Get(para,sqlmap);
        }

        /// <summary>
        /// 查列表
        /// </summary>
        /// <param name="para"></param>
        /// <param name="sqlmap"></param>
        /// <param name="satrt"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        protected IList QueryList(object para, string sqlmap, int satrt, int pageSize)
        {
            ISqlMapper localSqlMap = GetLocalSqlMap();
            IList result;
            try
            {
                result = localSqlMap.QueryForList(sqlmap, para, satrt, pageSize);
            }
            catch (Exception ex)
            {
                throw new IBatisNetException("Error executing query '" + sqlmap + "' for list.  Cause: " + ex.Message, ex);
            }
            return result;
        }


        #endregion
    }
}

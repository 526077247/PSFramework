using IBatisNet.DataMapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace service.core
{
    public class BaseDao : IBaseDao
    {
        #region 服务描述
        public ISqlMapper mapper;
        #endregion
        #region IBaseDao函数
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="sqlmap"></param>
        /// <returns></returns>
        public int Delete(object obj, string sqlmap)
        {
            return mapper.Delete(sqlmap, obj);
        }
        /// <summary>
        /// 取
        /// </summary>
        /// <param name="para"></param>
        /// <param name="sqlmap"></param>
        /// <returns></returns>
        public object Get(object para, string sqlmap)
        {
            return mapper.QueryForObject(sqlmap, para);
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="sqlmap"></param>
        /// <returns></returns>
        public object Insert(object obj, string sqlmap)
        {
            return mapper.Insert(sqlmap, obj);
        }
        /// <summary>
        /// 查数量
        /// </summary>
        /// <param name="para"></param>
        /// <param name="sqlmap"></param>
        /// <returns></returns>
        public int QueryCount(object para, string sqlmap)
        {
            return (int)mapper.QueryForObject(sqlmap, para);
        }
        /// <summary>
        /// 查列表
        /// </summary>
        /// <param name="para"></param>
        /// <param name="sqlmap"></param>
        /// <param name="start"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IList QueryList(object para, string sqlmap, int start, int pageSize)
        {
            return mapper.QueryForList(sqlmap, para, start, pageSize);
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="sqlmap"></param>
        /// <returns></returns>
        public object Update(object obj, string sqlmap)
        {
            return mapper.Update(sqlmap, obj);
        }
        #endregion
    }
}

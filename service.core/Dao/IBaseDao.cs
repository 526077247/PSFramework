using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace service.core
{
    public interface IBaseDao
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="sqlmap"></param>
        /// <returns></returns>
        object Insert(object obj, string sqlmap);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="sqlmap"></param>
        /// <returns></returns>
        int Delete(object obj, string sqlmap);
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="sqlmap"></param>
        /// <returns></returns>
        object Update(object obj, string sqlmap);
        /// <summary>
        /// 取
        /// </summary>
        /// <param name="para"></param>
        /// <param name="sqlmap"></param>
        /// <returns></returns>
        object Get(object para, string sqlmap);
        /// <summary>
        /// 查数量
        /// </summary>
        /// <param name="para"></param>
        /// <param name="sqlmap"></param>
        /// <returns></returns>
        int QueryCount(object para, string sqlmap);
        /// <summary>
        /// 查列表
        /// </summary>
        /// <param name="para"></param>
        /// <param name="sqlmap"></param>
        /// <param name="satrt"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IList QueryList(object para, string sqlmap, int satrt, int pageSize);

    }
}

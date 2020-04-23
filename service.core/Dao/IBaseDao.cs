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
        /// <returns></returns>
        object Insert(object obj);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        int Delete(object obj);
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        object Update(object obj);
        /// <summary>
        /// 取
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        object Get(object obj);
        /// <summary>
        /// 条件取
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        object GetByPara(Hashtable para);
        /// <summary>
        /// 查列表
        /// </summary>
        /// <param name="map"></param>
        /// <param name="start"></param>
        /// <param name="paseSize"></param>
        /// <returns></returns>
        IList QueryList(Hashtable map, int start, int paseSize);
        /// <summary>
        /// 查数量
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        int QueryCount(Hashtable map);

    }
}

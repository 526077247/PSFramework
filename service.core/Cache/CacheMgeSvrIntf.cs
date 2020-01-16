using System;
using System.Collections.Generic;
using System.Text;

namespace service.core
{
    public interface ICacheMgeSvr
    {

        /// <summary>
        /// 增加/修改
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Put(string key, object value, int timeSpanSeconds = 0);

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        T Get<T>(string key);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Delete(string key);
        /// <summary>
        /// 是否存在key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Exists(string key);
        /// <summary>
        /// 批量存
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        bool HPut(Dictionary<string, object> dic);
        /// <summary>
        /// 批量取
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        bool HGet(out Dictionary<string, object> dic);
        /// <summary>
        /// 查询匹配所有Key
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        List<string> Keys(string pattern);
    }


}

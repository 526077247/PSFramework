
using System;
using System.Collections.Generic;
using System.Text;

namespace service.core
{
    public interface ICacheManager
    {
        /// <summary>
        /// 取缓存
        /// </summary>
        /// <param name="CacheName"></param>
        /// <returns></returns>
        ICacheMgeSvr GetCache(string CacheName);
    }
    public class CacheManager : ICacheManager
    {
        #region 服务描述
        private Dictionary<string, ICacheMgeSvr> _dic;
        private static object _synRoot = new object();
        public CacheManager()
        {
            _dic = new Dictionary<string, ICacheMgeSvr>();
        }
        #endregion
        #region ICacheManager函数
        /// <summary>
        /// 取缓存
        /// </summary>
        /// <param name="CacheName"></param>
        /// <returns></returns>
        public ICacheMgeSvr GetCache(string CacheName)
        {
            object synRoot = _synRoot;
            lock (synRoot)
            {
                if (!_dic.TryGetValue(CacheName, out ICacheMgeSvr cacheMgeSvr))
                {

                    if (ConfigurationManager.Configuration["Caches:" + CacheName + ":Type"] == "Redis")
                    {
                        string IP = ConfigurationManager.Configuration["Caches:" + CacheName + ":Host"];
                        int Port = int.Parse(ConfigurationManager.Configuration["Caches:" + CacheName + ":Port"]);
                        TimeSpan timeSpan = new TimeSpan(
                            int.Parse(ConfigurationManager.Configuration["Caches:" + CacheName + ":lifeTime:hours"]),
                            int.Parse(ConfigurationManager.Configuration["Caches:" + CacheName + ":lifeTime:min"]),
                            int.Parse(ConfigurationManager.Configuration["Caches:" + CacheName + ":lifeTime:seconds"])
                            );
                        cacheMgeSvr = new RedisMgeSvr(IP, Port, timeSpan);
                        _dic.Add(CacheName, cacheMgeSvr);
                    }
                }
                return cacheMgeSvr;
            }
        }
        #endregion

        
    }


}


using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using ServiceStack.Redis;

namespace service.core
{

    public class RedisMgeSvr : ICacheMgeSvr
    {
        #region 服务描述

        private TimeSpan _lifeTime;
        private readonly PooledRedisClientManager pool = null;
        private readonly string[] redisHosts = null;
        public int RedisMaxReadPool = 3;
        public int RedisMaxWritePool = 1;

        public RedisMgeSvr(string REDIS_IP, int REDIS_PORT, TimeSpan lifeTime)
        {
            var redisHostStr = $"{REDIS_IP}:{REDIS_PORT}";

            if (!string.IsNullOrEmpty(redisHostStr))
            {
                redisHosts = redisHostStr.Split(',');

                if (redisHosts.Length > 0)
                {
                    pool = new PooledRedisClientManager(redisHosts, redisHosts,
                        new RedisClientManagerConfig()
                        {
                            MaxWritePoolSize = RedisMaxWritePool,
                            MaxReadPoolSize = RedisMaxReadPool,
                            AutoStart = true
                        });
                }
            }
            _lifeTime = lifeTime;
        }
        #endregion

        #region ICacheMgeSvr函数
        /// <summary>
        /// 增加/修改
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Put(string key, object value, int timeSpanSeconds = 0)
        {
            TimeSpan timeSpan = timeSpanSeconds == 0 ? _lifeTime : new TimeSpan(0, 0, timeSpanSeconds);
            if (value == null)
            {
                return false;
            }

            if (timeSpan.TotalSeconds <= 0)
            {
                Delete(key);

                return false;
            }
            try
            {
                if (pool != null)
                {
                    using var r = pool.GetClient();
                    if (r != null)
                    {
                        r.SendTimeout = 1000;
                        r.Set(key, value, timeSpan);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0}:{1}发生异常!{2}", "cache", "增加/修改", key));
            }
            return true;
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return default(T);
            }

            T obj = default(T);

            try
            {
                if (pool != null)
                {
                    using var r = pool.GetClient();
                    if (r != null)
                    {
                        r.SendTimeout = 1000;
                        obj = r.Get<T>(key);
                    }
                }
            }
            catch
            {
                throw new Exception(string.Format("{0}:{1}发生异常!{2}", "cache", "查询", key));
            }
            return obj;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Delete(string key)
        {
            try
            {
                if (pool != null)
                {
                    using var r = pool.GetClient();
                    if (r != null)
                    {
                        r.SendTimeout = 1000;
                        r.Remove(key);
                    }
                }
            }
            catch
            {
                throw new Exception(string.Format("{0}:{1}发生异常!{2}", "cache", "删除", key));
            }
            return true;
        }
        /// <summary>
        /// 是否存在key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Exists(string key)
        {
            try
            {
                if (pool != null)
                {
                    using var r = pool.GetClient();
                    if (r != null)
                    {
                        r.SendTimeout = 1000;
                        return r.ContainsKey(key);
                    }
                }
            }
            catch
            {
                throw new Exception(string.Format("{0}:{1}发生异常!{2}", "cache", "是否存在", key));
            }

            return false;
        }
        /// <summary>
        /// 批量存
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public bool HPut(Dictionary<string, object> dic)
        {
            if (dic.Count == 0)
            {
                return true;
            }
            try
            {
                if (pool != null)
                {
                    using var r = pool.GetClient();
                    if (r != null)
                    {
                        r.SendTimeout = 1000;
                        r.SetAll(dic);
                    }
                }
            }
            catch
            {
                throw new Exception(string.Format("{0}:{1}发生异常!{2}", "cache", "批量存", dic.Count));
            }
            return true;
        }
        /// <summary>
        /// 批量取
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public bool HGet(out Dictionary<string, object> dic)
        {
            dic = new Dictionary<string, object>();

            try
            {
                if (pool != null)
                {
                    using var r = pool.GetClient();
                    if (r != null)
                    {
                        r.SendTimeout = 1000;
                        dic = (Dictionary<string, object>)r.GetAll<object>(r.GetAllKeys());
                    }
                }
            }
            catch
            {
                throw new Exception(string.Format("{0}:{1}发生异常!{2}", "cache", "批量取", dic.Count));
            }
            return true;

        }
        /// <summary>
        /// 查询匹配所有Key
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public List<string> Keys(string pattern)
        {
            List<string> result = new List<string>();
            try
            {
                if (pool != null)
                {
                    using var r = pool.GetClient();
                    if (r != null)
                    {
                        r.SendTimeout = 1000;
                        result = r.SearchKeys(pattern);
                    }
                }
            }
            catch
            {
                throw new Exception(string.Format("{0}:{1}发生异常!{2}", "cache", "查询匹配", pattern));
            }
            return result;
        }
        #endregion

    }

}


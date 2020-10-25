using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using ServiceStack.Redis;
using System.Linq;
namespace service.core
{
    public class LRURedisMgeSvrImp: ICacheMgeSvr
    {

        #region 服务描述

        private TimeSpan _lifeTime;
        private readonly PooledRedisClientManager pool = null;
        private readonly string[] redisHosts = null;
        public int RedisMaxReadPool = 3;
        public int RedisMaxWritePool = 1;
        public ILogger log;
        public LRURedisMgeSvrImp(string REDIS_IP, int REDIS_PORT, TimeSpan lifeTime, int LRUSize)
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
            log = LogManager.GetLog("LRURedis");
            _locker = new ReaderWriterLockSlim();
            _capacity = LRUSize > 0 ? LRUSize : DEFAULT_CAPACITY;
            _dictionary = new Dictionary<string, object>();
            _linkedList = new LinkedList<string>();
            _timeList = new LinkedList<string>();
            _timeDictionary= new Dictionary<string, DateTime>();
            Thread thread = new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(1);
                    try
                    {          
                        if (Count > 0 )
                        {
                            _locker.EnterReadLock();
                            var key = _timeList.Last.Value ;
                            var date = _timeDictionary[key];
                            _locker.ExitReadLock();
                            if (date < DateTime.Now)
                            {
                                _locker.EnterWriteLock();
                                _timeDictionary.Remove(key);
                                _dictionary.Remove(key);
                                _linkedList.Remove(key);
                                _timeList.Remove(key);
                                _locker.ExitWriteLock();
                            }
                        }
                        
                    }
                    catch(Exception ex)
                    {
                        log.Error(ex);
                    }
                }
            });
            thread.Start();
        }
        #endregion

        #region LRU
        const int DEFAULT_CAPACITY = 255;

        int _capacity;
        ReaderWriterLockSlim _locker;
        IDictionary<string, object> _dictionary;
        LinkedList<string> _linkedList;
        LinkedList<string> _timeList;
        IDictionary<string, DateTime> _timeDictionary;
        public void Set(string key, object value,DateTime lifeTime)
        {
            _locker.EnterWriteLock();
            try
            {
                if (DateTime.MinValue != lifeTime)
                {
                    _timeDictionary[key] = lifeTime;
                    _timeList.Remove(key);
                    _timeList.AddFirst(key);
                }
                _dictionary[key] = value;
                _linkedList.Remove(key);
                _linkedList.AddFirst(key);
                if (_linkedList.Count > _capacity)
                {
                    _timeDictionary.Remove(_linkedList.Last.Value);
                    _dictionary.Remove(_linkedList.Last.Value);
                    _timeList.Remove(_linkedList.Last.Value);
                    _linkedList.RemoveLast();
                    
                }
            }
            finally { _locker.ExitWriteLock(); }
        }

        public void Remove(string key)
        {
            _locker.EnterWriteLock();
            try
            {
                _timeDictionary.Remove(key);
                _timeList.Remove(key);
                _dictionary.Remove(key);
                _linkedList.Remove(key);
            }
            finally { _locker.ExitWriteLock(); }
        }
        public bool TryGet(string key, out object value)
        {
            _locker.EnterUpgradeableReadLock();
            try
            {
                bool b = _dictionary.TryGetValue(key, out value);
                if (b)
                {
                    _locker.EnterWriteLock();
                    try
                    {
                        _linkedList.Remove(key);
                        _linkedList.AddFirst(key);
                    }
                    finally { _locker.ExitWriteLock(); }
                }
                return b;
            }
            catch { throw; }
            finally { _locker.ExitUpgradeableReadLock(); }
        }

        public bool ContainsKey(string key)
        {
            _locker.EnterReadLock();
            try
            {
                return _dictionary.ContainsKey(key);
            }
            finally { _locker.ExitReadLock(); }
        }

        public int Count
        {
            get
            {
                _locker.EnterReadLock();
                try
                {
                    return _dictionary.Count;
                }
                finally { _locker.ExitReadLock(); }
            }
        }

        public int Capacity
        {
            get
            {
                _locker.EnterReadLock();
                try
                {
                    return _capacity;
                }
                finally { _locker.ExitReadLock(); }
            }
            set
            {
                _locker.EnterUpgradeableReadLock();
                try
                {
                    if (value > 0 && _capacity != value)
                    {
                        _locker.EnterWriteLock();
                        try
                        {
                            _capacity = value;
                            while (_linkedList.Count > _capacity)
                            {
                                _linkedList.RemoveLast();
                            }
                        }
                        finally { _locker.ExitWriteLock(); }
                    }
                }
                finally { _locker.ExitUpgradeableReadLock(); }
            }
        }

        public ICollection<string> AllKeys
        {
            get
            {
                _locker.EnterReadLock();
                try
                {
                    return _dictionary.Keys;
                }
                finally { _locker.ExitReadLock(); }
            }
        }

        public ICollection<object> Values
        {
            get
            {
                _locker.EnterReadLock();
                try
                {
                    return _dictionary.Values;
                }
                finally { _locker.ExitReadLock(); }
            }
        }
        #endregion
        #region Redis
        private bool PutToRedis(string key, object value, TimeSpan timeSpan)
        {
           
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
            Set(key, value,DateTime.Now.Add(timeSpan));
            PutToRedis(key, value, timeSpan);
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
            if (TryGet(key, out var res))
            {
                return (T)res;
            }
            else
            {
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
                if(obj!=null)
                    Set(key, obj,DateTime.MinValue);
                return obj;
            }

            
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
                if (AllKeys.Contains(key))
                {
                    Remove(key);
                }
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
                if (AllKeys.Contains(key))
                {
                    return true;
                }
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
        /// 批量取
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public bool HGet(List<string> keys, out Dictionary<string, object> dic)
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
                        dic = (Dictionary<string, object>)r.GetAll<object>(keys);
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
        /// 批量取
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public bool HGet<T>(out Dictionary<string, T> dic)
        {
            dic = new Dictionary<string, T>();

            try
            {
                if (pool != null)
                {
                    using var r = pool.GetClient();
                    if (r != null)
                    {
                        r.SendTimeout = 1000;
                        dic = (Dictionary<string, T>)r.GetAll<T>(r.GetAllKeys());
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
        /// 批量取
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public bool HGet<T>(List<string> keys, out Dictionary<string, T> dic)
        {
            dic = new Dictionary<string, T>();

            try
            {
                if (pool != null)
                {
                    using var r = pool.GetClient();
                    if (r != null)
                    {
                        r.SendTimeout = 1000;
                        dic = (Dictionary<string, T>)r.GetAll<T>(keys);
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

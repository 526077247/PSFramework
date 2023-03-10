using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
namespace Service.Core
{
    public class LRUMgeSvrImp : ICacheMgeSvr
    {
        LRUCache<string, object> cache;
        public LRUMgeSvrImp(int size)
        {
            cache = new LRUCache<string, object>(size);
        }
        public bool Delete(string key)
        {
            cache.Remove(key);
            return true;
        }

        public bool Exists(string key)
        {
            return cache.Keys.Contains(key);
        }

        public T Get<T>(string key)
        {
            cache.TryGet(key, out object value);

            return (T)value;
        }

        public bool HGet(out Dictionary<string, object> dic)
        {
            dic= cache.GetAll();
            return true;
        }

        public bool HGet(List<string> keys, out Dictionary<string, object> dic)
        {
            dic = new Dictionary<string, object>();
            for (int i = 0; i < keys.Count; i++)
            {
                if(cache.TryGet(keys[i], out object value))
                {
                    dic.Add(keys[i], value);
                }
            }
            return true;
        }

        public bool HGet<T>(out Dictionary<string, T> dic)
        {
            dic = cache.GetAll() as Dictionary<string, T>;
            return true;
        }

        public bool HGet<T>(List<string> keys, out Dictionary<string, T> dic)
        {
            dic = new Dictionary<string, T>();
            for (int i = 0; i < keys.Count; i++)
            {
                if (cache.TryGet(keys[i], out object value))
                {
                    dic.Add(keys[i], (T)value);
                }
            }
            return true;
        }

        public bool HPut(Dictionary<string, object> dic)
        {
            foreach (KeyValuePair<string, object> item in dic)
            {
                cache.Set(item.Key, item.Value);
            }
            return true;
        }

        public List<string> Keys(string pattern)
        {
            return cache.Keys as List<string>;
        }

        public bool Put(string key, object value, int timeSpanSeconds = 0)
        {
            cache.Set(key, value);
            return true;
        }
    }
    public class LRUCache<TKey, TValue>
    {
        const int DEFAULT_CAPACITY = 255;

        int _capacity;
        ReaderWriterLockSlim _locker;
        IDictionary<TKey, TValue> _dictionary;
        LinkedList<TKey> _linkedList;

        public LRUCache() : this(DEFAULT_CAPACITY) { }

        public LRUCache(int capacity)
        {
            _locker = new ReaderWriterLockSlim();
            _capacity = capacity > 0 ? capacity : DEFAULT_CAPACITY;
            _dictionary = new Dictionary<TKey, TValue>();
            _linkedList = new LinkedList<TKey>();
        }

        public void Set(TKey key, TValue value)
        {
            _locker.EnterWriteLock();
            try
            {
                _dictionary[key] = value;
                _linkedList.Remove(key);
                _linkedList.AddFirst(key);
                if (_linkedList.Count > _capacity)
                {
                    _dictionary.Remove(_linkedList.Last.Value);
                    _linkedList.RemoveLast();
                }
            }
            finally { _locker.ExitWriteLock(); }
        }
        public Dictionary<TKey, TValue> GetAll()
        {
            return _dictionary as Dictionary<TKey, TValue>;
        }
        public void Remove(TKey key)
        {
            _locker.EnterWriteLock();
            try
            {
                _dictionary.Remove(key);
                _linkedList.Remove(key);
            }
            finally { _locker.ExitWriteLock(); }
        }
        public bool TryGet(TKey key, out TValue value)
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

        public bool ContainsKey(TKey key)
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

        public ICollection<TKey> Keys
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

        public ICollection<TValue> Values
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
    }

}


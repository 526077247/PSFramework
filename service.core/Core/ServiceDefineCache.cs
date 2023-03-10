using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Service.Core
{
    /// <summary>
    /// 服务定义缓存
    /// </summary>
    public static class ServiceDefineCache
    {
        static Dictionary<string, ServiceDefine> serviceCache = new Dictionary<string, ServiceDefine>();
        static Dictionary<string, Type> typeCache = new Dictionary<string, Type>();

        /// <summary>
        /// 通过路径找指定接口类型
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static ServiceDefine GetServiceDefineByPath(string path)
        {
            if (serviceCache.ContainsKey(path))
            {
                return serviceCache[path];
            }
            string jstr = File.ReadAllText(path);
            ServiceDefine serviceDefine = JsonConvert.DeserializeObject<ServiceDefine>(jstr);
            serviceCache[path] = serviceDefine;
            return serviceDefine;
        }
        /// <summary>
        /// 通过路径找指定接口类型
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static Type GetTypeByPath(string path)
        {
            if (typeCache.ContainsKey(path))
            {
                return typeCache[path];
            }
            ServiceDefine serviceDefine = GetServiceDefineByPath(path);
            if (serviceDefine == null) return null;
            Type type = ServiceManager.GetTypeFromAssembly(serviceDefine.IntfName, serviceDefine.IntfAssembly);
            typeCache[path] = type;
            return type;
        }
    }
}

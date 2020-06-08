using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace service.core
{
    public static class DynServerFactory
    {
        private static readonly ProxyGenerator Generator = new ProxyGenerator();
        /// <summary>
        /// 创建代理服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static T CreateServer<T>(string url,string type="json")
        {
            DynamicProxySvrInvocation Interceptor = new DynamicProxySvrInvocation(url, type);   
            T p = (T)Generator.CreateInterfaceProxyWithoutTarget(typeof(T),Interceptor);
            return p;
        }
        /// <summary>
        /// 创建代理服务
        /// </summary>
        /// <param name="url"></param>
        /// <param name="intftype"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object CreateServer(string url, Type intftype, string type = "json")
        {
            DynamicProxySvrInvocation Interceptor = new DynamicProxySvrInvocation(url, type);
            object p = Generator.CreateInterfaceProxyWithoutTarget(intftype, Interceptor);
            return p;
        }
    }
}

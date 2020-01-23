using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace service.core
{
    public class DynServerFactory
    {
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
            ProxyGenerator Generator = new ProxyGenerator();
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
            ProxyGenerator Generator = new ProxyGenerator();
            object p = Generator.CreateInterfaceProxyWithoutTarget(intftype, Interceptor);
            return p;
        }
    }
}

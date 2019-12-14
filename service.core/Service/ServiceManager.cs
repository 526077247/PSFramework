using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;

namespace service.core
{
    public static class ServiceManager
    {
        private static IWindsorContainer container = new WindsorContainer(new XmlInterpreter(ConfigurationManager.Configuration.GetSection("servicesfile").Value));
        
        /// <summary>
        /// 取Service实例
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="SvrID"></param>
        /// <returns></returns>
        public static TService GetService<TService>(string SvrID) where TService : class
        {
            return container.Resolve<TService>(SvrID);
        }
        /// <summary>
        /// 取Service实例
        /// </summary>
        /// <param name="SvrID"></param>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public static object GetService(string SvrID, Type serviceType)
        {
            return container.Resolve(SvrID,serviceType);
        }
        /// <summary>
        /// 取Service实例
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="SvrID"></param>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public static object GetService(Type serviceType)
        {
            return container.Resolve(serviceType);
        }
        /// <summary>
        /// 根据类名取类
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static Type GetTypeFromAssembly(string typeName, Assembly assembly)
        {
            Type[] types = assembly.GetTypes();
            foreach (var t in types)
            {
                if (t.Name == typeName)
                {
                    return t;
                }
            }
            return null;
        }
    }
  
}

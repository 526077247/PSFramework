using Castle.MicroKernel.Registration;
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
        private static IWindsorContainer container = null;
        public static void UseHttpManager(this IServiceCollection app)
        {
            if (!string.IsNullOrEmpty(ConfigurationManager.Configuration.GetSection("servicesfile").Value))
            {
                container = new WindsorContainer(new XmlInterpreter(ConfigurationManager.Configuration.GetSection("servicesfile").Value));
            }
        }

        /// <summary>
        /// 取Service实例
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="SvrID"></param>
        /// <returns></returns>
        public static TService GetService<TService>(string SvrID) where TService : class
        {
            if (container == null)
            {
                throw new Exception("servicesfile未配置");
            }
            try
            {
                IProxyService proxy = container.Resolve<IProxyService>(SvrID);
                if (proxy != null)
                {
                    try
                    {
                        TService service = container.Resolve<TService>(SvrID + "Proxy");
                        return service;
                    }
                    catch
                    {
                        TService svr = proxy.GetService<TService>();
                        container.Register(
                           Component.For<TService>()
                           .Instance(svr)
                           .Named(SvrID + "Proxy")
                           .LifeStyle.Singleton
                        );
                        return svr;
                    }

                }
            }
            catch { }
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
            if (container == null)
            {
                throw new Exception("servicesfile未配置");
            }
            try
            {
                IProxyService proxy = container.Resolve<IProxyService>(SvrID);
                if (proxy != null)
                {
                    try
                    {
                        var service = container.Resolve(SvrID + "Proxy", serviceType);
                        return service;
                    }
                    catch
                    {
                        var svr = proxy.GetService();
                        container.Register(
                           Component.For(serviceType)
                           .Instance(svr)
                           .Named(SvrID + "Proxy")
                           .LifeStyle.Singleton
                        );
                        return svr;
                    }
                }
            }
            catch { }
            return container.Resolve(SvrID, serviceType);
        }
        /// <summary>
        /// 取Service实例
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public static object GetService(Type serviceType)
        {
            if (container == null)
            {
                throw new Exception("servicesfile未配置");
            }
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
                if (t.FullName == typeName)
                {
                    return t;
                }
            }
            return null;
        }

        
    }

}

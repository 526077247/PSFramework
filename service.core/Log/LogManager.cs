using Castle.MicroKernel.Registration;
using Castle.Windsor;
using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace service.core
{
    /// <summary>
    /// log4net帮助类
    /// AdoNetAppender仅支持到.net framework4.5，不支持在.net core项目中持久化日志到数据库
    /// </summary>
    public class LogManager
    {
        private static ILoggerRepository repository;
        private static IWindsorContainer container = new WindsorContainer();

        public static ILogger GetLog(string name)
        {
            try
            {
                ILogger res = container.Resolve<ILogger>(name);
                return res;
            }
            catch
            {
                
                if (repository == null)
                {
                    repository = log4net.LogManager.CreateRepository("CoreLogRepository");
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    using Stream stream = assembly.GetManifestResourceStream("service.core.Log.log4net.config");
                    XmlConfigurator.Configure(repository, stream);

                }
                ILog log = log4net.LogManager.GetLogger(repository.Name, name);
                ILogger logger = new Logger(log);
                
                container.Register(
                   Component.For<ILogger>()
                   .Instance(logger)
                   .Named(name)
                   .LifeStyle.Singleton
                );
                return logger;
            }

           
        }
        
        

    }
}

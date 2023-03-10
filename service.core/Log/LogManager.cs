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

namespace Service.Core
{
    /// <summary>
    /// log
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
                    using Stream stream = assembly.GetManifestResourceStream("Service.Core.Log.log4net.config");
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

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using IBatisNet.Common;
using IBatisNet.DataAccess;
using IBatisNet.DataAccess.Configuration;
using IBatisNet.DataMapper;
using IBatisNet.DataMapper.Configuration;
using Newtonsoft.Json;

namespace service.core
{
    public interface IDaoManager
    {
        object GetDao<T>();
    }
    public class DaoManager: IDaoManager
    {

        private IWindsorContainer container;
        private ISqlMapper mapper;

        public DaoManager()
        {
            container = new WindsorContainer(new XmlInterpreter(ConfigurationManager.Configuration.GetSection("daofile").Value));
            DomSqlMapBuilder builder = new DomSqlMapBuilder();
            mapper = builder.Configure(ConfigurationManager.Configuration.GetSection("mapfile").Value);
        }
        public DaoManager(string path)
        {
            container = new WindsorContainer();
            DomSqlMapBuilder builder = new DomSqlMapBuilder();
            mapper = builder.Configure(path);
        }
        /// <summary>
        /// 取Dao实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public object GetDao<T>()
        {
            BaseDao obj = (BaseDao)container.Resolve(typeof(T));
            obj.mapper = mapper;
            return obj;
        }


    }
   
}

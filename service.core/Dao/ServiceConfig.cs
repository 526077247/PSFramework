using Castle.DynamicProxy;
using IBatisNet.Common;
using IBatisNet.Common.Logging;
using IBatisNet.Common.Utilities;
using IBatisNet.DataAccess;
using IBatisNet.DataAccess.Configuration;
using IBatisNet.DataAccess.DaoSessionHandlers;
using IBatisNet.DataAccess.Interfaces;
using IBatisNet.DataAccess.SessionStore;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.IO;
using System.Text;

namespace service.core
{
    public class ServiceConfig
    {
		private static object _synRoot = new object();
		
		private static ServiceConfig _instance;

		private static readonly ILog _logger = IBatisNet.Common.Logging.LogManager.GetLogger(typeof(ServiceConfig));

        /// <summary>
        ///
        /// </summary>
        public IDaoManager DaoManager { get; private set; }

        /// <summary>
        /// 不允许实例化。
        /// </summary>
        private ServiceConfig()
		{
		}

		/// <summary>
		/// 取得服务实例，实例为单实例对象，释放实例只能用Reset()方法。
		/// </summary>
		/// <returns>服务实例</returns>
		public static ServiceConfig GetInstance()
		{
			return GetInstance(null);
		}

		/// <summary>
		/// 取得服务实例，实例为单实例对象，释放实例只能用Reset()方法。
		/// </summary>
		/// <param name="daoFilePath">dao文件路径, 如传入空则由AppSettings["DaoFile"]决定</param>
		/// <returns>服务实例</returns>
		public static ServiceConfig GetInstance(string daoFilePath)
		{
			if (_instance == null)
			{
				object synRoot = _synRoot;
				lock (synRoot)
				{
					if (_instance == null)
					{
						ConfigureHandler configureHandler = new ConfigureHandler(Reset);
						DomDaoManagerBuilder domDaoManagerBuilder = new DomDaoManagerBuilder();
						string text = string.IsNullOrEmpty(daoFilePath) ? ConfigurationManager.Configuration.GetSection("serviceCore:daoFile").Value : daoFilePath;
						if (!Path.IsPathRooted(text))
						{
							text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, text);
						}
						if (!File.Exists(text))
						{
							throw new Exception("AppSettings中指定的配置文件(DaoFile)不存在!");
						}

						try
						{
							domDaoManagerBuilder.ConfigureAndWatch(text, configureHandler);
						}
						catch (Exception ex2)
						{
							Exception ex = ex2;
							while (ex.InnerException != null)
							{
								ex = ex.InnerException;
							}
							throw new Exception("dao.config不存在或错! " + ex2.Message, ex);
						}
						_instance = new ServiceConfig
						{
							DaoManager = IBatisNet.DataAccess.DaoManager.GetInstance("SqlMapDao")
						};
						ModifyConnectString(_instance.DaoManager, "SqlMapDao");
					}
				}
			}
			return _instance;
		}

		private static void ModifyConnectString(IDaoManager daoManager, string id)
		{
			IConfigurationSection section = ConfigurationManager.Configuration.GetSection("dao:ConnectionStrings");
			if (section != null)
			{
				string text = section.GetConnectionString(id);
				try
				{
					if (!string.IsNullOrEmpty(text))
					{
						text = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(text));
					}
				}
				catch (Exception ex)
				{
					string text2 = "连接字符串定义错：" + id + ", " + ex.Message;
					_logger.Error(text2);
					throw new Exception(text2);
				}
				if (!string.IsNullOrEmpty(text))
				{
					(daoManager.GetDaoSession() as SqlMapDaoSession).SqlMap.DataSource.ConnectionString = text;
				}
			}
		}

		/// <summary>
		/// 释放实例现有实例。
		/// </summary>
		/// <param name="obj"></param>
		public static void Reset(object obj)
		{
			_instance = null;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="daoID"></param>
		/// <returns></returns>
		public static IDaoManager GetDaoManager(string daoID)
		{
			IDaoManager daoManager = IBatisNet.DataAccess.DaoManager.GetInstance(daoID);
			ModifyConnectString(daoManager, daoID);
			return daoManager;
		}


	}
}

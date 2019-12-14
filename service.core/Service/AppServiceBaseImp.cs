using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace service.core
{
    public class AppServiceBase
    {
        /// <summary>
        /// 取版本号
        /// </summary>
        /// <returns></returns>
        [PublishMethod]
        public string GetVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyName assemblyName = assembly.GetName();
            Version version = assemblyName.Version;
            return version.ToString();
        }
    }
}

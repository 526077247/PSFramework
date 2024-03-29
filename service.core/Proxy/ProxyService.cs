﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Service.Core
{
    internal class ProxyService: IProxyService
    {
        private string _service;
        private string _type;
        private string _url;
        public ProxyService(string service,string url, string type="json")
        {
            _service = service;
            _url = url;
            _type = type;
        }

        public object GetService()
        {
            string IntfName = _service.Trim().Split(",")[0].Trim();
            string IntfAssembly = _service.Trim().Split(",")[1].Trim();
            Type intf = ServiceManager.GetTypeFromAssembly(IntfName, IntfAssembly);
            object obj= DynServerFactory.CreateServer(_url, intf, _type);
            return obj;
        }
        public T GetService<T>()
        {
            T obj = DynServerFactory.CreateServer<T>(_url, _type);
            return obj;
        }

    }
}

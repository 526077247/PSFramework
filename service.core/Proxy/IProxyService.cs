using System;
using System.Collections.Generic;
using System.Text;

namespace service.core
{
    internal interface IProxyService
    {
        public object GetService();
        public T GetService<T>();
    }
}

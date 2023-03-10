using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Core
{
    internal interface IProxyService
    {
        public object GetService();
        public T GetService<T>();
    }
}

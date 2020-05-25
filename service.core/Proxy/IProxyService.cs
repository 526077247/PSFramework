using System;
using System.Collections.Generic;
using System.Text;

namespace service.core
{
    public interface IProxyService
    {
        public object GetService();
        public T GetService<T>();
    }
}

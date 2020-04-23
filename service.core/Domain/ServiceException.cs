using System;
using System.Collections.Generic;
using System.Text;

namespace service.core
{
    public class ServiceException: Exception
    {
        public ServiceException(int _code,string _msg) : base()
        {
            code = _code;
            msg =_msg;
        }

        public int code { get; set; }
        public string msg { get; set; }
    }
}

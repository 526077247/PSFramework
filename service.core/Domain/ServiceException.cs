using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Core
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

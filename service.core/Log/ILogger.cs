using log4net;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Core
{
    public interface ILogger
    {
        void Debug(object message);
       
        void Error(object message);
       
        void Fatal(object message);
        
        void Info(object message);
        
        void Warn(object message);
    }
}

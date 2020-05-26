using log4net;
using System;
using System.Collections.Generic;
using System.Text;

namespace service.core
{
    public class Logger : ILogger
    {
        private readonly ILog log;
        public Logger(ILog _log)
        {
            log = _log;
        }
        public void Debug(object message)
        {
            log.Debug(getLogStr(message));
        }

        public void Error(object message)
        {
            log.Error(getLogStr(message));
        }

        public void Fatal(object message)
        {
            log.Fatal(getLogStr(message));
        }

        public void Info(object message)
        {
            log.Info(getLogStr(message));
        }

        public void Warn(object message)
        {
            log.Warn(getLogStr(message));
        }

        private string getLogStr(object message)
        {
            string msg;
            if (message.GetType() == typeof(Exception))
                msg = ((Exception)message).Message + "<br>" + ((Exception)message).StackTrace;
            else if (message.GetType() == typeof(string))
                msg = (string)message;
            else
                try
                {
                    msg = Newtonsoft.Json.JsonConvert.SerializeObject(message);
                }
                catch
                {
                    msg = message.ToString();
                }
           
            return msg;
        }
    }
}

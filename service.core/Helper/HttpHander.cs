using System;
using System.Collections.Generic;
using System.Text;

namespace service.core
{
    public static class HttpHander
    {
        public static void ReturnCustomResult(int code,string msg,object data =null)
        {
            throw new Exception("CUSTOMRESULT:" + Newtonsoft.Json.JsonConvert.SerializeObject(new Result { code = code, msg = msg, data = data })) ;
        }

    }
}

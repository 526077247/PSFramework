using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
namespace service.core
{
    internal class DynamicProxySvrInvocation : IInterceptor
    {
        private string _url;
        private string _type;
        internal DynamicProxySvrInvocation(string url, string type)
        {
            _url = url;
            if (_url.EndsWith("/"))
                _url = _url[0..^1];
            _type = type;

        }
        /// <summary>
        /// 拦截器
        /// </summary>
        /// <param name="invocation"></param>
        public void Intercept(IInvocation invocation)
        {
            if (_type == "stream")
            {
                invocation.ReturnValue = HttpPostHelper.PostStream(_url + "/" + invocation.Method.Name, GetpstData(invocation));
            }
            else if (_type == "json")
            {
                invocation.ReturnValue = HttpPostHelper.PostJson(_url + "/" + invocation.Method.Name, GetpstData(invocation), invocation.Method.ReturnType);
            }
            else
            {
                invocation.ReturnValue = "type定义出错";
            }
        }


        private string GetpstData(IInvocation invocation)
        {
            StringBuilder builder = new StringBuilder();
            var Parameters = invocation.Method.GetParameters();
            for (int i = 0; i < Parameters.Length; i++)
            {
                string jStr = JsonConvert.SerializeObject(invocation.Arguments[i]);
                if (jStr.StartsWith("{") || jStr.StartsWith("["))
                    builder.Append(Parameters[i].Name + "=" + jStr + "&");
                else
                    builder.Append(Parameters[i].Name + "=" + invocation.Arguments[i].ToString() + "&");
            }
            string result = builder.ToString();
            if (result.EndsWith("&"))
                result = result[0..^1];
            return result;
        }

    }
}

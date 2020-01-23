using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace service.core
{
    public class DynamicProxySvr : IInterceptor
    {
        private string _url;
        private string _type;
        private ServiceDefine _serviceDefine = null;
        public DynamicProxySvr(string url, string svrType, string type)
        {
            _url = url;
            _type = type;

        }

        public void Intercept(IInvocation invocation)
        {


            MethodInfo concreteMethod = invocation.GetConcreteMethod();


            if (!invocation.MethodInvocationTarget.IsAbstract)
            {
                var Parameters = invocation.Method.GetParameters();
                if (_type == "stream")
                {
                    invocation.ReturnValue = HttpPostHelper.PostStream(_url,"");
                }
                else if (_type == "json")
                {
                    invocation.ReturnValue = HttpPostHelper.PostJson(_url,"");
                }
                else
                {
                    throw new Exception("type定义出错");
                }
                //执行原对象中的方法   
                //invocation.Proceed();
                invocation.ReturnValue = "";


            }


        }

    }
}

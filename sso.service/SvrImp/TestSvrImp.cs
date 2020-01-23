using System;
using System.Collections.Generic;
using System.Text;
using Castle.DynamicProxy;
using service.core;
namespace sso.service
{
    public  class TestSvr:AppServiceBase,ITestSvr
    {
        #region 服务描述
        public TestSvr() : base()
        {

        }


        #endregion

        #region ITestSvr

        [PublishMethod]
        public string Post()
        {
            IUserInfoMgeSvr p = DynServerFactory.CreateServer<IUserInfoMgeSvr>("http://127.0.0.1:8081/api/UserInfoMgeSvr.proxy/GetVersion", "stream");

            //执行方法看效果   

            //string result = HttpPostHelper.PostStream<string>("http://127.0.0.1:8081/api/UserInfoMgeSvr.proxy/GetVersion", "");
            string result = p.GetVersion();
            return result;
        }

        #endregion
    }
}

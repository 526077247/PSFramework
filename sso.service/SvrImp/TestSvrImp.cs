using System;
using System.Collections.Generic;
using System.Text;
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
            //string result = HttpPostHelper.PostStream<string>("http://127.0.0.1:8081/api/UserInfoMgeSvr.proxy/GetVersion", "");
            string result = HttpPostHelper.PostJson<string>("http://127.0.0.1:8081/api/UserInfoMgeSvr.assx/GetVersion", "");
            return result;
        }

        #endregion
    }
}

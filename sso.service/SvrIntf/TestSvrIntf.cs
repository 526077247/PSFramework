using Service.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace sso.service
{
  
    public interface ITestSvr: IAppServiceBase
    {
        [PublishMethod]
        string Post();
    }
}

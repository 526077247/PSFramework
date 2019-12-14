using System;
using System.Collections.Generic;
using System.Text;

namespace service.core
{
    public class PublishMethodAttribute : Attribute
    {

    }

    public class CheckLoginAttribute : Attribute
    {

    }

    public class PublishName: Attribute
    {
        public string SvrId;
        public PublishName(string _svrId)
        {
            SvrId = _svrId;
        }
    }
}

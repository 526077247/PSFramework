using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Core
{
    public interface ICheckLoginMgeSvr
    {
        bool CheckLogin(string token);
    }

    public class BaseCheckLoginMgeSvr: ICheckLoginMgeSvr
    {
        public bool CheckLogin(string token)
        {
            return true;
        }
    }
}

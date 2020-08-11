
using service.core;
using System;
using System.Collections.Generic;
using System.Text;

namespace sso.service
{
    /// <summary>
    /// 邮件管理服务
    /// </summary>
    public interface IEMailMgeSvr : IAppServiceBase
    {

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="content">内容</param>
        /// <param name="subject">标题</param>
        /// <param name="fromAlias">发送方称呼</param>
        /// <param name="accountName">发送方地址</param>
        /// <param name="addressType">发送方地址类型</param>
        /// <param name="replyToAddress">使用默认回信地址</param>
        /// <returns></returns>
        [PublishMethod]
        bool SendMail(string address, string content, string subject, string fromAlias, string accountName, int addressType = 1, bool replyToAddress = true);

    }
}
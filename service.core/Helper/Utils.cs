
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace service.core
{
    public static class Utils
    {
        internal static IHttpContextAccessor Accessor { get; private set; }
        internal static void StandBy(IHttpContextAccessor accessor)
        {
            Accessor = accessor;
        }
        public static HttpRequest request { get { return Accessor.HttpContext.Request; } }
        private static char[] constant =
      {
        '0','1','2','3','4','5','6','7','8','9',
        'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
        'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'
      };
        public static string GenerateRandomNumber(int Length)
        {
            StringBuilder newRandom = new StringBuilder(62);
            Random rd = new Random();
            for (int i = 0; i < Length; i++)
            {
                newRandom.Append(constant[rd.Next(62)]);
            }
            return newRandom.ToString();
        }
        public static string GetIPAddr()
        {
            if (request == null)
                return null;
            string ip = request.Headers["X-Forwarded-For"];
            if (string.IsNullOrEmpty(ip) || "unknown".Equals(ip))
                ip = request.Headers["Proxy-Client-IP"];
            if (string.IsNullOrEmpty(ip) || "unknown".Equals(ip))
                ip = request.Headers["WL-Proxy-Client-IP"];
            if (string.IsNullOrEmpty(ip) || "unknown".Equals(ip))
                ip = request.Headers["HTTP_CLIENT_IP"];
            if (string.IsNullOrEmpty(ip) || "unknown".Equals(ip))
                ip = request.Headers["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip) || "unknown".Equals(ip))
                ip = request.Host.Host;

            return ip;
        }

        /// <summary>  
        /// 根据GUID获取16位的唯一字符串  
        /// </summary>  
        /// <param name=\"guid\"></param>  
        /// <returns></returns>  
        public static string GuidToString(int length = 10)
        {
            StringBuilder newRandom = new StringBuilder(62);
            Random rd = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < length; i++)
            {
                newRandom.Append(constant[rd.Next(62)]);
            }
            return newRandom.ToString();
        }
    }
}

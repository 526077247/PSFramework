using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace service.core
{
    public class AssxHelper
    {
        /// <summary>
        /// 取服务列表
        /// </summary>
        /// <param name="SvrID"></param>
        /// <returns></returns>
        public static string GetSvrIntfInfo(string SvrID)
        {
            string result;
            if (string.IsNullOrEmpty(ConfigurationManager.Configuration["Services:" + SvrID + ":SvrID"]))
            {
                return "服务未定义" + SvrID;
            }
            else
            {
                string IntfAss = ConfigurationManager.Configuration["Services:" + SvrID + ":IntfAssembly"];
                string IntfName = ConfigurationManager.Configuration["Services:" + SvrID + ":IntfName"];
                Type intf = ServiceManager.GetTypeFromAssembly(IntfName, Assembly.Load(IntfAss));
                if (intf != null)
                {
                    StringBuilder builder = new StringBuilder();
                    builder.AppendLine("服务接口定义：");
                    builder.Append(GetSvrStr(intf));
                    result = builder.ToString();
                }
                else
                {
                    return "未找到接口定义" + IntfAss + "." + IntfName;
                }
            }
            return result;
        }
        /// <summary>
        /// 取服务接口列表
        /// </summary>
        /// <param name="intf"></param>
        /// <returns></returns>
        public static StringBuilder GetSvrStr(Type intf)
        {
            StringBuilder builder = new StringBuilder();
            foreach (Type type in intf.GetInterfaces())
            {
                builder.Append(GetSvrStr(type));
            }
            foreach (MethodInfo Method in intf.GetMethods())
            {
                builder.AppendLine();
                builder.AppendLine(GetSvrMethodStr(Method));
            }
            
            return builder;
        }
        /// <summary>
        /// 取方法
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static string GetSvrMethodStr(MethodInfo method)
        {
            if(method.GetCustomAttribute(typeof(PublishMethodAttribute))==null)
            {
                return "";
            }
            StringBuilder builder = new StringBuilder();
            builder.Append(method.ReturnType.Name + " ");
            builder.Append(method.Name+"(");
            StringBuilder para = new StringBuilder();
            foreach (var item in method.GetParameters())
            {
                para.Append(item.ParameterType.Name + " " + item.Name + ", ");
            }
            if(method.GetParameters().Length>0)
                builder.Append(para.ToString().Substring(0,para.Length-2) + ");");
            else
                builder.Append(");");
            return builder.ToString();
        }
    }
}

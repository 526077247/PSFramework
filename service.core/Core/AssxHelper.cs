using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;

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
                string notePath = AppDomain.CurrentDomain.BaseDirectory + "/" + IntfAss + ".xml";
                List<XElement> elements = new List<XElement>();
                if (File.Exists(notePath))
                {
                    XElement xe = XElement.Load(notePath);
                    elements.AddRange(xe.Elements("members").Elements("member"));
                              
                }
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "/service.core.xml"))
                {
                    XElement xe = XElement.Load(AppDomain.CurrentDomain.BaseDirectory + "/service.core.xml");
                    elements.AddRange(xe.Elements("members").Elements("member"));

                }
                if (intf != null)
                {
                    StringBuilder builder = new StringBuilder();
                    builder.AppendLine("服务接口定义：");
                    builder.Append(GetSvrStr(intf, elements));
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
        /// <param name="notes"></param>
        /// <returns></returns>
        public static StringBuilder GetSvrStr(Type intf, List<XElement> notes)
        {
            StringBuilder builder = new StringBuilder();
            foreach (Type type in intf.GetInterfaces())
            {
                builder.Append(GetSvrStr(type, notes));
            }
            foreach (MethodInfo Method in intf.GetMethods())
            {
                builder.AppendLine();
                builder.AppendLine(GetSvrMethodStr(Method, notes));
            }
            
            return builder;
        }
        /// <summary>
        /// 取方法
        /// </summary>
        /// <param name="method"></param>
        /// <param name="notes"></param>
        /// <returns></returns>
        public static string GetSvrMethodStr(MethodInfo method, List<XElement> notes)
        {
            if(method.GetCustomAttribute(typeof(PublishMethodAttribute))==null)
            {
                return "";
            }
            StringBuilder builder = new StringBuilder();
            builder.Append(GetSvrMethodNotes(method, notes));
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

        /// <summary>
        /// 取注释
        /// </summary>
        /// <param name="svrID"></param>
        /// <param name="method"></param>
        /// <param name="notes"></param>
        /// <returns></returns>
        public static string GetSvrMethodNotes(MethodInfo method, List<XElement> notes)
        {
            foreach(var item in notes)
            {
                if(item.Attribute("name").ToString().Contains(method.Name)&&item.Attribute("name").ToString().Contains(method.Module.Assembly.GetName().Name))
                {
                    StringBuilder SB = new StringBuilder();
                    foreach(var node in item.Elements())
                    {
                        SB.AppendLine(node.ToString().Replace(" ",""));
                    }
                    return SB.ToString();
                }
            }
            return "";
        }
    }
}

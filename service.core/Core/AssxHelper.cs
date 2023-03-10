using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Service.Core
{
    internal class AssxHelper
    {
        private static Dictionary<string, string> cache = new Dictionary<string, string>();
        /// <summary>
        /// 取服务列表
        /// </summary>
        /// <param name="context"></param>
        /// <param name="SvrID"></param>
        /// <returns></returns>
        internal static string GetSvrIntfInfo(HttpContext context, string SvrID)
        {
            string result;
            var path = Path.GetFullPath("wwwroot" + context.Request.Path.ToString().Replace(".rsfs", ".json").Replace(".assx", ".json").Replace(".proxy",".json")).ToString();
            if (path.EndsWith('/')|| path.EndsWith('\\'))
                path = path[0..^1];
            if (!File.Exists(path))
            {
                return "服务未定义" + SvrID;
            }
            else
            {
                if (cache.ContainsKey(path))
                    return cache[path];
                ServiceDefine serviceDefine = ServiceDefineCache.GetServiceDefineByPath(path);
                Type intf = ServiceDefineCache.GetTypeByPath(path);
                if (intf != null)
                {
                    string notePath = AppDomain.CurrentDomain.BaseDirectory + "/" + serviceDefine.IntfAssembly + ".xml";
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
                    StringBuilder builder = new StringBuilder();
                    builder.AppendLine("服务接口定义：");
                    builder.Append(GetSvrStr(intf, elements));
                    result = builder.ToString();
                }
                else
                {
                    return "未找到接口定义" + serviceDefine.IntfName;
                }
            }
            cache[path] = result;
            return result;
        }
        /// <summary>
        /// 取服务接口列表
        /// </summary>
        /// <param name="intf"></param>
        /// <param name="notes"></param>
        /// <returns></returns>
        internal static StringBuilder GetSvrStr(Type intf, List<XElement> notes)
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
        internal static string GetSvrMethodStr(MethodInfo method, List<XElement> notes)
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
        /// <param name="method"></param>
        /// <param name="notes"></param>
        /// <returns></returns>
        internal static string GetSvrMethodNotes(MethodInfo method, List<XElement> notes)
        {
            foreach(var item in notes)
            {
                if (!item.Attribute("name").ToString().Contains("M:"))
                {
                    continue;
                }
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

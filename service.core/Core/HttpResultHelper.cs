using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace service.core
{
    public class HttpResultHelper
    {
        /// <summary>
        /// 调用接口
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Result GetHttpResult(HttpContext context, string SvrID, string method)
        {

            IFormCollection values = null;
            try
            {
                values = context.Request.HttpContext.Request.Form;
            }
            catch { }
            Result result = CreateFailResult("");
            try
            {
                if (string.IsNullOrEmpty(ConfigurationManager.Configuration["Services:" + SvrID + ":SvrID"]))
                {
                    result = CreateFailResult("服务未定义" + SvrID);
                }
                else
                {
                    string IntfAss = ConfigurationManager.Configuration["Services:" + SvrID + ":IntfAssembly"];
                    string IntfName = ConfigurationManager.Configuration["Services:" + SvrID + ":IntfName"];
                    Type intf = ServiceManager.GetTypeFromAssembly(IntfName, Assembly.Load(IntfAss));
                    if (intf != null)
                    {
                        object obj = ServiceManager.GetService(SvrID, intf);
                        if (obj == null)
                        {
                            result = CreateFailResult("服务未定义" + SvrID);
                            return result;
                        }
                        MethodInfo realmethod = intf.GetMethod(method);
                        if (realmethod == null)
                        {
                            foreach (Type type in intf.GetInterfaces())
                            {
                                realmethod = type.GetMethod(method);
                                if (realmethod != null)
                                    break;
                            }
                        }
                        if (realmethod == null)
                            result = CreateFailResult("未找到方法" + method);
                        else
                        {
                            if (realmethod.GetCustomAttribute(typeof(PublishMethodAttribute)) == null)
                            {
                                result = CreateFailResult("服务未发布");
                            }
                            else
                            {
                                if (realmethod.GetCustomAttribute(typeof(CheckLoginAttribute)) != null)
                                {
                                    string token = context.Request.Query["token"].ToString();
                                    ICheckLoginMgeSvr checkLoginMgeSvr = (ICheckLoginMgeSvr)ServiceManager.GetService(typeof(ICheckLoginMgeSvr));
                                    if (checkLoginMgeSvr == null)
                                    {
                                        result = CreateFailResult("服务ICheckLoginMgeSvr未实现或未添加");
                                        return result;
                                    }
                                    if (!checkLoginMgeSvr.CheckLogin(token))
                                    {
                                        result = CreateOffLineResult("用户未登录");
                                        return result;
                                    }
                                }
                                ParameterInfo[] infos = realmethod.GetParameters();

                                object[] objs = new object[infos.Length];
                                for (int i = 0; i < infos.Length; i++)
                                {
                                    if (infos[i].ParameterType == typeof(IFormFile))
                                    {
                                        if (values.Files.Count > 0)
                                            objs[i] = values.Files[0];
                                    }
                                    else if (infos[i].ParameterType == typeof(IFormCollection))
                                    {
                                        if (values.Files.Count > 0)
                                            objs[i] = values.Files;
                                    }
                                    else
                                    {
                                        if (values != null && values.ContainsKey(infos[i].Name))
                                        {

                                            objs[i] = ChangeValueToType(values[infos[i].Name].ToString(), infos[i].ParameterType);
                                        }
                                        else
                                        {
                                            objs[i] = ChangeValueToType(context.Request.Query[infos[i].Name].ToString(), infos[i].ParameterType); 
                                        }
                                    }
                                }
                                result.data = realmethod.Invoke(obj, objs);
                                result.code = (int)TYPE_OF_RESULT_TYPE.success;
                                result.msg = "Success";
                            }
                        }
                    }
                    else
                    {
                        result = CreateFailResult("未找到接口定义" + IntfAss + "." + IntfName + "  ;");
                    }
                }

            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("CUSTOMRESULT"))
                {
                    string jsonStr = ex.Message["CUSTOMRESULT:".Length..];
                    result = JsonConvert.DeserializeObject<Result>(jsonStr);
                }
                else
                    result = CreateFailResult(ex.ToString());
            }
            return result;
        }

        private static object ChangeValueToType(string value, Type type)
        {
            object result = Convert.ChangeType(value, type);
            if (result == null)
                result = JsonConvert.DeserializeObject(value);
            return result;

        }

        /// <summary>
        /// 创建错误返回值
        /// </summary>
        /// <param name="Reason"></param>
        /// <returns></returns>
        private static Result CreateFailResult(string Reason)
        {
            return new Result
            {
                code = (int)TYPE_OF_RESULT_TYPE.failure,
                msg = Reason,
                data = null,
            };
        }
        /// <summary>
        /// 创建未登录返回值
        /// </summary>
        /// <param name="Reason"></param>
        /// <returns></returns>
        private static Result CreateOffLineResult(string Reason)
        {
            return new Result
            {
                code = (int)TYPE_OF_RESULT_TYPE.offline,
                msg = "用户未登录",
                data = null,
            };
        }
    }
}

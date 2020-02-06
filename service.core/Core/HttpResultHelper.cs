using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace service.core
{
    public class HttpResultHelper
    {
        /// <summary>
        /// 调用接口(restful封装)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="SvrID"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static Result GetRestfulHttpResult(HttpContext context, string SvrID, string method)
        {
            Result result = CreateFailResult("");
            try
            {
                var path = Directory.GetParent("wwwroot" + context.Request.Path.ToString().Replace(".rsfs", ".json")).ToString();
                if (!File.Exists(path))
                {
                    result = CreateFailResult("服务未定义" + SvrID);
                }
                else
                {
                    IFormCollection values = null;
                    try
                    {
                        values = context.Request.HttpContext.Request.Form;
                    }
                    catch { }
                    string jstr = File.ReadAllText(path);
                    ServiceDefine serviceDefine = JsonConvert.DeserializeObject<ServiceDefine>(jstr);
                    Type intf = ServiceManager.GetTypeFromAssembly(serviceDefine.IntfName, Assembly.Load(serviceDefine.IntfAssembly));

                    if (intf != null)
                    {
                        result.data = GetServiceResult(context, intf, serviceDefine, method);
                        result.code = (int)TYPE_OF_RESULT_TYPE.success;
                        result.msg = "Success";
                    }
                    else
                    {
                        result = CreateFailResult("未找到接口定义" + serviceDefine.IntfName + "  ;");
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
                    result = CreateFailResult(ex.Message.ToString());
            }
            return result;
        }
        /// <summary>
        /// 调用接口(结果)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="SvrID"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static object GetHttpResult(HttpContext context, string SvrID, string method)
        {
            ErrorResponse result = CreateFailResult2("");
            try
            {
                var path = Directory.GetParent("wwwroot" + context.Request.Path.ToString().Replace(".assx", ".json")).ToString();
                if (!File.Exists(path))
                {
                    result = CreateFailResult2("服务未定义" + SvrID);
                }
                else
                {
                    string jstr = File.ReadAllText(path);
                    ServiceDefine serviceDefine = JsonConvert.DeserializeObject<ServiceDefine>(jstr);
                    Type intf = ServiceManager.GetTypeFromAssembly(serviceDefine.IntfName, Assembly.Load(serviceDefine.IntfAssembly));
                    if (intf != null)
                    {
                        return GetServiceResult(context, intf, serviceDefine, method);
                    }
                    else
                    {
                        result = CreateFailResult2("未找到接口定义" + serviceDefine.IntfName + "  ;");
                    }
                }

            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("CUSTOMRESULT"))
                {
                    string jsonStr = ex.Message["CUSTOMRESULT:".Length..];
                    Result res = JsonConvert.DeserializeObject<Result>(jsonStr);
                    result.errCode = res.code;
                    result.errMsg = res.msg;
                }
                else
                    result = CreateFailResult2(ex.Message.ToString());
            }
            return result;
        }
        /// <summary>
        /// 调用接口(比特流)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="SvrID"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static object GetProxyHttpResult(HttpContext context, string SvrID, string method)
        {
            ErrorResponse result = CreateFailResult2("");
            try
            {
                var path = Directory.GetParent("wwwroot" + context.Request.Path.ToString().Replace(".proxy", ".json")).ToString();
                if (!File.Exists(path))
                {
                    result = CreateFailResult2("服务未定义" + SvrID);
                }
                else
                {
                    string jstr = File.ReadAllText(path);
                    ServiceDefine serviceDefine = JsonConvert.DeserializeObject<ServiceDefine>(jstr);
                    Type intf = ServiceManager.GetTypeFromAssembly(serviceDefine.IntfName, Assembly.Load(serviceDefine.IntfAssembly));
                    if (intf != null)
                    {
                        return GetServiceResult(context, intf, serviceDefine, method);
                    }
                    else
                    {
                        result = CreateFailResult2("未找到接口定义" + serviceDefine.IntfName + "  ;");
                    }
                }

            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("CUSTOMRESULT"))
                {
                    string jsonStr = ex.Message["CUSTOMRESULT:".Length..];
                    Result res = JsonConvert.DeserializeObject<Result>(jsonStr);
                    result.errCode = res.code;
                    result.errMsg = res.msg;
                }
                else
                    result = CreateFailResult2(ex.Message.ToString());
            }
            return result;
        }
        /// <summary>
        /// 反射调用方法获取执行结果
        /// </summary>
        /// <param name="context"></param>
        /// <param name="intf"></param>
        /// <param name="serviceDefine"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private static object GetServiceResult(HttpContext context, Type intf, ServiceDefine serviceDefine, string method)
        {
            IFormCollection values = null;
            try
            {
                values = context.Request.HttpContext.Request.Form;
            }
            catch { }
            object obj = ServiceManager.GetService(serviceDefine.SvrID, intf);
            if (obj == null)
            {
                HttpHander.ReturnCustomResult((int)TYPE_OF_RESULT_TYPE.failure, "服务未定义" + serviceDefine.SvrID);
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
                HttpHander.ReturnCustomResult((int)TYPE_OF_RESULT_TYPE.failure, "未找到方法" + method);
            else
            {
                if (realmethod.GetCustomAttribute(typeof(PublishMethodAttribute)) == null)
                {
                    HttpHander.ReturnCustomResult((int)TYPE_OF_RESULT_TYPE.failure, "服务未发布");
                }
                else
                {
                    if (realmethod.GetCustomAttribute(typeof(CheckLoginAttribute)) != null)
                    {
                        string token = context.Request.Query["token"].ToString();
                        if (string.IsNullOrEmpty(token))
                        {
                            try
                            {
                                token = context.Request.Form["token"];
                            }
                            catch { }
                        }
                        ICheckLoginMgeSvr checkLoginMgeSvr = (ICheckLoginMgeSvr)ServiceManager.GetService(typeof(ICheckLoginMgeSvr));
                        if (checkLoginMgeSvr == null)
                        {
                            HttpHander.ReturnCustomResult((int)TYPE_OF_RESULT_TYPE.failure, "服务ICheckLoginMgeSvr未实现或未添加");
                        }
                        if (!checkLoginMgeSvr.CheckLogin(token))
                        {
                            HttpHander.ReturnCustomResult((int)TYPE_OF_RESULT_TYPE.offline, "用户未登录");
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
                            else
                                HttpHander.ReturnCustomResult((int)TYPE_OF_RESULT_TYPE.failure, $"参数{infos[i]}未找到");
                        }
                        else if (infos[i].ParameterType == typeof(IFormCollection))
                        {
                            if (values.Files.Count > 0)
                                objs[i] = values.Files;
                            else
                                HttpHander.ReturnCustomResult((int)TYPE_OF_RESULT_TYPE.failure, $"参数{infos[i]}未找到");
                        }
                        else
                        {
                            var items = context.Request.Query[infos[i].Name];
                            if (items.Count == 0)
                            {
                                try
                                {
                                    items = context.Request.Form[infos[i].Name];
                                }
                                catch { }
                            }
                            if (!infos[i].HasDefaultValue)
                            {
                                if (items.Count == 0)
                                {
                                    HttpHander.ReturnCustomResult((int)TYPE_OF_RESULT_TYPE.failure, $"参数{infos[i]}未找到");
                                }
                                else
                                {
                                    objs[i] = ChangeValueToType(items[0].ToString(), infos[i].ParameterType);
                                }
                            }
                            else
                            {
                                if (items.Count == 0)
                                {
                                    objs[i] = infos[i].DefaultValue;
                                }
                                else
                                {
                                    objs[i] = ChangeValueToType(items[0].ToString(), infos[i].ParameterType);
                                }
                            }
                        }
                    }
                    return realmethod.Invoke(obj, objs);
                }
            }
            return null;
        }
        /// <summary>
        /// 转换类型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static object ChangeValueToType(string value, Type type)
        {
            object result = null;
            try
            {
                result = Convert.ChangeType(value, type);
            }
            catch { }
            if (result == null)
                result = JsonConvert.DeserializeObject(value, type);
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
        /// 创建错误返回值
        /// </summary>
        /// <param name="Reason"></param>
        /// <returns></returns>
        private static ErrorResponse CreateFailResult2(string Reason)
        {
            return new ErrorResponse
            {
                errCode = (int)TYPE_OF_RESULT_TYPE.failure,
                errMsg = Reason,
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
        /// <summary>
        /// 创建未登录返回值
        /// </summary>
        /// <param name="Reason"></param>
        /// <returns></returns>
        private static ErrorResponse CreateOffLineResult2(string Reason)
        {
            return new ErrorResponse
            {
                errCode = (int)TYPE_OF_RESULT_TYPE.offline,
                errMsg = "用户未登录",
            };
        }
    }
}

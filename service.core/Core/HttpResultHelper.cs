using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.WebUtilities;
using System.Threading;

namespace Service.Core
{
    internal class HttpResultHelper
    {
        private static ILogger logger = LogManager.GetLog("System");
        
        /// <summary>
        /// 调用接口(restful封装)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="SvrID"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        internal static Result GetRestfulHttpResult(HttpContext context, string SvrID, string method)
        {

            Result result;
            try
            {
                var path = Directory.GetParent("wwwroot" + context.Request.Path.ToString().Replace(".rsfs", ".json")).ToString();
                if (!File.Exists(path))
                {
                    result = CreateFailResult("服务未定义" + SvrID);
                }
                else
                {
                    ServiceDefine serviceDefine = ServiceDefineCache.GetServiceDefineByPath(path);
                    Type intf = ServiceDefineCache.GetTypeByPath(path);

                    if (intf != null)
                    {
                        result = new Result
                        {
                            data = GetServiceResult(context, intf, serviceDefine, method),
                            code = (int)TYPE_OF_RESULT_TYPE.success,
                            msg = "Success"
                        };
                    }
                    else
                    {
                        result = CreateFailResult("未找到接口定义" + serviceDefine.IntfName + "  ;");
                    }
                }

            }
            catch (ServiceException ex)
            {
                result = CreateFailResult(ex.code, ex.msg);
            }
            catch (Exception ex)
            {
                result = CreateFailResult(ex.Message + (ex.InnerException?.Message != null ? ex.InnerException.Message.ToString() : ""));
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
        internal static object GetHttpResult(HttpContext context, string SvrID, string method)
        {
            ErrorResponse result;
            try
            {
                var path = Directory.GetParent("wwwroot" + context.Request.Path.ToString().Replace(".assx", ".json")).ToString();
                if (!File.Exists(path))
                {
                    result = CreateFailResult2("服务未定义" + SvrID);
                }
                else
                {
                    ServiceDefine serviceDefine = ServiceDefineCache.GetServiceDefineByPath(path);
                    Type intf = ServiceDefineCache.GetTypeByPath(path);
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
            catch (ServiceException ex)
            {
                result = CreateFailResult2(ex.code, ex.msg);
            }
            catch (Exception ex)
            {
                result = CreateFailResult2(ex.Message + (ex.InnerException?.Message != null ? ex.InnerException.Message : ""));
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
            var map = GetPara(context);
            IFormCollection values = null;
            try
            {
                values = context.Request.HttpContext.Request.Form;
            }
            catch { }
            object obj = ServiceManager.GetService(serviceDefine.SvrID, intf);
            if (obj == null)
            {
                throw new ServiceException((int)TYPE_OF_RESULT_TYPE.failure, "服务未定义" + serviceDefine.SvrID);
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
                throw new ServiceException((int)TYPE_OF_RESULT_TYPE.failure, "未找到方法" + method);
            else
            {
                if (realmethod.GetCustomAttribute(typeof(PublishMethodAttribute)) == null)
                {
                    throw new ServiceException((int)TYPE_OF_RESULT_TYPE.failure, "服务未发布");
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
                            throw new ServiceException((int)TYPE_OF_RESULT_TYPE.failure, "服务ICheckLoginMgeSvr未实现或未添加");
                        }
                        if (!checkLoginMgeSvr.CheckLogin(token))
                        {
                            throw new ServiceException((int)TYPE_OF_RESULT_TYPE.offline, "用户未登录");
                        }
                    }
                    ParameterInfo[] infos = realmethod.GetParameters();
                    object[] objs = new object[infos.Length];
                    for (int i = 0; i < infos.Length; i++)
                    {
                        if (map != null && map.ContainsKey(infos[i].Name))
                        {
                            objs[i] = ChangeValueToType(map[infos[i].Name], infos[i].ParameterType);
                        }
                        else if (infos[i].ParameterType == typeof(IFormFile))
                        {
                            if (values.Files.Count > 0)
                                objs[i] = values.Files[0];
                            else
                                throw new ServiceException((int)TYPE_OF_RESULT_TYPE.failure, $"参数{infos[i]}未找到");
                        }
                        else if (infos[i].ParameterType == typeof(IFormCollection))
                        {
                            if (values.Files.Count > 0)
                                objs[i] = values.Files;
                            else
                                throw new ServiceException((int)TYPE_OF_RESULT_TYPE.failure, $"参数{infos[i]}未找到");
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
                                    throw new ServiceException((int)TYPE_OF_RESULT_TYPE.failure, $"参数{infos[i]}未找到");
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
                    AutoLogAttribute logconfig = realmethod.GetCustomAttribute(typeof(AutoLogAttribute)) as AutoLogAttribute;
                    if (logconfig  != null && (logconfig.Level=="INFO"|| logconfig.Level =="ALL"))
                    {
                        try
                        {
                            logger.Info($"Source:{Utils.GetIPAddr()},Path:{context.Request.Path},Para:{JsonConvert.SerializeObject(objs)}");
                        }
                        catch
                        {

                        }
                    }
                    try
                    {
                        object res = realmethod.Invoke(obj, objs);
                        return res;
                    }
                    catch (ServiceException sex)
                    {
                        throw sex;
                    }
                    catch (Exception ex)
                    {
                        if (logconfig != null && (logconfig.Level == "ERROR" || logconfig.Level == "ALL"))
                        {
                            logger.Error(ex);
                        }
                        if (ex.InnerException is ServiceException se)
                        {
                            throw se;
                        }
                        throw ex;
                    }
                }
            }
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
                if (type == typeof(int))
                {
                    return Convert.ToInt32(value);
                }
                if (type == typeof(uint))
                {
                    return Convert.ToUInt32(value);
                }
                if (type == typeof(short))
                {
                    return Convert.ToInt16(value);
                }
                if (type == typeof(ushort))
                {
                    return Convert.ToUInt16(value);
                }
                if (type == typeof(long))
                {
                    return Convert.ToInt64(value);
                }
                if (type == typeof(ulong))
                {
                    return Convert.ToUInt64(value);
                }
                if (type == typeof(double))
                {
                    return Convert.ToDouble(value);
                }
                if (type == typeof(float))
                {
                    return Convert.ToSingle(value);
                }
                if (type == typeof(decimal))
                {
                    return Convert.ToDecimal(value);
                }
                if (type == typeof(Guid))
                {
                    return new Guid(value.ToString());
                }
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
        /// 创建错误返回值
        /// </summary>
        /// <param name="code"></param>
        /// <param name="Reason"></param>
        /// <returns></returns>
        private static Result CreateFailResult(int code, string Reason)
        {
            return new Result
            {
                code = code,
                msg = Reason,
                data = null,
            };
        }
        /// <summary>
        /// 创建错误返回值
        /// </summary>
        /// <param name="code"></param>
        /// <param name="Reason"></param>
        /// <returns></returns>
        private static ErrorResponse CreateFailResult2(int code, string Reason)
        {
            return new ErrorResponse
            {
                errCode = code,
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

        private static async Task<string> ReadBodyAsync(HttpRequest request)
        {
            if (request.ContentLength > 0)
            {
                await EnableRewindAsync(request).ConfigureAwait(false);
                var encoding = GetRequestEncoding(request);
                return await ReadStreamAsync(request.Body, encoding).ConfigureAwait(false);
            }
            return null;
        }

        private static Encoding GetRequestEncoding(HttpRequest request)
        {
            var requestContentType = request.ContentType;
            var requestMediaType = requestContentType == null ? default(MediaType) : new MediaType(requestContentType);
            var requestEncoding = requestMediaType.Encoding;
            if (requestEncoding == null)
            {
                requestEncoding = Encoding.UTF8;
            }
            return requestEncoding;
        }

        private static async Task EnableRewindAsync(HttpRequest request)
        {
            if (!request.Body.CanSeek)
            {
                request.EnableBuffering();

                await request.Body.DrainAsync(CancellationToken.None);
                request.Body.Seek(0L, SeekOrigin.Begin);
            }
        }

        private static async Task<string> ReadStreamAsync(Stream stream, Encoding encoding)
        {
            using (StreamReader sr = new StreamReader(stream, encoding, true, 1024, true))//这里注意Body部分不能随StreamReader一起释放
            {
                var str = await sr.ReadToEndAsync();
                stream.Seek(0, SeekOrigin.Begin);//内容读取完成后需要将当前位置初始化，否则后面的InputFormatter会无法读取
                return str;
            }
        }

        /// <summary>
        /// 将获取的formData存入字典数组
        /// </summary>
        private static Dictionary<string, string> GetFormData(string formData, Encoding encoding)
        {
            //将参数存入字符数组
            string[] dataArry = formData.Split('&');

            //定义字典,将参数按照键值对存入字典中
            Dictionary<string, string> dataDic = new Dictionary<string, string>();
            //遍历字符数组
            for (int i = 0; i <= dataArry.Length - 1; i++)
            {
                //当前参数值
                string dataParm = dataArry[i];
                //"="的索引值
                int dIndex = dataParm.IndexOf("=");
                //参数名作为key
                string key = dataParm.Substring(0, dIndex);
                //参数值作为Value
                string value = dataParm.Substring(dIndex + 1, dataParm.Length - dIndex - 1);
                //将编码后的Value解码
                string deValue = System.Web.HttpUtility.UrlDecode(value, encoding);
                if (key != "__VIEWSTATE")
                {
                    //将参数以键值对存入字典
                    dataDic.Add(key, deValue);
                }
            }

            return dataDic;
        }

        private static Dictionary<string, string> GetPara(HttpContext context)
        {
            if (context.Request.Headers["Urlencode-Type"] == "PS1801")
            {
                var encoding = GetRequestEncoding(context.Request);
                var requestContent = ReadBodyAsync(context.Request);
                return GetFormData(requestContent.Result.Replace("+", "%2b"), encoding);
            }
            return null;
        }

        
    }
}

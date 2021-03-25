using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace service.core
{
    internal class TsHelper
    {
        private static Dictionary<string, string> cache = new Dictionary<string, string>();
        /// <summary>
        /// 获取ts工具服务
        /// </summary>
        /// <param name="context"></param>
        /// <param name="SvrID"></param>
        /// <returns></returns>
        internal static string GeTsScriptsClient(HttpContext context, string SvrID)
        {
            var path = Path.GetFullPath("wwwroot" + context.Request.Path.ToString().Replace(".sts", ".json").ToString());
            if (path.EndsWith('/') || path.EndsWith('\\'))
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
                    string text = "";
                    string svrName = SvrID;
                    string responsetype = context.Request.Query["res"];
                    if (string.IsNullOrEmpty(responsetype))
                        responsetype = "json";
                    using (IEnumerator<KeyValuePair<string, StringValues>> enumerator = context.Request.Query.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            KeyValuePair<string, StringValues> current = enumerator.Current;
                            if (current.Key.EndsWith("callback"))
                            {
                                text = current.Value;
                                break;
                            }
                        }
                    }
                    if (Uri.TryCreate(context.Request.GetDisplayUrl(), UriKind.Absolute, out Uri uri))
                    {
                        string serverPath = uri.AbsolutePath;
                        string text2 = CreateTsServer(intf, serverPath, responsetype, svrName);
                        if (!string.IsNullOrEmpty(text))
                        {
                            text2 = text + "(" + JsonConvert.SerializeObject(text2) + ");";
                        }
                        cache[path] = text2;
                        return text2;
                    }


                }
                else
                {
                    return "未找到接口定义" + serviceDefine.IntfName;
                }
            }

            return "";
        }

        /// <summary>
        /// 创建指定服务的ts类
        /// </summary>
        /// <param name="def">服务</param>
        /// <param name="ServerPath">服务路径或id</param>
        /// <param name="response">返回类型json或text</param>
        /// <returns>js类</returns>
        private static string CreateTsServer(Type def, string ServerPath, string response, string resultName)
        {
            if (ServerPath[ServerPath.Length - 1] != '/')
            {
                ServerPath += "/";
            }
            var _intfName = def.Name;
            var _svrName = (_intfName.StartsWith("I")) ? _intfName.Substring(1) : _intfName;
            var methodsstr = new List<string>();
            var objs = new List<string>();
            var ScriptStr = new StringBuilder();
            // 头部对angular的固定引用
            AppendToStringBuilder(ScriptStr, "/* Angular Version 6 below need to be deleted {providedIn: 'root'} */");
            AppendToStringBuilder(ScriptStr, "/* angular */");
            AppendToStringBuilder(ScriptStr, "import {Injectable} from '@angular/core';");
            AppendToStringBuilder(ScriptStr, "import {HttpClient, HttpHeaders, HttpParams} from '@angular/common/http';");
            AppendToStringBuilder(ScriptStr, "import {catchError} from 'rxjs/operators';");
            AppendToStringBuilder(ScriptStr, "import {Result} from '../domain/result.domain';");
            AppendToStringBuilder(ScriptStr, "/* owner */");
            //AppendToStringBuilder(ScriptStr, "import {ErrorHandle, ErrorResponse} from '../share';");
            // 遍历方法中需要引用的类
            foreach (MethodInfo meth in def.GetMethods())
            {

                if (TypeIsObject(meth.ReturnType) && !IsList(meth.ReturnType) && !objs.Contains(meth.ReturnType.Name))
                    objs.Add(meth.ReturnType.Name);

                var methstr = CreateJsMethodParamObj(meth, objs, response);
                methodsstr.Add(methstr);
            }
            foreach (var objstr in objs)
            {
                AppendToStringBuilder(ScriptStr, $"import {{{objstr}}} from \'../domain/{objstr.ToLower()}.domain\';");
            }
            AppendToStringBuilder(ScriptStr);
            AppendToStringBuilder(ScriptStr, "@Injectable({");
            AppendToStringBuilder(ScriptStr, "providedIn: 'root'", 1);
            AppendToStringBuilder(ScriptStr, "})");
            // 服务主体部分
            if (!string.IsNullOrEmpty(resultName))
            {
                _svrName = resultName;
            }
            AppendToStringBuilder(ScriptStr, $"export class {_svrName} {{");
            AppendToStringBuilder(ScriptStr, "baseurl: string;", 1);
            //AppendToStringBuilder(ScriptStr, "errHandle: ErrorHandle;", 1);
            AppendToStringBuilder(ScriptStr, "header: HttpHeaders;", 1);
            AppendToStringBuilder(ScriptStr);
            AppendToStringBuilder(ScriptStr, "constructor(private httpClient: HttpClient) {", 1);
            AppendToStringBuilder(ScriptStr, $"this.baseurl = '{ServerPath.Replace("sts", "assx")}';", 2);
            //AppendToStringBuilder(ScriptStr, "this.errHandle = new ErrorHandle();", 2);
            AppendToStringBuilder(ScriptStr, "this.header = new HttpHeaders().append('Urlencode-Type', 'PS1801');", 2);
            AppendToStringBuilder(ScriptStr, "}", 1);
            foreach (var cmethod in methodsstr)
            {
                ScriptStr.Append(cmethod);
            }
            AppendToStringBuilder(ScriptStr, "}");
            AppendToStringBuilder(ScriptStr);
            return ScriptStr.ToString();
        }

        /// <summary>
        /// 参数用对象传递
        /// </summary>
        /// <param name="meth"></param>
        /// <param name="objs"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        private static string CreateJsMethodParamObj(MethodInfo meth, ICollection<string> objs, string response)
        {
            var ScriptStr = new StringBuilder();
            var pars = new StringBuilder();
            var parsdata = new StringBuilder();
            var parInfos = meth.GetParameters();
            var optionparams = new List<ParameterInfo>();
            var retunrtype = GetTStype(meth.ReturnType);

            parInfos.Aggregate(true, (current, par) => dealParam(objs, pars, parsdata, current, optionparams, par));
            AppendToStringBuilder(ScriptStr);
            AppendToStringBuilder(ScriptStr, $"{meth.Name}({pars}): Promise<{(response == "json" ? retunrtype : "string")}> {{", 1);
            //AppendToStringBuilder(ScriptStr, "const that = this;", 2);
            AppendToStringBuilder(ScriptStr, "return new Promise((resolve, reject) => {", 2);
            // 将参数处理为HttpParam
            GetParamObj(parInfos, ScriptStr, optionparams);
            // 发起http post请求
            AppendToStringBuilder(ScriptStr, $"this.httpClient.post<Result>(this.baseurl + \'{meth.Name}\', httpParams, {{headers: this.header}}).subscribe(res => {{", 3);
            //AppendToStringBuilder(ScriptStr, ".pipe(", 4);
            //AppendToStringBuilder(ScriptStr, $"catchError(this.errHandle.listen('{meth.Name}'))", 5);
            //AppendToStringBuilder(ScriptStr, ")", 4);
            //AppendToStringBuilder(ScriptStr, ".subscribe(data => {", 4);
            AppendToStringBuilder(ScriptStr, "if (res.code !== 0){", 4);
            AppendToStringBuilder(ScriptStr, "reject(res.msg);", 5);
            //AppendToStringBuilder(ScriptStr, "}", 4);
            //AppendToStringBuilder(ScriptStr, "if (data instanceof ErrorResponse) {", 4);
            //AppendToStringBuilder(ScriptStr, "reject(data.errorResponse);", 5);
            AppendToStringBuilder(ScriptStr, "} else {", 4);
            if (TypeIsObject(meth.ReturnType) && !IsList(meth.ReturnType))
                AppendToStringBuilder(ScriptStr, $"resolve(new {retunrtype}(res.data));", 5);
            else if (IsList(meth.ReturnType))
            {
                AppendToStringBuilder(ScriptStr, "const tmp = [];", 5);
                AppendToStringBuilder(ScriptStr, "for (const item of res.data) {", 5);
                AppendToStringBuilder(ScriptStr, $"tmp.push(new {retunrtype.Substring(0, retunrtype.Length - 2)}(item));", 6);
                AppendToStringBuilder(ScriptStr, "}", 5);
                AppendToStringBuilder(ScriptStr, "resolve(tmp);", 5);
            }
            else
                AppendToStringBuilder(ScriptStr, $"resolve(res.data as {retunrtype});", 5);
            AppendToStringBuilder(ScriptStr, "}", 4);
            AppendToStringBuilder(ScriptStr, "}, err => {", 3);
            AppendToStringBuilder(ScriptStr, "reject(err);", 4);
            AppendToStringBuilder(ScriptStr, "});", 3);
            AppendToStringBuilder(ScriptStr, "});", 2);
            AppendToStringBuilder(ScriptStr, "}", 1);

            return ScriptStr.ToString().Replace("`1","");
        }
        /// <summary>
        /// 用对象的方式处理参数
        /// </summary>
        /// <param name="parInfos"></param>
        /// <param name="ScriptStr"></param>
        private static void GetParamObj(IList<ParameterInfo> parInfos, StringBuilder ScriptStr, IList<ParameterInfo> optionPar)
        {
            var ptype = "const";
            if (optionPar.Count > 0)
                ptype = "let";
            var tmpParams = new List<ParameterInfo>();
            foreach (var p in parInfos)
            {
                bool isNotOptionsPar = true;
                foreach (var optionP in optionPar)
                {
                    if (p.Name.Equals(optionP.Name))
                    {
                        isNotOptionsPar = false;
                    }
                }
                if (isNotOptionsPar)
                {
                    tmpParams.Add(p);
                }
            }
            if (tmpParams.Count > 0)
            {
                AppendToStringBuilder(ScriptStr, $"{ptype} httpParams = new HttpParams()", 3);
                for (int i = 0; i < tmpParams.Count; i++)
                {
                    var t = tmpParams[i];
                    if (t.IsOptional) continue;
                    ParameterInfo param = t;
                    AppendToStringBuilder(ScriptStr, $".append(\'{param.Name}\', {getparastr(param)})" + (i == tmpParams.Count - 1 ? ";" : ""), 4);
                }
            }
            else
            {
                AppendToStringBuilder(ScriptStr, $"{ptype} httpParams = new HttpParams();", 3);
            }

            foreach (ParameterInfo param in optionPar)
            {
                AppendToStringBuilder(ScriptStr, $"if ({LowerFirstLetter(param.Name)} !== undefined) {{", 3);
                AppendToStringBuilder(ScriptStr, $"httpParams = httpParams.append(\'{param.Name}\', {getparastr(param)});", 4);
                AppendToStringBuilder(ScriptStr, "}", 3);
            }

        }



        /// <summary>
        /// 处理参数
        /// </summary>
        /// <param name="objs"></param>
        /// <param name="pars"></param>
        /// <param name="parsdata"></param>
        /// <param name="first"></param>
        /// <param name="optionparams"></param>
        /// <param name="par"></param>
        /// <returns></returns>
        private static bool dealParam(ICollection<string> objs, StringBuilder pars, StringBuilder parsdata, bool first, ICollection<ParameterInfo> optionparams, ParameterInfo par)
        {
            if (par.IsOptional)
                optionparams.Add(par);
            if (par.ParameterType.GetInterface("IJsonSerializer") != null)
            {
                if (TypeIsObject(par.ParameterType) && !objs.Contains(par.ParameterType.Name))
                    objs.Add(par.ParameterType.Name);
            }

            var partype = GetTStype(par.ParameterType);
            var parname = LowerFirstLetter(par.Name);
            if (!first)
            {
                pars.Append(", ");
                if (!par.IsOptional)
                    parsdata.Append($" + \'&{parname}=\' + {getparastr(par)}");

            }
            else
            {
                if (!par.IsOptional)
                {
                    first = false;
                    parsdata.Append($"\'{parname}=\' + {getparastr(par)}");
                }

            }
            pars.Append(parname + (par.IsOptional ? "?" : "") + ": " + partype);
            return first;
        }

        /// <summary>
        /// 参数拼接
        /// </summary>
        /// <param name="par"></param>
        /// <returns></returns>
        private static string getparastr(ParameterInfo par)
        {
            string parName = LowerFirstLetter(par.Name);
            if (IsNoShowType(par.ParameterType))
                return $"JSON.stringify({parName})";
            if (par.ParameterType == typeof(DateTime))
                return $"{parName}.toLocaleString()";
            return $"{parName}.toString()";
        }

        /// <summary>
        /// 判断类是否对象
        /// </summary>
        /// <param name="ctype"></param>
        /// <returns></returns>
        private static bool TypeIsObject(Type ctype)
        {
            return ctype.IsClass && !IsNoShowType(ctype);
        }

        /// <summary>
        /// 判断类是否动态类和HashTable
        /// </summary>
        /// <param name="ctype"></param>
        /// <returns></returns>
        private static bool IsNoShowType(Type ctype)
        {
            return IsTypeOrBaseType(ctype, typeof(Hashtable));
        }

        /// <summary>
        /// 判断类是否动态类和HashTable
        /// </summary>
        /// <param name="ctype"></param>
        /// <returns></returns>
        private static bool IsList(Type ctype)
        {
            return ctype.Name == typeof(List<object>).Name;
        }

        private static bool IsTypeOrBaseType(Type ctype, Type comparetype)
        {
            while (true)
            {
                if (ctype == comparetype)
                    return true;
                else if (ctype.BaseType != null)
                {
                    ctype = ctype.BaseType;
                    continue;
                }
                return false;
            }
        }

        /// <summary>
        /// 对应TS类名
        /// </summary>
        /// <param name="ctype"></param>
        /// <returns></returns>
        private static string GetTStype(Type ctype)
        {
            var partype = ctype.Name;
            if (ctype == typeof(int) || ctype == typeof(long) || ctype == typeof(double) || ctype == typeof(float))
                partype = "number";
            else if (ctype == typeof(string))
                partype = "string";
            else if (ctype == typeof(DateTime))
                partype = "Date";
            else if (ctype == typeof(bool))
                partype = "boolean";
            else if (IsList(ctype))
                partype = ctype.GenericTypeArguments[0].Name + "[]";
            else if (IsNoShowType(ctype))
                partype = "any";
            return partype;
        }

        private static string LowerFirstLetter(string str)
        {
            return str.Substring(0, 1).ToLower() + str.Substring(1);
        }



        #region 字符串处理
        /// <summary>
        /// 将一行字符串加入到StringBuilder里
        /// </summary>
        /// <param name="sb">加入到的StringBuilder</param>
        /// <param name="appendStr">需要添加的字符串</param>
        /// <param name="indentCount">字符串前缩进次数</param>
        private static void AppendToStringBuilder(StringBuilder sb, string appendStr = "", int indentCount = 0)
        {
            string result = "";
            while (indentCount != 0)
            {
                result += "  ";
                indentCount--;
            }
            sb.AppendLine(result + appendStr);
        }
        #endregion
    }



}

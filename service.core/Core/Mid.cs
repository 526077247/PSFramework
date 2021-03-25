using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace service.core
{

    public static class HttpManagerMiddlewareExtension
    {
        public static void UseHttpManager(this IApplicationBuilder app,IHttpContextAccessor accessor)
        {
            Utils.StandBy(accessor);
            app.UseMiddleware<HttpManagerMiddleware>();
        }
        public static void UseHttpManager(this IApplicationBuilder app)
        {
            app.UseMiddleware<HttpManagerMiddleware>();
        }
    }
    /// <summary>
    /// 通过反射调用服务
    /// </summary>
    public class HttpManagerMiddleware
    {
        private readonly RequestDelegate next;

        public HttpManagerMiddleware(RequestDelegate next)
        {
            this.next = next;
        }



        public async Task Invoke(HttpContext context)
        {
            
            string path = context.Request.Path;
            path = path.EndsWith("/") ? path.Substring(0, path.Length - 1) : path;
            string[] paths = path.Split("/");
            if (paths.Length == 3 && paths[2].EndsWith(".rsfs"))
            {
                string SvrID = paths[2].Replace(".rsfs", "");

                string result = AssxHelper.GetSvrIntfInfo(context, SvrID);
                context.Response.ContentType = "text/plain; charset=utf-8";
                await context.Response.WriteAsync(result);
            }
            else if (paths.Length == 3 && paths[2].EndsWith(".assx"))
            {
                string SvrID = paths[2].Replace(".assx", "");

                string result = AssxHelper.GetSvrIntfInfo(context, SvrID);
                context.Response.ContentType = "text/plain; charset=utf-8";
                await context.Response.WriteAsync(result);
            }
            else if (paths.Length == 4 && paths[2].EndsWith(".rsfs"))
            {
                string SvrID = paths[2].Replace(".rsfs", "");
                string method = paths[3];
                Result result = HttpResultHelper.GetRestfulHttpResult(context, SvrID, method);
                context.Response.ContentType = "text/plain; charset=utf-8";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(result));
            }
            else if (paths.Length == 4 && paths[2].EndsWith(".assx"))
            {
                string SvrID = paths[2].Replace(".assx", "");
                string method = paths[3];
                object result = HttpResultHelper.GetHttpResult(context, SvrID, method);
                context.Response.ContentType = "text/plain; charset=utf-8";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(result));
            }
            else if (paths.Length == 3 && paths[2].EndsWith(".sts"))
            {
                string SvrID = paths[2].Replace(".sts", "");
                string result = TsHelper.GeTsScriptsClient(context, SvrID);
                context.Response.ContentType = "text/plain; charset=utf-8";
                await context.Response.WriteAsync(result);
            }
            else
            {
                await next.Invoke(context);
            }

        }

    }
    public static class Jump404MiddlewareExtension
    {
        public static void UseJump404(this IApplicationBuilder app, string path)
        {
            app.UseMiddleware<Jump404Middleware>(path);
        }
    }
    /// <summary>
    /// 当页面为404时跳到主页
    /// </summary>
    public class Jump404Middleware
    {
        private readonly RequestDelegate next;
        private readonly string path;
        public Jump404Middleware(RequestDelegate next, string path)
        {
            this.next = next;
            this.path = path;
        }

        public async Task Invoke(HttpContext context)
        {
            await next.Invoke(context);

            var response = context.Response;

            //如果是404就跳转到主页
            if (response.StatusCode == 404)
            {
                response.Redirect(path);
            }
        }
    }

}

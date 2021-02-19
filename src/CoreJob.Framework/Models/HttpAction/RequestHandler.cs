using System;
using System.Text;
using System.Threading.Tasks;
using CoreJob.Framework.Abstractions;
using CoreJob.Framework.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace CoreJob.Framework.Models.HttpAction
{
    public class RequestHandler
    {
        private readonly ILogger<RequestHandler> _logger;

        public RequestHandler(ILogger<RequestHandler> logger)
        {
            _logger = logger;
        }

        public async Task<bool> Do(HttpContext context, JobOptions options, RouteTable routeTable)
        {
            if (IsCoreJobRequest(context.Request, options, out PathString subPath))
            {
                // This is a request in the Dashboard path
                await HandleRequest(context, subPath, options, routeTable);
                return true;
            }

            return false;
        }

        private async Task HandleRequest(HttpContext context, PathString subPath, JobOptions options, RouteTable routeTable)
        {
            context.Response.ContentType = options.OutputMessageType.GetDescription();
            // token 检查
            if (!await ValidToken(context, options))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteBytesAsync(await context.ResponseObjAsync("Token验证失败".Fail()));
                return;
            }

            context.Response.StatusCode = StatusCodes.Status200OK;
            var subUrl = subPath.Value.EnsureStartSlash();
            var route = routeTable.FindByPattern(subUrl);
            if (route != null)
            {
                var controller = context.RequestServices.GetRequiredService(route.TypeController) as IJobController;
                if (controller != null)
                {
                    try
                    {
                        var response = await controller.ActionAsync(context);

                        if (response != null)
                        {
                            await context.Response.WriteBytesAsync(response.ExecuteResult(context));
                            return;
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "请求异常");
                        await context.Response.WriteBytesAsync(await context.ResponseObjAsync("请求异常".Fail()));
                        return;
                    }
                }
            }

            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteBytesAsync(await context.ResponseObjAsync("404 NotFound".Fail()));
        }

        private bool IsCoreJobRequest(HttpRequest request, JobOptions options, out PathString subPath)
        {
            if (request.Path.StartsWithSegments(options.ApiUriSegments, StringComparison.OrdinalIgnoreCase, out subPath))
            {
                return true;
            }

            return false;
        }

        private ValueTask<bool> ValidToken(HttpContext context, JobOptions options)
        {
            context.Request.Headers.TryGetValue(JobConstant.Token, out StringValues token);
            if (!string.IsNullOrEmpty(options.Token) && token != options.Token)
            {
                return new ValueTask<bool>(false);
            }

            return new ValueTask<bool>(true);
        }
    }
}

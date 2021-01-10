using System.Threading.Tasks;
using CoreJob.Framework.Models.HttpAction;
using CoreJob.Server.Framework.Models;
using Microsoft.AspNetCore.Http;

namespace CoreJob.Server.Framework
{
    public class CoreJobServerMiddleware
    {
        private readonly CoreJobServerOptions _options;
        private readonly ServerRouteTable _routeTable;
        private readonly RequestDelegate _next;
        private readonly RequestHandler _requestHandler;

        public CoreJobServerMiddleware(CoreJobServerOptions options,
            ServerRouteTable routeTable,
            RequestHandler requestHandler,
            RequestDelegate next)
        {
            _options = options;
            _routeTable = routeTable;
            _next = next;
            _requestHandler = requestHandler;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (await _requestHandler.Do(context, _options, _routeTable))
            {
                return;
            }

            await _next(context);
        }
    }
}

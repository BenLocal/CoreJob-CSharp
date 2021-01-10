using System.Threading.Tasks;
using CoreJob.Client.Framework.Models;
using CoreJob.Framework.Models.HttpAction;
using Microsoft.AspNetCore.Http;

namespace CoreJob.Client.Framework
{
    public class CoreJobClientMiddleware
    {
        private readonly CoreJobClientOptions _options;
        private readonly ClientRouteTable _routeTable;
        private readonly RequestDelegate _next;
        private readonly RequestHandler _requestHandler;

        public CoreJobClientMiddleware(CoreJobClientOptions options,
            ClientRouteTable routeTable,
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

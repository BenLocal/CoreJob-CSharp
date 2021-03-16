using System.Collections.Generic;
using System.Threading.Tasks;
using CoreJob.Client.Framework.Abstractions;
using CoreJob.Client.Framework.Models;
using CoreJob.Framework;
using CoreJob.Framework.Models.Request;
using CoreJob.Framework.Models.Response;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace CoreJob.Client.Framework
{
    public class CoreJobExecutor : ICoreJobExecutor
    {
        private readonly CoreJobClientOptions _options;

        private readonly IRestClient _httpClient;

        private readonly ILogger _logger;

        public CoreJobExecutor(CoreJobClientOptions options,
            IRestClient httpClient,
            ILogger<CoreJobExecutor> logger)
        {
            _options = options;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<bool> CallBackExecutor(List<CallBackResquest> callBackInfos)
        {
            return await InnnerAdminHttpClient<string>(CreateRestRequest($"{_options.ApiUriSegments}/callback", callBackInfos), "执行器回调");
        }

        public async Task<bool> RegistryExecutor()
        {
            var registryBody = new RegistryRequest
            {
                Key = _options.ExecutorAppName,
                Host = _options.ExecutorUrl
            };

            return await InnnerAdminHttpClient<string>(CreateRestRequest($"{_options.ApiUriSegments}/registry", registryBody), "注册执行器");
        }

        public async Task<bool> RemoveRegistryExecutor()
        {
            var registryBody = new RemoveRegistryRequest
            {
                Key = _options.ExecutorAppName,
                Host = _options.ExecutorUrl
            };

            return await InnnerAdminHttpClient<string>(CreateRestRequest($"{_options.ApiUriSegments}/removeRegistry", registryBody), "移除执行器");
        }

        private RestRequest CreateRestRequest(string url, object registryBody)
        {
            var request = new RestRequest(url, Method.POST);
            if (!string.IsNullOrEmpty(_options.Token))
            {
                request.AddHeader(JobConstant.Token, _options.Token);
            }

            request.AddRequestBody(_options.InputMessageType, registryBody);

            return request;
        }

        private async Task<bool> InnnerAdminHttpClient<T>(RestRequest request, string logActionName) where T : class
        {
            var response = await _httpClient.ExecuteAsync(request);

            var result = response.GetResponseData<T>();

            if (result?.Code == JobConstant.HTTP_SUCCESS_CODE)
            {
                return true;
            }

            _logger.LogError(response.ErrorException, $"corejob {logActionName} Http请求失败,{response.StatusCode},{response.ErrorMessage}");
            return false;
        }
    }
}

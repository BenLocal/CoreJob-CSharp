using System;
using System.Threading.Tasks;
using CoreJob.Framework;
using CoreJob.Framework.Models;
using CoreJob.Framework.Models.Request;
using CoreJob.Framework.Models.Response;
using CoreJob.Server.Framework.Abstractions;
using CoreJob.Server.Framework.Models;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace CoreJob.Server.Framework.Services
{
    public class ServerSender : IServerSender
    {
        private readonly CoreJobServerOptions _options;

        private readonly ILogger _logger;

        public ServerSender(CoreJobServerOptions options,
            ILogger<ServerSender> logger)
        {
            _options = options;
            _logger = logger;
        }

        public async Task<ResponseContext<string>> BeatAction(string host)
        {
            IRestClient _client = Create(host);

            var request = new RestRequest($"{_options.ApiUriSegments}/beat", Method.POST);
            if (_options.Token.NotNullOrEmpty())
            {
                request.AddHeader(JobConstant.Token, _options.Token);
            }

            var response = await _client.ExecuteAsync(request);
            _logger.LogInformation($"core job beat 请求完成，{response.IsSuccessful}, {response.StatusCode}");
            return response.GetResponseData<string>();
        }

        public async Task<ResponseContext<JobLogResponse>> LogAction(JobContext context)
        {
            IRestClient _client = Create(context.ClientHostUrl);

            var request = new RestRequest($"{_options.ApiUriSegments}/log", Method.POST);
            if (_options.Token.NotNullOrEmpty())
            {
                request.AddHeader(JobConstant.Token, _options.Token);
            }

            var logRequest = new JobLogRequest()
            {
                FromLineNum = context.LogFromLineNum,
                JobLogId = context.LogId,
                LogDateTime = context.LogDateTime
            };

            request.AddRequestBody(_options.InputMessageType, logRequest);
            var response = await _client.ExecuteAsync(request);
            _logger.LogInformation($"core job log 请求完成，{response.IsSuccessful}, {response.StatusCode}");
            return response.GetResponseData<JobLogResponse>();
        }

        public async Task<ResponseContext<string>> RunAction(JobContext context)
        {
            IRestClient _client = Create(context.ClientHostUrl);

            var request = new RestRequest($"{_options.ApiUriSegments}/run", Method.POST);
            if (_options.Token.NotNullOrEmpty())
            {
                request.AddHeader(JobConstant.Token, _options.Token);
            }

            var runRequest = new RunRequest()
            {
                JobId = context.Id,
                ExecutorHandler = context.Detail.ExecutorHandler,
                ExecutorParams = context.Detail.ExecutorParam,
                LogId = context.LogId,
                LogDateTime = context.LogDateTime
            };

            request.AddRequestBody(_options.InputMessageType, runRequest);
            var response = await _client.ExecuteAsync(request);
            _logger.LogInformation($"core job run 请求完成，{response.IsSuccessful}, {response.StatusCode}");
            return response.GetResponseData<string>();
        }

        private IRestClient Create(string host)
        {
            IRestClient _client = new RestClient(host);
            _client.RemoteCertificateValidationCallback += 
                (sender, certificate, chain, sslPolicyErrors) => true;

            return _client;
        }
    }
}

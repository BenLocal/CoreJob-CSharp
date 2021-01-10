using System;
using System.Threading;
using System.Threading.Tasks;
using CoreJob.Client.Framework.Abstractions;
using CoreJob.Client.Framework.Models;
using Microsoft.Extensions.Logging;

namespace CoreJob.Client.Framework.Services
{
    public class ExecutorRegistryProcessor : IProcessor
    {
        private readonly ILogger _logger;

        private readonly CoreJobClientOptions _options;

        private readonly ICoreJobExecutor _xxlJobExecutor;

        public ExecutorRegistryProcessor(ILogger<ExecutorRegistryProcessor> logger,
            CoreJobClientOptions options,
            ICoreJobExecutor xxlJobExecutor)
        {
            _logger = logger;
            _options = options;
            _xxlJobExecutor = xxlJobExecutor;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Task.Run(() => Stop(new CancellationToken()));
            }
        }

        public Task Start(CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(async () =>
            {
                var heartbeatIntervalSecond = Math.Max(_options.HeartbeatIntervalSecond, 30);
                _logger.LogInformation($"开始定时注册执行器,间隔{heartbeatIntervalSecond}秒......");
                try
                {
                    // Registry
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            await _xxlJobExecutor.RegistryExecutor();
                            await Task.Delay(TimeSpan.FromSeconds(heartbeatIntervalSecond), cancellationToken);
                        }
                        catch (TaskCanceledException)
                        {
                            _logger.LogInformation("程序关闭");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "core注册执行器错误，重试中......");
                            await Task.Delay(TimeSpan.FromSeconds(heartbeatIntervalSecond), cancellationToken);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    //ignore
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                }
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            return Task.CompletedTask;
        }

        public async Task Stop(CancellationToken cancellationToken)
        {
            try
            {
                await _xxlJobExecutor.RemoveRegistryExecutor();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "corejob删除执行器错误");
            }
        }
    }
}

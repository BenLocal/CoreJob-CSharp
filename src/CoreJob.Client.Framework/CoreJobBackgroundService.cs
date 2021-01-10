using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using  CoreJob.Client.Framework.Abstractions;
using CoreJob.Framework;

namespace CoreJob.Client.Framework
{
    public class CoreJobBackgroundService : IHostedService, IDisposable
    {
        private bool _started = false;

        private readonly IServiceProvider _provider;

        private readonly IProcessor _processor;

        public CoreJobBackgroundService(IServiceProvider provider,
            IProcessor processor)
        {
            _provider = provider;
            _processor = processor;
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
                JobUtilExtensions.RunSync(async () => await StopInnerAsync(new CancellationToken()));
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (!_started)
            {
                await _processor.Start(cancellationToken);

                _started = true;
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await StopInnerAsync(cancellationToken);
        }

        private async Task StopInnerAsync(CancellationToken cancellationToken)
        {
            if (_started)
            {
                await _processor.Stop(cancellationToken);
                _started = false;
            }
        }
    }
}

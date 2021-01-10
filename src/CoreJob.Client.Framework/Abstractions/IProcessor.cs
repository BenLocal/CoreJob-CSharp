using System;
using System.Threading;
using System.Threading.Tasks;

namespace  CoreJob.Client.Framework.Abstractions
{
    public interface IProcessor : IDisposable
    {
        Task Start(CancellationToken cancellationToken);

        Task Stop(CancellationToken cancellationToken);
    }
}

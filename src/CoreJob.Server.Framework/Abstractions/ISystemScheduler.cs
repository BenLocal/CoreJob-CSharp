using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoreJob.Server.Framework.Abstractions
{
    public interface ISystemScheduler
    {
        Task Start();
    }
}

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreJob.Framework.Abstractions
{
    public interface IJobOptionsExtension
    {
        /// <summary>
        /// Registered service
        /// </summary>
        /// <param name="services"></param>
        void AddServices(IServiceCollection services);
    }
}

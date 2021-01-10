using System;
using System.Collections.Concurrent;
using CoreJob.Client.Framework.Abstractions;
using Microsoft.Extensions.Logging;

namespace CoreJob.Client.Framework.Logger
{
    public class CustomJobLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        private readonly JobLoggerConfiguration _config;
        private readonly ConcurrentDictionary<string, CustomJobLogger> _loggers = new ConcurrentDictionary<string, CustomJobLogger>();
        private readonly IJobLoggerStore _jobLogger;
        internal IExternalScopeProvider ScopeProvider;
        protected IDisposable SettingsChangeToken;

        public CustomJobLoggerProvider(JobLoggerConfiguration config, IJobLoggerStore jobLogger)
        {
            _config = config;
            _jobLogger = jobLogger;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new CustomJobLogger(name, this));
        }

        public JobLoggerConfiguration Config => _config;

        public IJobLoggerStore JobLoggerStore => _jobLogger;

        public void SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            ScopeProvider = scopeProvider;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);  // instructs GC not bother to call the destructor  
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _loggers.Clear();
            }
        }
    }
}

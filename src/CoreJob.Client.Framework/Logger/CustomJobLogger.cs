using System;
using System.Collections.Generic;
using System.Linq;
using CoreJob.Client.Framework.Models;
using CoreJob.Framework;
using Microsoft.Extensions.Logging;

namespace CoreJob.Client.Framework.Logger
{
    public class CustomJobLogger : ILogger
    {
        private readonly string _name;

        private readonly CustomJobLoggerProvider _provider;

        public CustomJobLogger(string name,
            CustomJobLoggerProvider provider)
        {
            _name = name;
            _provider = provider;
        }


        public IDisposable BeginScope<TState>(TState state) => _provider.ScopeProvider.Push(state);

        public bool IsEnabled(LogLevel logLevel) => logLevel == LogLevel.Information;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var scopes = GetScopes(state);
            if (scopes.Exists(x => x.Properties.ContainsKey(JobConstant.LOGGER_SCOPE_JOBAREA_KEY) &&
                x.Properties[JobConstant.LOGGER_SCOPE_JOBAREA_KEY]?.ToString() == JobConstant.LOGGER_SCOPE_JOBAREA_VALUE))
            {
                var scope = scopes.FirstOrDefault(x => x.Properties.ContainsKey(JobConstant.LOGGER_SCOPE_JOBAREA_KEY) &&
                    x.Properties[JobConstant.LOGGER_SCOPE_JOBAREA_KEY]?.ToString() == JobConstant.LOGGER_SCOPE_JOBAREA_VALUE);
                if (scope.Properties.TryGetValue(JobConstant.LOGGER_SCOPE_JOBID_KEY, out var jobId) &&
                    scope.Properties.TryGetValue(JobConstant.LOGGER_SCOPE_JOBLOGID_KEY, out var jobLogId))
                {
                    _provider.JobLoggerStore.Save(new JobLoggerInfo()
                    {
                        Message = $"[{_name}],[{formatter(state, exception)}]",
                        InData = DateTime.Now,
                        JobId = Convert.ToInt32(jobId),
                        JobLogId = Convert.ToInt32(jobLogId)
                    });
                }
            }
        }

        private List<JobLogScopeInfo> GetScopes<TState>(TState state)
        {
            List<JobLogScopeInfo> scopes = new List<JobLogScopeInfo>();
            if (_provider.ScopeProvider != null)
            {
                _provider.ScopeProvider.ForEachScope((value, loggingProps) =>
                {
                    JobLogScopeInfo scope = new JobLogScopeInfo();
                    if (value is string)
                    {
                        scope.Text = value.ToString();
                    }
                    else if (value is IEnumerable<KeyValuePair<string, object>> props)
                    {
                        if (scope.Properties == null)
                            scope.Properties = new Dictionary<string, object>();

                        foreach (var pair in props)
                        {
                            scope.Properties[pair.Key] = pair.Value;
                        }
                    }

                    scopes.Add(scope);
                },
                state);
            }

            return scopes;
        }
    }

    public class JobLoggerConfiguration
    {
    }


    public class JobLogScopeInfo
    {
        public string Text { get; set; }
        public Dictionary<string, object> Properties { get; set; }
    }
}

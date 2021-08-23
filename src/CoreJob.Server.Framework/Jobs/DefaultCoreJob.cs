using System;
using System.Threading.Tasks;
using CoreJob.Framework;
using CoreJob.Framework.Models;
using CoreJob.Server.Framework.Abstractions;
using CoreJob.Server.Framework.Models;
using CoreJob.Server.Framework.Store;
using Quartz;

namespace CoreJob.Server.Framework.Jobs
{
    public class DefaultCoreJob : IJob
    {
        private readonly IServerSender _serverSender;

        private readonly CoreJobServerOptions _options;

        public DefaultCoreJob(IServerSender serverSender,
            CoreJobServerOptions options)
        {
            _serverSender = serverSender;
            _options = options;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var hasError = context.Trigger.JobDataMap.GetBoolean(JobConstant.MAP_DATA_HAS_ERROR);
            if (hasError)
            {
                return;
            }

            JobInfo job = context.Trigger.JobDataMap.GetMapData<JobInfo>();
            var logInfo = context.Trigger.JobDataMap.GetMapData<JobLog>();
            var host = context.Trigger.JobDataMap.GetString(JobConstant.MAP_DATA_EXECUTER_HOST_KEY);
            var result = await _serverSender.RunAction(new JobContext(job)
            {
                ClientHostUrl = host,
                LogId = logInfo.Id,
                LogDateTime = logInfo.StartTime.GetTimeStamp()
            });

            if (result.Success())
            {
                context.Trigger.JobDataMap.SetMapData(result);
                return;
            }

            if (!result.Health())
            { 
                // 网络异常重试
            }

            context.Trigger.JobDataMap.SetMapData(result);
        }
    }
}

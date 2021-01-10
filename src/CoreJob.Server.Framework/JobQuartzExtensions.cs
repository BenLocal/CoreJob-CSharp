using System;
using System.Collections.Generic;
using System.Text;
using CoreJob.Framework;
using CoreJob.Framework.Models;
using Quartz;

namespace CoreJob.Server.Framework
{
    public static class JobQuartzExtensions
    {
        public static void SetSchedulerContextData<T>(this SchedulerContext context, T data) where T : class
        {
            context.Put(typeof(T).FullName, data);
        }

        public static T GetSchedulerContextData<T>(this SchedulerContext context)
        {
            return (T) context.Get(typeof(T).FullName);
        }

        public static void SetMapData<T>(this JobDataMap map, T data) where T : class
        {
            map.Put(typeof(T).FullName, data);
        }

        public static T GetMapData<T>(this JobDataMap map)
        {
            return (T) map.Get(typeof(T).FullName);
        }

        public static JobKey GetJobKey(this JobInfo job) => job.Id.GetJobKey();

        public static JobKey GetJobKey(this int jobId)
        {
            return JobKey.Create(jobId.ToString(), JobConstant.Job_Default_Group);
        }

        public static TriggerKey GetTriggerKey(this JobInfo job) => job.Id.GetTriggerKey();

        public static TriggerKey GetTriggerKey(this int jobId)
        {
            return new TriggerKey(jobId.ToString(), JobConstant.Job_Default_Group);
        }

        public static JobKey GetSystemJobKey(this Type type)
        {
            return JobKey.Create(type.FullName, JobConstant.Job_System_Group);
        }

        public static TriggerKey GetSystemTriggerKey(this Type type)
        {
            return new TriggerKey(type.FullName, JobConstant.Job_System_Group);
        }
    }
}

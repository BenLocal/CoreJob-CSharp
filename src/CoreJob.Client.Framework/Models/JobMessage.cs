using CoreJob.Framework;
using CoreJob.Framework.Models.Request;

namespace CoreJob.Client.Framework.Models
{
    public class JobMessage
    {
        private readonly RunRequest _request;

        private readonly object _lockObj = new object();

        public JobMessage(RunRequest request)
        {
            _request = request;
        }

        /// <summary>
        /// 当前任务是否已停止
        /// </summary>
        public JobStatus Status { get; set; }

        /// <summary>
        /// 停止原因
        /// </summary>
        public string Reason { get; set; }

        public int CallBackCode { get; set; } = JobConstant.HTTP_SUCCESS_CODE;

        public int Id => _request.JobId;

        public string ExecutorHandler => _request.ExecutorHandler;

        public string ExecutorParams => _request.ExecutorParams;

        public int LogId => _request.LogId;

        public bool Kill()
        {
            lock (_lockObj)
            {
                if (Status == JobStatus.Created)
                {
                    Status = JobStatus.Killed;
                    return true;
                }
            }
            return false;
        }

        public bool Run()
        {
            lock (_lockObj)
            {
                if (Status == JobStatus.Created)
                {
                    Status = JobStatus.Running;
                    return true;
                }
            }
            return false;
        }
    }

    public enum JobStatus
    {
        /// <summary>
        ///初始状态  待执行
        /// </summary>
        Created = 0,

        /// <summary>
        /// 运行中
        /// </summary>
        Running = 1,

        /// <summary>
        /// 已终止
        /// </summary>
        Killed = 2,

        /// <summary>
        /// 执行成功
        /// </summary>
        Success = 3
    }
}

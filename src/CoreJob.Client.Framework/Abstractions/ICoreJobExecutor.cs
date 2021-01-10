using System.Collections.Generic;
using System.Threading.Tasks;
using CoreJob.Framework.Models.Request;

namespace CoreJob.Client.Framework.Abstractions
{
    public interface ICoreJobExecutor
    {
        /// <summary>
        /// 执行器注册时使用，调度中心会实时感知注册成功的执行器并发起任务调度
        /// 
        /// 地址格式：{调度中心跟地址}/registry
        /// 
        /// Header：
        ///    XXL-JOB-ACCESS-TOKEN : {请求令牌}
        ///    
        /// 请求数据格式如下，放置在 RequestBody 中，JSON格式：
        ///    {
        ///        "registryKey":"xxl-job-executor-example",       // 执行器AppName
        ///        "registryValue":"http://127.0.0.1:9999/"        // 执行器地址，内置服务跟地址
        ///    }
        /// 响应数据格式：
        ///    {
        ///      "code": 200,      // 200 表示正常、其他失败
        ///      "msg": null      // 错误提示消息
        ///    }
        /// </summary>
        /// <returns></returns>
        Task<bool> RegistryExecutor();

        /// <summary>
        /// 执行器注册摘除时使用，注册摘除后的执行器不参与任务调度与执行
        /// 
        /// 地址格式：{调度中心跟地址}/registryRemove
        /// 
        /// Header：
        ///    XXL-JOB-ACCESS-TOKEN : {请求令牌}
        ///    
        /// 请求数据格式如下，放置在 RequestBody 中，JSON格式：
        ///    {
        ///        "registryKey":"xxl-job-executor-example",       // 执行器AppName
        ///        "registryValue":"http://127.0.0.1:9999/"        // 执行器地址，内置服务跟地址
        ///    }
        ///    
        /// 响应数据格式：
        ///    {
        ///      "code": 200,      // 200 表示正常、其他失败
        ///      "msg": null      // 错误提示消息
        ///    }
        /// </summary>
        /// <returns></returns>
        Task<bool> RemoveRegistryExecutor();

        /// <summary>
        /// 说明：执行器执行完任务后，回调任务结果时使用
        /// ------
        /// 地址格式：{调度中心跟地址}/callback
        ///
        /// Header：
        ///    XXL-JOB-ACCESS-TOKEN : {请求令牌}
        ///    
        /// 请求数据格式如下，放置在 RequestBody 中，JSON格式：
        ///    [{
        ///        "logId":1,              // 本次调度日志ID
        ///        "logDateTim":0,         // 本次调度日志时间
        ///        "executeResult":{
        ///            "code": 200,        // 200 表示任务执行正常，500表示失败
        ///            "msg": null
        ///        }
        ///    }]
        /// 响应数据格式：
        ///    {
        ///      "code": 200,      // 200 表示正常、其他失败
        ///      "msg": null      // 错误提示消息
        ///    }
        /// </summary>
        /// <param name="callBackInfos"></param>
        /// <returns></returns>
        Task<bool> CallBackExecutor(List<CallBackResquest> callBackInfos);
    }
}

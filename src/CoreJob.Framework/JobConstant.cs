using System;
using System.Collections.Generic;
using System.Text;

namespace CoreJob.Framework
{
    public static class JobConstant
    {
        public const string Msgpack_ContentType = "application/x-msgpack";

        public const string Json_ContentType = "application/json";

        /// <summary>
        /// token标识key
        /// </summary>
        public const string Token = "X-COREJOB-ACCESS-TOKEN";

        public const int HTTP_SUCCESS_CODE = 200;

        public const int HTTP_FAIL_CODE = 500;

        /// <summary>
        /// SERIAL_EXECUTION=单机串行
        /// </summary>
        public const string EXECUTORBLOCKSTRATEGY_SERIAL_EXECUTION = "SERIAL_EXECUTION";

        /// <summary>
        /// DISCARD_LATER=丢弃后续调度
        /// </summary>
        public const string EXECUTORBLOCKSTRATEGY_DISCARD_LATER = "DISCARD_LATER";

        /// <summary>
        /// COVER_EARLY=覆盖之前调度 
        /// </summary>
        public const string EXECUTORBLOCKSTRATEGY_COVER_EARLY = "COVER_EARLY";

        public const string LOGGER_SCOPE_JOBID_KEY = "id";

        public const string LOGGER_SCOPE_JOBLOGID_KEY = "logId";

        public const string LOGGER_SCOPE_JOBAREA_KEY = "area";

        public const string LOGGER_SCOPE_JOBAREA_VALUE = "job_handler";

        public const string MAP_DATA_EXECUTER_HOST_KEY = "ExecuterHost";

        public const string Job_Default_Group = "default";

        public const string Job_System_Group = "system";
    }
}

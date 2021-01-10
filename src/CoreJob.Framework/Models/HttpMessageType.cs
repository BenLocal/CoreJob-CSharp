using System.ComponentModel;

namespace CoreJob.Framework.Models
{
    public enum HttpMessageType
    {
        [Description(JobConstant.Msgpack_ContentType)]
        MSGPACK,

        [Description(JobConstant.Json_ContentType)]
        JSON
    }
}

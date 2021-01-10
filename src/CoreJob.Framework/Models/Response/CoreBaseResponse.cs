using System;
using MessagePack;

namespace CoreJob.Framework.Models.Response
{
    [MessagePackObject]
    public class CoreBaseResponse<T>
    {
        [Key(0)]
        public int Code { get; set; }

        [Key(1)]
        public string Msg { get; set; }

        [Key(2)]
        public T Content { get; set; }
    }

    public struct EmptyResponse
    {

    }

    public class ResponseContext<T> : CoreBaseResponse<T>
    {
        public System.Net.HttpStatusCode StatusCode { get; set; }

        public bool Success()
        {
            return Health() && Code == JobConstant.HTTP_SUCCESS_CODE;
        }

        public bool Health()
        {
            return StatusCode == System.Net.HttpStatusCode.OK;
        }
    }

    public static class CoreBaseExtensions
    {
        public static ResponseContext<T> CreateContext<T>(this CoreBaseResponse<T> baseCore)
        {
            return new ResponseContext<T>()
            {
                Code = baseCore.Code,
                Msg = baseCore.Msg,
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = baseCore.Content
            };
        }

        public static ResponseContext<T> CreateFailContext<T>(this System.Net.HttpStatusCode statusCode, string error)
        {
            return new ResponseContext<T>()
            {
                Code = JobConstant.HTTP_FAIL_CODE,
                Msg = error,
                StatusCode = statusCode
            };
        }

        public static ResponseContext<T> CreateFailContext<T>(this Exception e)
        {
            return new ResponseContext<T>()
            {
                Code = JobConstant.HTTP_FAIL_CODE,
                Msg = e.Message,
                StatusCode = System.Net.HttpStatusCode.BadRequest,
            };
        }
        public static CoreBaseResponse<string> Fail(this string errorMessage)
            => Fail<string>(errorMessage);

        public static CoreBaseResponse<T> Fail<T>(this string errorMessage)
        {
            if (errorMessage.NullOrEmpty())
            {
                errorMessage = "处理错误";
            }

            return new CoreBaseResponse<T>()
            {
                Code = JobConstant.HTTP_FAIL_CODE,
                Msg = errorMessage
            };
        }

        public static CoreBaseResponse<string> Success(this string msg)
            => Success<string>(msg);

        public static CoreBaseResponse<T> Success<T>(this T content)
        {
            return new CoreBaseResponse<T>()
            {
                Code = JobConstant.HTTP_SUCCESS_CODE,
                Msg = string.Empty,
                Content = content
            };
        }
    }
}

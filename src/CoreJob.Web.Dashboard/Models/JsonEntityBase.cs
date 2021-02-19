using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using FluentValidation.Results;
using static CoreJob.Web.Dashboard.Models.JsonEntityExtensions;

namespace CoreJob.Web.Dashboard.Models
{
    public class JsonEntityBase
    {
        [JsonPropertyName("cod")]
        public string Code { set; get; }

        [JsonPropertyName("msg")]
        public string Message { get; set; }

        [JsonPropertyName("fs")]
        public IEnumerable<FailureInfo> Failures { get; set; }

        [JsonPropertyName("suc")]
        public bool IsSuccess { get; set; }
    }

    public class JsonEntityDataBase<T> : JsonEntityBase
    {
        [JsonPropertyName("data")]
        public T Data { get; set; }
    }

    public class JsonEntityListDataBase<T> : JsonEntityBase
    {
        [JsonPropertyName("data")]
        public List<T> Data { get; set; }
    }

    public class EmptyJson : JsonEntityBase
    {
        private static EmptyJson _current = new EmptyJson() 
        {
            Code = "ok",
            Message = string.Empty,
            IsSuccess = true,
        };

        public static JsonEntityBase Success()
        {
            return _current;
        }
    }

    public static class JsonEntityExtensions
    {
        private static JsonEntityDataBase<T> SuccessInner<T>(this T data) where T : class
        {
            return new JsonEntityDataBase<T>()
            {
                Code = "ok",
                Message = string.Empty,
                IsSuccess = true,
                Data = data
            };
        }

        private static JsonEntityListDataBase<T> SuccessInner<T>(this List<T> data) where T : class
         => new JsonEntityListDataBase<T>()
         {
             Code = "ok",
             Message = string.Empty,
             IsSuccess = true,
             Data = data
         };

        public static JsonEntityBase Success<T>(this List<T> data) where T : class => SuccessInner(data);

        public static JsonEntityBase Success<T>(this T data) where T : class => SuccessInner(data);

        public static JsonEntityBase Success(this object data) => SuccessInner(data);

        public static JsonEntityBase Success(this List<object> data) => SuccessInner(data);

        public static JsonEntityBase Error(this IEnumerable<ValidationFailure> failures)
        {
            return new JsonEntityBase()
            {
                IsSuccess = false,
                Code = "err_9999",
                Message = "发生错误",
                Failures = failures.Select(x => new FailureInfo()
                {
                    Field = x.PropertyName,
                    Reason = x.ErrorMessage
                })
            };
        }

        public static JsonEntityBase Error(this string error)
        {
            return new JsonEntityBase()
            {
                IsSuccess = false,
                Code = "err_0001",
                Message = error,
            };
        }

        public class FailureInfo
        { 
            public string Field { get; set; }

            public string Reason { get; set; }
        }
    }
}

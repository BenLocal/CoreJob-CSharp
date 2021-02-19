using System.Text;
using CoreJob.Framework.Json.Extensions;
using Microsoft.AspNetCore.Http;

namespace CoreJob.Framework.Models.HttpAction
{
    public class JsonActionResponse : IActionResponse
    {
        public JsonActionResponse(HttpContext context, object value)
        {
            this.HttpContext = context;
            this.Result = value;
        }

        public object Result { get; set; }

        public HttpContext HttpContext { get; set; }

        public byte[] ExecuteResult(HttpContext context)
        {
            if (this.Result == null)
            {
                return new byte[0];
            }

            return Encoding.UTF8.GetBytes(this.Result.SerializeSnakeCaseObject());
        }
    }
}

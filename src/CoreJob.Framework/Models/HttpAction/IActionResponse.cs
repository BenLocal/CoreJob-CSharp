using Microsoft.AspNetCore.Http;

namespace CoreJob.Framework.Models.HttpAction
{
    public interface IActionResponse
    {
        object Result { get; set; }

        HttpContext HttpContext { get; set; }

        byte[] ExecuteResult(HttpContext context);
    }
}

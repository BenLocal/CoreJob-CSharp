namespace CoreJob.Framework.Models
{
    public class JobOptions
    {
        public HttpMessageType InputMessageType { get; set; } = HttpMessageType.JSON;

        public HttpMessageType OutputMessageType { get; set; } = HttpMessageType.JSON;

        /// <summary>
        /// 访问token
        /// </summary>
        public string Token { get; set; }

        public string ApiUriSegments { get; set; } = "/api/core/job";
    }
}

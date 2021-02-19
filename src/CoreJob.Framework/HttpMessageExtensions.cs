using System;
using System.Buffers;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CoreJob.Framework.Models;
using CoreJob.Framework.Models.HttpAction;
using CoreJob.Framework.Models.Response;
using CoreJob.Framework.Json.Extensions;
using MessagePack;
using Microsoft.AspNetCore.Http;
using RestSharp;

namespace CoreJob.Framework
{
    public static class HttpMessageExtensions
    {
        public static void AddRequestBody<T>(this IRestRequest restRequest, HttpMessageType httpMessageType, T obj)
        {
            switch (httpMessageType)
            {
                case HttpMessageType.MSGPACK:
                    restRequest.AddParameter(string.Empty, obj.SerializeMsgPack(),
                            JobConstant.Msgpack_ContentType, ParameterType.RequestBody);
                    break;
                case HttpMessageType.JSON:
                    restRequest.AddParameter(string.Empty, obj.SerializeSnakeCaseObject(),
                            JobConstant.Json_ContentType, ParameterType.RequestBody);
                    break;
            }
        }

        public static ResponseContext<T> GetResponseData<T>(this IRestResponse restResponse)
        {
            if (restResponse.IsSuccessful && restResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var contentType = restResponse.ContentType.ToLower().Trim();
                if (contentType.Contains(JobConstant.Json_ContentType))
                {
                    return restResponse.Content.DeserializeSnakeCaseObject<CoreBaseResponse<T>>().CreateContext();
                }
                else if (contentType.Contains(JobConstant.Msgpack_ContentType))
                {
                    return restResponse.RawBytes.DeserializeMsgPack<CoreBaseResponse<T>>().CreateContext();
                }
            }

            return restResponse.StatusCode.CreateFailContext<T>(restResponse?.ErrorException?.Message ?? "执行服务器错误");
        }

        public static async Task<T> FromHttpRequestBody<T>(this HttpRequest request) where T : class
        {
            var contentType = request.ContentType.ToLower().Trim();
            if (contentType.Contains(JobConstant.Json_ContentType))
            {
                return await FromJsonBody<T>(request);
            }
            else if (contentType.Contains(JobConstant.Msgpack_ContentType))
            {
                return await FromMessagePackBody<T>(request);
            }

            return default;
        }

        public static async Task<IActionResponse> ResponseAsync(this HttpContext context, object obj)
        {
            var contentType = context.Response.ContentType.ToLower().Trim();
            if (contentType.Contains(JobConstant.Json_ContentType))
            {
                return await Task.FromResult(new JsonActionResponse(context, obj));
            }
            else if (contentType.Contains(JobConstant.Msgpack_ContentType))
            {
                return await Task.FromResult(new MsgPackActionResponse(context, obj));
            }

            throw new Exception("响应ContentType错误");
        }

        public static async Task<byte[]> ResponseObjAsync(this HttpContext context, object obj)
        {
            var response = await ResponseAsync(context, obj);
            return response.ExecuteResult(context);
        }

        public static async Task WriteBytesAsync(this Microsoft.AspNetCore.Http.HttpResponse response, byte[] bytes)
        {
            await response.Body.WriteAsync(bytes, 0, bytes.Length);
        }

        private static async Task<T> FromJsonBody<T>(this HttpRequest request) where T : class
        {
            var stream = request.Body;
            var reader = new StreamReader(stream);
            var contentFromBody = await reader.ReadToEndAsync();
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
            return contentFromBody.DeserializeSnakeCaseObject<T>();
        }

        private static async Task<T> FromMessagePackBody<T>(this HttpRequest request) where T : class
        {
            var bytes = await ReadToEnd(request.Body);
            return bytes.DeserializeMsgPack<T>();
        }

        public static async Task<byte[]> ReadToEnd(Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = await stream.ReadAsync(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }
    }
}

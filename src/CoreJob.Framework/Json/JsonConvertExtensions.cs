using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CoreJob.Framework.Json.Extensions
{
    public static class JsonConvertExtensions
    {
        public static T DeserializeObject<T>(this string str, JsonSerializerOptions options = null) where T : class
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            try
            {
                return JsonSerializer.Deserialize<T>(str, options);
            }
            catch(Exception)
            {
                return null;
            }
        }

        public static async ValueTask<T> DeserializeObjectAsync<T>(this string str, JsonSerializerOptions options = null) where T : class
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            try
            {
                using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(str)))
                {
                    return await JsonSerializer.DeserializeAsync<T>(stream, options);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string SerializeObject<T>(this T obj, JsonSerializerOptions options = null)
        {
            if (obj == null)
            {
                return null;
            }

            try
            {
                return JsonSerializer.Serialize(obj, options);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static T DeserializeSnakeCaseObject<T>(this string str, JsonSerializerOptions options = null) where T : class
            => DeserializeObject<T>(str, new JsonSerializerOptions(options)
            {
                PropertyNamingPolicy = new SnakeCaseNamingPolicySpan()
            });

        public static string SerializeSnakeCaseObject<T>(this T obj, JsonSerializerOptions options = null)
           => SerializeObject<T>(obj, new JsonSerializerOptions(options)
           {
               PropertyNamingPolicy = new SnakeCaseNamingPolicySpan()
           });
    }

    public class JsonDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Debug.Assert(typeToConvert == typeof(DateTime));
            return DateTime.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}

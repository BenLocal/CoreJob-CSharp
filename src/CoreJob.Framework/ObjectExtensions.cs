using System;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using MessagePack;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CoreJob.Framework
{
    public static class ObjectExtensions
    {
        public static bool NotNullOrEmpty(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        public static bool NullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool EmptyString(this string value)
        {
            return value != null && value == "";
        }

        public static bool IsNull(this string value)
        {
            return value == null;
        }

        public static bool NotNull(this string value)
        { 
            return value != null;
        }

        public static string GetDescription<T>(this T enumItem) where T : struct
        {
            FieldInfo fi = enumItem.GetType().GetField(enumItem.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            return enumItem.ToString();
        }

        public static int GetValue(this Enum em)
        {
            try
            {
                return Convert.ToInt32(em);
            }
            catch
            {
                return -1;
            }
        }

        public static byte[] SerializeMsgPack<T>(this T obj)
        {
            return MessagePackSerializer.Serialize(obj, MessagePack.Resolvers.ContractlessStandardResolver.Options);
        }

        public static byte[] SerializeMsgPack<T>(this T obj, MessagePackSerializerOptions serializerOptions)
        {
            return MessagePackSerializer.Serialize(obj, serializerOptions);
        }

        public static T DeserializeMsgPack<T>(this byte[] bytes)
        {
            return MessagePackSerializer.Deserialize<T>(bytes, MessagePack.Resolvers.ContractlessStandardResolver.Options);
        }

        public static T DeserializeObject<T>(this string str, JsonSerializerSettings settings) where T : class
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(str, settings);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static T DeserializeObject<T>(this string str) where T : class
            => DeserializeObject<T>(str, null);

        public static T DeserializeSnakeCaseObject<T>(this string str) where T : class
            => DeserializeObject<T>(str, new JsonSerializerSettings()
            {
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            });

        public static string SerializeObject(this object obj, JsonSerializerSettings settings)
        {
            if (obj == null)
            {
                return null;
            }

            try
            {
                return JsonConvert.SerializeObject(obj, settings);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string SerializeObject(this object obj)
            => SerializeObject(obj, null);

        public static string SerializeSnakeCaseObject(this object obj)
           => SerializeObject(obj, new JsonSerializerSettings()
           {
               ContractResolver = new DefaultContractResolver
               {
                   NamingStrategy = new SnakeCaseNamingStrategy()
               }
           });

        public static string ToHexString(this byte[] hex)
        {
            if (hex == null) return null;
            if (hex.Length == 0) return string.Empty;

            var s = new StringBuilder();
            foreach (byte b in hex)
            {
                s.Append(b.ToString("x2"));
            }
            return s.ToString();
        }

        public static byte[] ToHexBytes(this string hex)
        {
            if (hex == null) return null;
            if (hex.Length == 0) return new byte[0];

            int l = hex.Length / 2;
            var b = new byte[l];
            for (int i = 0; i < l; ++i)
            {
                b[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return b;
        }

    }
}

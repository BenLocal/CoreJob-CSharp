using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using MessagePack;

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

        public static bool NotNullOrEmptyList<T>(this ICollection<T> value)
        {
            return value != null && value.Any();
        }

        public static bool IsNullOrEmptyList<T>(this ICollection<T> value)
        {
            return value == null || !value.Any();
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
    }
}

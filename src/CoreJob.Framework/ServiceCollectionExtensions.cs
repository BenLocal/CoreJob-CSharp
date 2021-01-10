using System;
using System.ComponentModel;
using System.Reflection;
using CoreJob.Framework.Models.HttpAction;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoreJob.Framework
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRouteTable<T>(this IServiceCollection services, T routeTable) where T : RouteTable
        {
            services.AddSingleton(routeTable);
            foreach (var route in routeTable)
            {
                services.AddScoped(route.TypeController);
            }

            services.AddSingleton<RequestHandler>();

            return services;
        }

        public static IServiceCollection AddOptionConfiguration<T>(this IServiceCollection services, IConfiguration configuration, T options)
        {
            var allProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach(var prop in allProperties)
            {
                var value = configuration.GetEnvironmentOrConfigStr($"CoreJob:{prop.Name}");
                if (value != null)
                {
                    prop.SetValue(options, ToTypeInner(value, prop.PropertyType));
                }
            }
            return services;
        }

        private static object ToTypeInner(object value, Type targetType)
        {
            var converter = TypeDescriptor.GetConverter(targetType);
            var valueType = value.GetType();

            if (targetType.IsAssignableFrom(valueType))
                return value;

            var targetTypeInfo = targetType.GetTypeInfo();
            if (targetTypeInfo.IsEnum && (value is string || valueType.GetTypeInfo().IsEnum))
            {
                // attempt to match enum by name.
                if (TryEnumIsDefined(targetType, value.ToString()))
                {
                    object parsedValue = Enum.Parse(targetType, value.ToString(), false);
                    return parsedValue;
                }

                string message = $"The Enum value of '{value}' is not defined as a valid value for '{targetType.FullName}'.";
                throw new ArgumentException(message);
            }

            if (targetTypeInfo.IsEnum && valueType.IsNumeric())
                return Enum.ToObject(targetType, value);

            if (converter.CanConvertFrom(valueType))
            {
                object convertedValue = converter.ConvertFrom(value);
                return convertedValue;
            }

            if (!(value is IConvertible))
                throw new ArgumentException($"An incompatible value specified.  Target Type: {targetType.FullName} Value Type: {value.GetType().FullName}", nameof(value));
            try
            {
                object convertedValue = Convert.ChangeType(value, targetType);
                return convertedValue;
            }
            catch (Exception e)
            {
                throw new ArgumentException($"An incompatible value specified.  Target Type: {targetType.FullName} Value Type: {value.GetType().FullName}", nameof(value), e);
            }
        }

        private static bool TryEnumIsDefined(Type type, object value)
        {
            if (type == null || value == null || !type.GetTypeInfo().IsEnum)
                return false;

            // Return true if the value is an enum and is a matching type.
            if (type == value.GetType())
                return true;

            if (TryEnumIsDefined<int>(type, value))
                return true;
            if (TryEnumIsDefined<string>(type, value))
                return true;
            if (TryEnumIsDefined<byte>(type, value))
                return true;
            if (TryEnumIsDefined<short>(type, value))
                return true;
            if (TryEnumIsDefined<long>(type, value))
                return true;
            if (TryEnumIsDefined<sbyte>(type, value))
                return true;
            if (TryEnumIsDefined<ushort>(type, value))
                return true;
            if (TryEnumIsDefined<uint>(type, value))
                return true;
            if (TryEnumIsDefined<ulong>(type, value))
                return true;

            return false;
        }

        private static bool TryEnumIsDefined<T>(Type type, object value)
        {
            // Catch any casting errors that can occur or if 0 is not defined as a default value.
            try
            {
                if (value is T && Enum.IsDefined(type, (T)value))
                    return true;
            }
            catch (Exception) { }

            return false;
        }

        private static bool IsNumeric(this Type type)
        {
            if (type.IsArray)
                return false;

            if (type == TypeHelper.ByteType ||
                type == TypeHelper.DecimalType ||
                type == TypeHelper.DoubleType ||
                type == TypeHelper.Int16Type ||
                type == TypeHelper.Int32Type ||
                type == TypeHelper.Int64Type ||
                type == TypeHelper.SByteType ||
                type == TypeHelper.SingleType ||
                type == TypeHelper.UInt16Type ||
                type == TypeHelper.UInt32Type ||
                type == TypeHelper.UInt64Type)
                return true;

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
            }

            return false;
        }

        private struct TypeHelper
        {
            public static readonly Type ObjectType = typeof(object);
            public static readonly Type StringType = typeof(string);
            public static readonly Type CharType = typeof(char);
            public static readonly Type NullableCharType = typeof(char?);
            public static readonly Type DateTimeType = typeof(DateTime);
            public static readonly Type NullableDateTimeType = typeof(DateTime?);
            public static readonly Type BoolType = typeof(bool);
            public static readonly Type NullableBoolType = typeof(bool?);
            public static readonly Type ByteArrayType = typeof(byte[]);
            public static readonly Type ByteType = typeof(byte);
            public static readonly Type SByteType = typeof(sbyte);
            public static readonly Type SingleType = typeof(float);
            public static readonly Type DecimalType = typeof(decimal);
            public static readonly Type Int16Type = typeof(short);
            public static readonly Type UInt16Type = typeof(ushort);
            public static readonly Type Int32Type = typeof(int);
            public static readonly Type UInt32Type = typeof(uint);
            public static readonly Type Int64Type = typeof(long);
            public static readonly Type UInt64Type = typeof(ulong);
            public static readonly Type DoubleType = typeof(double);
        }
    }
}

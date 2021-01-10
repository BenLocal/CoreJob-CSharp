using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace CoreJob.Framework
{
    public static class JobUtilExtensions
    {
        private static readonly TaskFactory _myTaskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

        public static string GetEnvironmentOrConfigStr(this IConfiguration configuration, string key)
        {
            // key1:ke2 => key1_key2
            var evnKey = key;
            if (key.Contains(":"))
            {
                evnKey = string.Join("_", key.Split(':'));
            }

            var value = Environment.GetEnvironmentVariable($"ASPNETCORE_{evnKey.ToUpper()}", EnvironmentVariableTarget.Process);

            if (string.IsNullOrEmpty(value))
            {
                value = configuration?.GetSection(key)?.Value;
            }

            return value;
        }

        public static void RunSync(Func<Task> func)
        {
            CultureInfo cultureUi = CultureInfo.CurrentUICulture;
            CultureInfo culture = CultureInfo.CurrentCulture;
            _myTaskFactory.StartNew(delegate
            {
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = cultureUi;
                return func();
            }).Unwrap().GetAwaiter().GetResult();
        }

        public static long GetTimeStamp(this DateTime dateTime)
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds);
        }

        public static DateTime GetDateTime(this long timestamp)
        {
            var start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return start.AddMilliseconds(timestamp).ToLocalTime();
        }

        public static string EnsureTrailingSlash(this string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            return Regex.Replace(input, "/+$", string.Empty) + "/";
        }

        public static string EnsureStartSlash(this string input)
        {
            if (input == null) return string.Empty;
            if (!input.StartsWith("/"))
            {
                return "/" + input;
            }
            return input;
        }

        public static List<(Type, T)> CollectByAttributeTypes<T>() where T : Attribute
        {
            List<(Type, T)> batchBaseTypes = new List<(Type, T)>();

            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (asm.FullName.StartsWith("System") || asm.FullName.StartsWith("Microsoft.Extensions")) continue;
                Type[] types;
                try
                {
                    types = asm.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    types = ex.Types;
                }

                foreach (var item in types)
                {
                    var attr = item.GetCustomAttribute<T>();
                    if (attr != null)
                    {
                        batchBaseTypes.Add((item, attr));
                    }
                }
            }

            return batchBaseTypes;
        }

        public static List<Type> CollectByInterface<T>(IEnumerable<Assembly> assemblies)
        {
            if (assemblies == null)
            {
                assemblies = AppDomain.CurrentDomain.GetAssemblies();
            }

            List<Type> batchBaseTypes = new List<Type>();

            foreach (var asm in assemblies)
            {
                if (asm.FullName.StartsWith("System") || asm.FullName.StartsWith("Microsoft.Extensions")) continue;

                Type[] types;
                try
                {
                    types = asm.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    types = ex.Types;
                }

                foreach (var item in types)
                {
                    if (typeof(T).IsAssignableFrom(item) && item != typeof(T) && !item.IsAbstract && !item.IsInterface)
                    {
                        batchBaseTypes.Add(item);
                    }
                }
            }

            return batchBaseTypes;
        }

        public static string MD5Encrypt(this string password)
        {
            if (password.NullOrEmpty())
            {
                return string.Empty;
            }

            using (MD5 md5 = MD5.Create())
            {
                byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(s);
            }
        }
    }
}

using System;

namespace CoreJob.Framework.Models.HttpAction
{
    public class RouteParameter
    {
        public string Name { get; }

        public string Pattern { get; }

        public Type TypeController { get; }

        public RouteParameter(string name, string pattern, Type type)
        {
            Name = name;
            Pattern = pattern;
            TypeController = type;
        }
    }
}

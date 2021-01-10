using System;

namespace CoreJob.Client.Framework.Models
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CoreJobHandlerAttribute : Attribute
    {
        public string Name { get; }

        public Type InterfaceType { get; set; }

        public CoreJobHandlerAttribute(string name, Type interfaceType)
        {
            Name = name;
            InterfaceType = interfaceType;
        }

        public CoreJobHandlerAttribute(string name)
        {
            Name = name;
        }
    }
}

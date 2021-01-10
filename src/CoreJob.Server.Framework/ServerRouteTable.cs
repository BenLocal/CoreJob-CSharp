using System;
using System.Collections.Generic;
using System.Text;
using CoreJob.Framework.Models.HttpAction;
using CoreJob.Server.Framework.Actions;

namespace CoreJob.Server.Framework
{
    public class ServerRouteTable : RouteTable
    {
        public ServerRouteTable()
        {
            AddRoute<RegistryController>("registry", "/registry");
            AddRoute<RemoveRegistryController>("removeRegistry", "/removeRegistry");
            AddRoute<CallBackContorller>("callback", "/callback");
        }
    }
}

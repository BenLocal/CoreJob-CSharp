using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CoreJob.Client.Framework.Actions;
using CoreJob.Framework.Abstractions;
using CoreJob.Framework.Models.HttpAction;

namespace CoreJob.Client.Framework
{
    public class ClientRouteTable : RouteTable
    {
        public ClientRouteTable()
        {
            AddRoute<RunController>("run", "/run");
            AddRoute<BeatController>("beat", "/beat");
            AddRoute<IdleBeatController>("idleBeat", "/idleBeat");
            AddRoute<KillController>("kill", "/kill");
            AddRoute<LogController>("run", "/log");
            AddRoute<OutCompleteController>("OutComplete", "/out/complete");
        }
    }
}

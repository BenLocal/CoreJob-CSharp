using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;

namespace CoreJob.Framework.Models.Response
{
    [MessagePackObject]
    public class JobLogResponse
    {
        [Key(0)]
        public int FromLineNum { get; set; }

        [Key(1)]
        public int ToLineNum { get; set; }

        [Key(2)]
        public string LogContent { get; set; }

        [Key(3)]
        public bool IsEnd { get; set; }
    }
}

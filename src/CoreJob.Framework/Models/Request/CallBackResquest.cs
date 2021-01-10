using MessagePack;

namespace CoreJob.Framework.Models.Request
{
    [MessagePackObject]
    public class CallBackResquest
    {
        [Key(0)]
        public int LogId { get; set; }

        [Key(1)]
        public long LogDateTime { get; set; }

        [Key(2)]
        public int ResultCode { get; set; }

        [Key(3)]
        public string ResultMsg { get; set; }
    }
}

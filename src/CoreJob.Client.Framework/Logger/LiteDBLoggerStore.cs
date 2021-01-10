using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CoreJob.Client.Framework.Abstractions;
using CoreJob.Client.Framework.Models;
using LiteDB;

namespace CoreJob.Client.Framework.Logger
{
    public class LiteDBLoggerStore : IJobLoggerStore
    {
        private const string DefaultPassWord = "DefaultPassWord12345678";

        private const string DefaultFilename = "joblog";

        private const string DefaultDir = "joblog";

        private const string DefaultJobLogCollectionName = "job";

        private readonly string _filename;
        public LiteDBLoggerStore()
        {
            var path = Path.Combine(Path.GetTempPath(), DefaultDir);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            
            _filename = Path.Combine(path, $"{DefaultFilename}-{DateTime.Now.ToString("yyyy-MM-dd")}.db");
        }


        public Task<(List<JobLoggerInfo>, int totalCount)> FilterJobList(int index, int limit, int jobLogId)
        {
            using (var db = new LiteDatabase($@"Filename={_filename};Password={DefaultPassWord}"))
            {
                var col = db.GetCollection<JobLoggerInfo>(DefaultJobLogCollectionName);
                var totalCount = col.Count(x => x.JobLogId == jobLogId);

                var list = col.Find(x => x.JobLogId == jobLogId, index, limit).ToList();

                return Task.FromResult((list, totalCount));
            }
        }

        public Task Save(JobLoggerInfo entry)
        {
            using (var db = new LiteDatabase($@"Filename={_filename};Password={DefaultPassWord}"))
            {
                var col = db.GetCollection<JobLoggerInfo>(DefaultJobLogCollectionName);
                col.EnsureIndex(x => x.JobLogId, true);
                col.Insert(entry);

                return Task.CompletedTask;
            }
        }
    }
}

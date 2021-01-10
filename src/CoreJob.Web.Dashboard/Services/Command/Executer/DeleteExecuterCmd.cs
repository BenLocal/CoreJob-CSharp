using System.Threading;
using System.Threading.Tasks;
using CoreJob.Server.Framework.Store;
using CoreJob.Web.Dashboard.Models;
using MediatR;

namespace CoreJob.Web.Dashboard.Services.Command.Executer
{
    public class DeleteExecuterCmd : IRequest<JsonEntityBase>
    {
        public int ExecuterId { get; set; }

        public class DeleteExecuterCmdRequest : IRequestHandler<DeleteExecuterCmd, JsonEntityBase>
        {
            private readonly JobDbContext _dbContext;

            public DeleteExecuterCmdRequest(JobDbContext dbContext)
            {
                _dbContext = dbContext;
            }


            public async Task<JsonEntityBase> Handle(DeleteExecuterCmd request, CancellationToken cancellationToken)
            {
                _dbContext.JobExecuter.Remove(new JobExecuter() { Id = request.ExecuterId });
                await _dbContext.SaveChangesAsync();

                return new
                {
                }.Success();
            }
        }
    }
}

using Quartz;
using System.Threading.Tasks;

namespace UberStrok.WebServices.AspNetCore.Job
{
    public class SessionJob : IJob
    {
        private readonly ISessionService sessionService;
        public SessionJob(ISessionService sessionService)
        {
            this.sessionService = sessionService;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            await sessionService.DeleteExpiredMemberSessionAsync();
        }
    }
}

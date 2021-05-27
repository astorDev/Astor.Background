using System.Threading.Tasks;
using GreenPipes;
using Microsoft.Extensions.Logging;

namespace Astor.Background.Core.Filters
{
    public class ExceptionsLogger : IFilter<EventContext>
    {
        public ILogger<ExceptionsLogger> Logger { get; }

        public ExceptionsLogger(ILogger<ExceptionsLogger> logger)
        {
            this.Logger = logger;
        }
        
        public async Task Send(EventContext context, IPipe<EventContext> next)
        {
            await next.Send(context);
            
            if (context.ActionResult.Exception != null)
            {
                this.Logger.LogError(context.ActionResult.Exception, $"exception occured while executing {context.Action.Id}");
            }
        }

        public void Probe(ProbeContext context)
        {
        }
    }
}
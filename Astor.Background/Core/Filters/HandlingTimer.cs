using System;
using System.Threading.Tasks;
using GreenPipes;

namespace Astor.Background.Core.Filters
{
    public class HandlingTimer : IFilter<EventContext>
    {
        public async Task Send(EventContext context, IPipe<EventContext> next)
        {
            context.HandlingParams.StartTime = DateTime.Now;

            await next.Send(context);
            
            context.HandlingParams.EndTime = DateTime.Now;
        }

        public void Probe(ProbeContext context)
        {
        }
    }
}
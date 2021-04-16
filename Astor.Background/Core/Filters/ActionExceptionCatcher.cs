using System;
using System.Threading.Tasks;
using GreenPipes;

namespace Astor.Background.Core.Filters
{
    public class ActionExceptionCatcher : IFilter<EventContext>
    {
        public async Task Send(EventContext context, IPipe<EventContext> next)
        {
            try
            {
                await next.Send(context);
            }
            catch (Exception e)
            {
                context.ActionResult.Exception = e;
            }
        }

        public void Probe(ProbeContext context)
        {
        }
    }
}
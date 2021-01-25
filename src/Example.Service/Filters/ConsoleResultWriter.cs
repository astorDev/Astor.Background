using System;
using System.Threading.Tasks;
using Astor.Background;
using GreenPipes;
using Newtonsoft.Json;

namespace Example.Service.Filters
{
    public class ConsoleResultWriter : IFilter<EventContext>
    {
        public Task Send(EventContext context, IPipe<EventContext> next)
        {
            Console.WriteLine($"result: {JsonConvert.SerializeObject(context.ActionResult.Output)}");
            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
        }
    }
}
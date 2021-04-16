using System;
using System.Threading.Tasks;
using Astor.Background.Core;
using Astor.GreenPipes;
using GreenPipes;

namespace Astor.Background.Tests
{
    public class ConsoleInputWriter : IFilter<EventContext>
    {
        public Task Send(EventContext context, IPipe<EventContext> next)
        {
            Console.WriteLine(context.Input.BodyString);
            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
        }
    }

    public static  class ConsoleInputWriterExtensions
    {
        public static PipeBuilder<EventContext> UseConsoleInputWriter(this PipeBuilder<EventContext> builder) =>
            builder.Use<ConsoleInputWriter>();
    }
}
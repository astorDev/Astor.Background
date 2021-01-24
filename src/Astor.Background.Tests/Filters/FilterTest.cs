using System;
using System.Threading.Tasks;
using Astor.GreenPipes;
using GreenPipes;
using Microsoft.Extensions.DependencyInjection;

namespace Astor.Background.Tests.Filters
{
    public class FilterTest : Test
    {
        public Task RunPipeAsync(Action<PipeBuilder<EventContext>> pipeConfiguration, EventContext eventContext)
        {
            var pipeBuilder = new PipeBuilder<EventContext>(this.ServiceCollection);
            pipeConfiguration(pipeBuilder);
            pipeBuilder.RegisterPipe();

            var pipe = this.ServiceProvider.GetRequiredService<IPipe<EventContext>>();
            return pipe.Send(eventContext);
        }
    }
}
using Astor.GreenPipes;
using Microsoft.Extensions.DependencyInjection;

namespace Astor.Background
{
    public class PipeBuilder : PipeBuilder<EventContext>
    {
        public PipeBuilder(IServiceCollection serviceCollection) : base(serviceCollection)
        {
        }
    }
}
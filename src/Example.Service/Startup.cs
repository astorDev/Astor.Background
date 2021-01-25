using Astor.Background;
using Astor.Background.Filters;
using Astor.Background.RabbitMq.Filters;
using Astor.GreenPipes;
using Astor.RabbitMq;
using Example.Service.Domain;
using Example.Service.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Example.Service
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddBackground(this.GetType().Assembly);
            services.AddRabbit(this.Configuration.GetSection("Rabbit"));
            
            services.Configure<GreetingPhrases>(this.Configuration.GetSection("Phrases"));
        }

        public void ConfigurePipe(PipeBuilder<EventContext> builder)
        {
            builder
                .Use<JsonBodyDeserializer>()
                .Use<ActionExecutor>()
                .Use<ConsoleResultWriter>()
                .Use<Acknowledger>();
        }
    }
}
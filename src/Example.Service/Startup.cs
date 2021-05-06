using Astor.Background.Core;
using Astor.Background.Core.Filters;
using Astor.Background.RabbitMq;
using Astor.Background.RabbitMq.Filters;
using Astor.Background.Tests;
using Astor.GreenPipes;
using Example.Service.Domain;
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
            services.AddRabbitMqBackgroundService(this.Configuration.GetConnectionString("RabbitMq"));
            
            services.Configure<GreetingPhrases>(this.Configuration.GetSection("Phrases"));
            services.AddSingleton<TextStore>();
        }

        public void ConfigurePipe(PipeBuilder<EventContext> builder)
        {
            builder
                    .Use<Acknowledger>()
                    .Use<LogPublisher>()
                    .Use<HandlingTimer>()
                    .Use<ActionExceptionCatcher>()
                    .Use<JsonBodyDeserializer>()
                    .Use<ActionExceptionCatcher>()
                    .Use<ActionExecutor>()
                ;
        }
    }
}
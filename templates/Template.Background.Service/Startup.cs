using Astor.Background.Core;
using Astor.Background.Core.Filters;
using Astor.Background.RabbitMq.Filters;
using Astor.Background.TelegramNotifications;
using Astor.RabbitMq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Template.Background.Service
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
            
            
        }

        public void ConfigurePipe(Astor.GreenPipes.PipeBuilder<EventContext> builder)
        {
            builder
                .Use<Acknowledger>()
                .Use<LogPublisher>()
                .Use<HandlingTimer>()
                .Use<ExceptionCatcherWithTelegramNotifications>()
                .Use<JsonBodyDeserializer>()
                .Use<ActionExceptionCatcher>()
                .Use<ActionExecutor>()
                ;
        }
    }
}
using System;
using Astor.Background.Core;
using Astor.Background.Core.Filters;
using Astor.Background.RabbitMq;
using Astor.Background.RabbitMq.Filters;
using Astor.GreenPipes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace Astor.Background.ElasticLogs.Service
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
            services.AddSingleton<IElasticClient>(sp =>
            {
                var uri = new Uri(this.Configuration["Elastic"]);
                var settings = new ConnectionSettings(uri);
                return new ElasticClient(settings);
            });
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
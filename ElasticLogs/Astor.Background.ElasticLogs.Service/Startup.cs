using System;
using Astor.Background.Core;
using Astor.Background.Core.Filters;
using Astor.Background.RabbitMq;
using Astor.Background.RabbitMq.Filters;
using Astor.GreenPipes;
using Elasticsearch.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Nest.JsonNetSerializer;

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
            services.AddRabbitMqBackgroundService(this.Configuration.GetConnectionString("RabbitMq"), "elastic-logs");
            services.AddSingleton<IElasticClient>(sp =>
            {
                var uri = new Uri(this.Configuration["Elastic"]);
                var pool = new SingleNodeConnectionPool(uri);
                var settings = new ConnectionSettings(pool, JsonNetSerializer.Default );
                return new ElasticClient(settings);
            });

            services.AddHttpClient<KibanaClient>(cl =>
            {
                cl.BaseAddress = new Uri(this.Configuration["Kibana"]);
            });
        }
        
        public void ConfigurePipe(PipeBuilder<EventContext> builder)
        {
            builder
                .Use<Acknowledger>()
                .Use<JsonBodyDeserializer>()
                .Use<ActionExecutor>()
                ;
        }
    }
}
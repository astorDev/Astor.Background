using System;
using System.Collections.Generic;
using Astor.Background.ElasticLogs.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Astor.Background.ElasticLogs.Tests
{
    public class Test
    {
        private IServiceProvider serviceProvider;
        public IServiceProvider ServiceProvider => this.serviceProvider ??= this.CreateServiceProvider();
        
        public IConfiguration CreateConfiguration()
        {
            var builder = new ConfigurationBuilder();
            OverrideConfiguration(builder);
            return builder.Build();
        }
        public static void OverrideConfiguration(IConfigurationBuilder builder)
        {
            builder.AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string>("ConnectionStrings:RabbitMq", "amqp://localhost:5672"),
                new KeyValuePair<string, string>("Elastic", "http://localhost:9200")
            });
        }
        

        public IServiceProvider CreateServiceProvider()
        {
            var services = new ServiceCollection();
            var configuration = this.CreateConfiguration();
            var startup = new Startup(configuration);
            startup.ConfigureServices(services);
            this.AdditionalServiceConfiguration(services);

            return services.BuildServiceProvider();
        }
        
        public void AdditionalServiceConfiguration(IServiceCollection services)
        {
            
        }
    }
}
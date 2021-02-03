using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Astor.RabbitMq
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRabbit(this IServiceCollection serviceCollection, string connectionString)
        {
            serviceCollection.AddSingleton(Options.Create(new ConnectionFactory
            {
                Uri = new Uri(connectionString)
            }));
            AddRabbitConnectionAndChannel(serviceCollection);
        }

        public static void AddRabbit(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Configure<ConnectionFactory>(configuration);
            AddRabbitConnectionAndChannel(serviceCollection);
        }
        
        private static void AddRabbitConnectionAndChannel(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(sp =>
            {
                var cf = sp.GetRequiredService<IOptions<ConnectionFactory>>().Value;
                return cf.CreateConnection();
            });
            serviceCollection.AddSingleton(sp => sp.GetRequiredService<IConnection>().CreateModel());
        }
    }
}
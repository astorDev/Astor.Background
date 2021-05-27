using System.Reflection;
using Astor.Background.Core;
using Astor.RabbitMq;
using Astor.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Astor.Background.RabbitMq
{
    public static class ServiceRegistrationExtensions
    {
        public static void AddRabbitMqBackgroundService(this IServiceCollection serviceCollection, string rabbitMqConnectionString, Assembly assembly = null, string internalExchangePrefix = null)
        {
            assembly ??= StackTraceAnalyzer.GetCallerType().Assembly;
            
            serviceCollection.AddBackground(assembly);
            serviceCollection.AddRabbit(rabbitMqConnectionString);
            serviceCollection.AddSingleton(sp =>
            {
                var coreService = sp.GetRequiredService<Core.Service>();
                return Service.Create(coreService, internalExchangePrefix);
            });
        }
    }
}
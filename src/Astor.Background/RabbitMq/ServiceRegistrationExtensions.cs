using Astor.Background.Core;
using Astor.RabbitMq;
using Astor.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Astor.Background.RabbitMq
{
    public static class ServiceRegistrationExtensions
    {
        public static void AddRabbitMqBackgroundService(this IServiceCollection serviceCollection, string rabbitMqConnectionString, string internalExchangePrefix = null)
        {
            var callerType = StackTraceAnalyzer.GetCallerType();
            
            serviceCollection.AddBackground(callerType.Assembly);
            serviceCollection.AddRabbit(rabbitMqConnectionString);
            serviceCollection.AddSingleton(sp =>
            {
                var coreService = sp.GetRequiredService<Core.Service>();
                return Service.Create(coreService, internalExchangePrefix);
            });
        }
    }
}
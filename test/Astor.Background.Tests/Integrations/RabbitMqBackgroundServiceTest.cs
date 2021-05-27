using Astor.Background.Core;
using Astor.Background.Core.Filters;
using Astor.Background.RabbitMq;
using Astor.Background.RabbitMq.Filters;
using Astor.GreenPipes;
using Example.Service.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace Astor.Background.Tests.Integrations
{
    public class RabbitMqBackgroundServiceTest : RabbitMqTest
    {
        public override IServiceCollection CreateBaseServiceCollection()
        {
            var serviceCollection = base.CreateBaseServiceCollection();
            serviceCollection.AddRabbitMqBackgroundService("amqp://localhost:5672", typeof(GreetingsController).Assembly, "auto-tested-greetings-service");
            
            serviceCollection.AddPipe<EventContext>(pipe => 
                pipe.Use<Acknowledger>()
                    .Use<JsonBodyDeserializer>()
                    .Use<ActionExecutor>());

            serviceCollection.AddSingleton<BackgroundService>();
            
            return serviceCollection;
        }
    }
}
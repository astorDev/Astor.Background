using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Astor.RabbitMq;
using Astor.Tests;
using Example.Service.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Action = Astor.Background.Core.Action;
using Service = Astor.Background.Core.Service;

namespace Astor.Background.Tests
{
    public class RabbitMqTest : GreetingsBasedTest
    {
        public readonly string Exchange = Guid.NewGuid().ToString();

        public readonly string Queue = Guid.NewGuid().ToString();

        public static Action GreetingAction => Service.Parse(typeof(GreetingsController)).Actions.First().Value;
        
        [TestCleanup]
        public void Clean()
        {
            var channel = this.ServiceProvider.GetRequiredService<IModel>();

            channel.QueueDelete(this.Queue);
            channel.ExchangeDelete(this.Exchange);
        }

        public ILogger<T> GetLogger<T>()
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            return loggerFactory.CreateLogger<T>();
        }
        
        public void SetupRabbit(Action<EventingBasicConsumer> consumption)
        {
            var channel = this.ServiceProvider.GetRequiredService<IModel>();
            
            channel.ExchangeDeclare(this.Exchange, "fanout");
            channel.QueueDeclare(this.Queue, false, false, false);
            channel.QueueBind(this.Queue, this.Exchange, "");

            var consumer = new EventingBasicConsumer(channel);
            consumption(consumer);
            
            channel.BasicConsume(this.Queue, false, consumer);
        }

        public override IServiceCollection CreateBaseServiceCollection()
        {
            var serviceCollection = base.CreateBaseServiceCollection();
            
            var configuration = this.buildConfiguration();
            serviceCollection.AddRabbit(configuration.GetSection("Rabbit"));

            return serviceCollection;
        }

        private IConfigurationRoot buildConfiguration()
        {
            return new ConfigurationBuilder().AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string>("Rabbit:Host", "localhost"),
                new KeyValuePair<string, string>("Rabbit:Port", "5672"),
                new KeyValuePair<string, string>("Rabbit:Password", "guest"),
                new KeyValuePair<string, string>("Rabbit:Login", "guest") 
            }).Build();
        }

        public async Task WaitForTextStore()
        {
            var textStore = this.ServiceProvider.GetRequiredService<TextStore>();
            var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token;

            while (textStore.TextOne == null)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            await Task.Delay(100);
        }
    }
}
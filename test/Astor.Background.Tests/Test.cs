using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Astor.Background.RabbitMq;
using Astor.RabbitMq;
using Example.Service.Controllers;
using Example.Service.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Action = Astor.Background.Core.Action;
using Service = Astor.Background.Core.Service;

namespace Astor.Background.Tests
{
    public class Test : Astor.Tests.Test
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

        public void PublishJson(object message)
        {
            var channel = this.ServiceProvider.GetRequiredService<IModel>();
            
            channel.PublishJson(this.Exchange, message);
        }

        public override IServiceCollection CreateBaseServiceCollection()
        {
            var configuration = this.buildConfiguration();
            
            var serviceCollection = base.CreateBaseServiceCollection();
            serviceCollection.AddRabbit(configuration.GetSection("Rabbit"));
            serviceCollection.AddSingleton(Options.Create(
                new GreetingPhrases
                {
                    Beginning = "AutoTestsHi"
                }));
            
            return serviceCollection;
        }

        private IConfigurationRoot buildConfiguration()
        {
            return new ConfigurationBuilder().AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string>("Phrases:Host", "localhost"),
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
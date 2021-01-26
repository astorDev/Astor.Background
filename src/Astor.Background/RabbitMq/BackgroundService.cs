using System;
using System.Threading;
using System.Threading.Tasks;
using Astor.Background.Core;
using GreenPipes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Astor.Background.RabbitMq
{
    public class BackgroundService : IHostedService
    {
        public IModel Channel { get; }
        public IServiceProvider ServiceProvider { get; }
        public Service Service { get; }

        public BackgroundService(IModel channel, 
            IServiceProvider serviceProvider, 
            Service service)
        {
            this.Channel = channel;
            this.ServiceProvider = serviceProvider;
            this.Service = service;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var subscription in this.Service.Subscriptions)
            {
                this.Channel.QueueDeclare(subscription.Action.Id, true, false, false);
                this.Channel.QueueBind(subscription.Action.Id, subscription.Attribute.Event.ToString(), routingKey: "");

                var consumer = new EventingBasicConsumer(this.Channel);

                consumer.Received += async (sender, eventArgs) =>
                {
                    var context = new EventContext(subscription.Action, InputHelper.Parse(eventArgs));

                    using var scope = this.ServiceProvider.CreateScope();
                    var pipe = scope.ServiceProvider.GetRequiredService<IPipe<EventContext>>();
                    await pipe.Send(context);
                };

                this.Channel.BasicConsume(subscription.Action.Id, false, consumer);
            }
            
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
        }
    }
}
using System;
using Astor.Background.Core;
using Astor.Background.Core.Abstractions;
using GreenPipes;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Action = Astor.Background.Core.Action;
using SubscribedOnAttribute = Astor.Background.RabbitMq.Abstractions.SubscribedOnAttribute;

namespace Astor.Background.RabbitMq
{
    public class Subscription
    {
        public bool DeclareExchange { get; init; }
        
        public bool DeclareAndBindQueue { get; init; }
        
        public string ExchangeName { get; init; }
        
        public Action Action { get; init; }
        
        public string InternalExchangeName { get; init; }
        
        private Subscription()
        {
        }

        public static Subscription Create(Core.Subscription coreSubscription, string internalExchangesPrefix = null)
        {
            if (coreSubscription.Attribute is SubscribedOnInternalAttribute subscribedOnInternalAttribute)
            {
                if (internalExchangesPrefix == null)
                {
                    throw new InvalidOperationException(@$"when using {typeof(SubscribedOnInternalAttribute)}
{nameof(internalExchangesPrefix)} is required");
                }

                return new Subscription
                {
                    DeclareExchange = true,
                    DeclareAndBindQueue = true,
                    InternalExchangeName = subscribedOnInternalAttribute.Event.ToString(),
                    ExchangeName = Service.InternalExchangeName(internalExchangesPrefix, subscribedOnInternalAttribute.Event),
                    Action = coreSubscription.Action
                };
            }

            if (coreSubscription.Attribute is SubscribedOnAttribute rabbitSubscribedOnAttribute)
            {
                return new Subscription
                {
                    DeclareExchange = rabbitSubscribedOnAttribute.DeclareExchange,
                    DeclareAndBindQueue = rabbitSubscribedOnAttribute.DeclareAndBindQueue,
                    ExchangeName = rabbitSubscribedOnAttribute.Event.ToString(),
                    Action = coreSubscription.Action
                };
            }

            throw new InvalidOperationException(@$"when using {typeof(BackgroundService).FullName}
all subscriptions must be of one of the types:
{typeof(SubscribedOnInternalAttribute).FullName}
{typeof(SubscribedOnAttribute).FullName}

but one of subscription for action {coreSubscription.Action.Id} is of type {coreSubscription.Attribute.GetType().FullName}
");
        }


        public void Register(IModel channel, IServiceProvider serviceProvider)
        {
            if (this.DeclareExchange)
            {
                channel.ExchangeDeclare(this.ExchangeName,  "fanout", true, false);
            }

            if (this.DeclareAndBindQueue)
            {
                channel.QueueDeclare(this.Action.Id, true, false, false);
                channel.QueueBind(this.Action.Id, this.ExchangeName, routingKey: "");
            }
                    
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += async (sender, eventArgs) =>
            {
                var context = new EventContext(this.Action, InputHelper.Parse(eventArgs));

                using var scope = serviceProvider.CreateScope();
                var pipe = scope.ServiceProvider.GetRequiredService<IPipe<EventContext>>();
                await pipe.Send(context);
            };

            channel.BasicConsume(this.Action.Id, false, consumer);
        }
    }
}
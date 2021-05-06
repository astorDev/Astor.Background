using System;
using System.Runtime.CompilerServices;
using Astor.Background.Core;
using Astor.RabbitMq;
using GreenPipes;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Telegram.Bot.Requests;
using Action = Astor.Background.Core.Action;

namespace Astor.Background.RabbitMq
{
    public static class HandlingLogic
    {
        public static void ConsumeQueue(this Action action, IModel rabbitChanel, IServiceProvider serviceProvider)
        {
            rabbitChanel.ConsumeQueue(action.Id, eventHandler(action, serviceProvider), new ConsumptionSettings
            {
                AutoAck = false
            });
        }

        public static void DeclareAndConsumeQueue(this Action action, IModel rabbitChannel,
            IServiceProvider serviceProvider)
        {
            rabbitChannel.DeclareAndConsumeQueue(action.Id, eventHandler(action, serviceProvider), new ConsumptionSettings
            {
                AutoAck = false
            });
        }

        private static EventHandler<BasicDeliverEventArgs> eventHandler(Action action, IServiceProvider serviceProvider)
        {
            return (async (sender, args) =>
            {
                var context = new EventContext(action, InputHelper.Parse(args));

                using var scope = serviceProvider.CreateScope();
                var pipe = scope.ServiceProvider.GetRequiredService<IPipe<EventContext>>();
                await pipe.Send(context);
            });
        }
    }
}
using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Astor.RabbitMq
{
    public static class ConsumptionExtensions
    {
        public static void ConsumeQueue(this IModel channel, 
            string queueName, 
            EventHandler<BasicDeliverEventArgs> consumption,
            ConsumptionSettings consumptionSettings = null)
        {
            consumptionSettings ??= new ConsumptionSettings();
            
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += consumption;
            channel.BasicConsume(queueName, consumptionSettings.AutoAck, consumer);
        }

        public static void DeclareAndConsumeQueue(this IModel channel,
            string queueName,
            EventHandler<BasicDeliverEventArgs> consumption,
            ConsumptionSettings consumptionSettings = null,
            QueueSettings queueSettings = null)
        {
            queueSettings ??= new QueueSettings();

            channel.QueueDeclare(queueName, queueSettings.Durable, queueSettings.Exclusive, queueSettings.AutoDelete,
                 queueSettings.Arguments);
            
            ConsumeQueue(channel, queueName, consumption, consumptionSettings);
        }
    }
}
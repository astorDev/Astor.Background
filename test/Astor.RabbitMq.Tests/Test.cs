using System;
using RabbitMQ.Client;

namespace Astor.RabbitMq.Tests
{
    public class Test
    {
        public static IModel CreateRabbitChannel()
        {
            var connectionFactory = new ConnectionFactory
            {
                Uri = new Uri("amqp://localhost:5672")
            };

            var connection = connectionFactory.CreateConnection();
            return connection.CreateModel();
        }
    }
}
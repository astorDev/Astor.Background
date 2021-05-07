using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Astor.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Client;

namespace Astor.RabbitMq.Tests
{
    [TestClass]
    public class SubscriptionExtensions_Should
    {
        [TestMethod]
        public async Task SubscribeToQueue()
        {
            const string queueName = "SubscriptionExtensions_Should_SubscribeToQueue";
            var processed = false;
            
            using var channel = Test.CreateRabbitChannel();
            channel.QueueDeclare(queueName, false, false, true, null);
            channel.ConsumeQueue(queueName, async (o, args) =>
            {
                processed = true;
            });
            
            channel.BasicPublish("", queueName, null, null);
            await Waiting.For(() => processed, TimeSpan.FromSeconds(2));
        }

        [TestMethod]
        public async Task DeclareAndSubscribeToQueue()
        {
            const string queueName = "SubscriptionExtensions_Should_DeclareAndSubscribeToQueue";
            var processed = false;
            
            using var channel = Test.CreateRabbitChannel();
            channel.DeclareAndConsumeQueue(queueName, async (o, args) =>
            {
                processed = true;
            });

            channel.BasicPublish("", queueName, null, null);
            await Waiting.For(() => processed, TimeSpan.FromSeconds(2));
        }
    }
}
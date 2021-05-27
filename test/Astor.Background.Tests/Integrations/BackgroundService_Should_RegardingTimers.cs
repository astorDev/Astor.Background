using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Astor.Background.Core;
using Astor.Background.Core.Filters;
using Astor.Background.Management.Protocol;
using Astor.Background.RabbitMq;
using Astor.Background.RabbitMq.Filters;
using Astor.Background.TelegramNotifications;
using Astor.GreenPipes;
using Astor.RabbitMq;
using Astor.Tests;
using Example.Service.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Service = Astor.Background.Core.Service;

namespace Astor.Background.Tests.Integrations
{
    [TestClass]
    public class BackgroundService_Should_RegardingTimers : RabbitMqBackgroundServiceTest
    {
        [TestMethod]
        public async Task PublishReceiverSchedule()
        {
            //Arrange
            string scheduleJson = null;
            this.ConsumeScheduleExchange((sender, args) => { scheduleJson = Encoding.UTF8.GetString(args.Body.ToArray()); });
            var bgService = this.ServiceProvider.GetRequiredService<BackgroundService>();
            
            //Act
            await bgService.StartAsync(CancellationToken.None);
            var token = new CancellationTokenSource(TimeSpan.FromSeconds(2)).Token;
            while (scheduleJson == null)
            {
                await Task.Delay(100, token);
            }

            //Assert
            var schedule = JsonConvert.DeserializeObject<ReceiverSchedule>(scheduleJson);
            Assert.AreEqual("Greetings", schedule.Receiver);
            Assert.AreEqual(2, schedule.ActionSchedules.Length);

            Assert.AreEqual(TimeSpan.FromSeconds(30), Single(schedule, "Greetings_RemindAboutYourself").Interval);
            
            var wakeupSchedule = Single(schedule, "Greetings_WakeUp");
            Assert.AreEqual(2, wakeupSchedule.EveryDayAt.Length);
            AssertAnyTime(wakeupSchedule, TimeSpan.FromHours(7));
            AssertAnyTime(wakeupSchedule, TimeSpan.FromHours(8));
            Assert.IsTrue(wakeupSchedule.EveryDayAt.Any(s => s == TimeSpan.FromHours(7)), "wakeupSchedule.EveryDayAt.Any(s => s == TimeSpan.FromHours(7))");
        }
        
        [TestMethod]
        [Ignore("Conflicts with upper method - runs as single")]
        public async Task SubscribeToQueue()
        {
            var bgService = this.ServiceProvider.GetRequiredService<BackgroundService>();
            var channel = this.ServiceProvider.GetRequiredService<IModel>();
            var textStore = this.ServiceProvider.GetRequiredService<TextStore>();
            
            await bgService.StartAsync(CancellationToken.None);
            channel.BasicPublish("", "Greetings_RemindAboutYourself");
            await Waiting.For(() => textStore.TextOne != null, TimeSpan.FromSeconds(5));
            
            Assert.AreEqual("Hey there I'm ready to say hello", textStore.TextOne);
        }

        private void ConsumeScheduleExchange(EventHandler<BasicDeliverEventArgs> handler)
        {
            const string queueName = "BackgroundService_Should_RegardingTimers_PublishReceiverSchedule";
            
            var channel = this.ServiceProvider.GetRequiredService<IModel>();

            channel.ExchangeDeclare(ExchangeNames.Schedule, "fanout", true);
            channel.DeclareAndConsumeQueue(queueName, handler);
            channel.QueueBind(queueName, ExchangeNames.Schedule, "");
        }
        
        private static ActionSchedule Single(ReceiverSchedule schedule, string actionId)
        {
            var actionSchedule = schedule.ActionSchedules.SingleOrDefault(a => a.ActionId == actionId);
            if (actionSchedule == null)
            {
                throw new InvalidOperationException($"schedule for {actionId} not found");
            }

            return actionSchedule;
        }

        private static void AssertAnyTime(ActionSchedule schedule, TimeSpan time)
        {
            Assert.IsTrue(schedule.EveryDayAt.Any(t => t == time), $"no time {time}");
        }
    }
}
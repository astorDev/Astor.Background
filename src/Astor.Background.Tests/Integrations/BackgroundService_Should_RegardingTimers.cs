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
using Service = Astor.Background.Core.Service;

namespace Astor.Background.Tests.Integrations
{
    [TestClass]
    public class BackgroundService_Should_RegardingTimers : Test
    {
        [TestMethod]
        public async Task PublishReceiverSchedule()
        {
            const string queueName = "BackgroundService_Should_RegardingTimers_PublishReceiverSchedule";
            string scheduleJson = null;
            
            var channel = this.ServiceProvider.GetRequiredService<IModel>();
            var service = RabbitMq.Service.Create(Service.Parse(typeof(GreetingsController)));
            
            channel.ExchangeDeclare(ExchangeNames.Schedule, "fanout", true);
            channel.DeclareAndConsumeQueue(queueName,
                (sender, args) =>
                {
                    scheduleJson = Encoding.UTF8.GetString(args.Body.ToArray());
                });
            channel.QueueBind(queueName, ExchangeNames.Schedule, "");

            var bgService = new BackgroundService(
                channel, 
                this.ServiceProvider, 
                service, 
                this.GetLogger<BackgroundService>());

            await bgService.StartAsync(CancellationToken.None);

            var token = new CancellationTokenSource(TimeSpan.FromSeconds(2)).Token;
            while (scheduleJson == null)
            {
                await Task.Delay(100, token);
            }

            var schedule = JsonConvert.DeserializeObject<ReceiverSchedule>(scheduleJson);
            Assert.AreEqual("Greetings", schedule.Receiver);
            Assert.AreEqual(2, schedule.ActionSchedules.Length);
            var remindAboutYourselfActionSchedule = single(schedule, "Greetings_RemindAboutYourself");
            var wakeupSchedule = single(schedule, "Greetings_WakeUp");
            
            Assert.AreEqual(TimeSpan.FromSeconds(10), remindAboutYourselfActionSchedule.Interval);
            Assert.AreEqual(2, wakeupSchedule.EveryDayAt.Length);
            assertAnyTime(wakeupSchedule, TimeSpan.FromHours(7));
            assertAnyTime(wakeupSchedule, TimeSpan.FromHours(8));
            Assert.IsTrue(wakeupSchedule.EveryDayAt.Any(s => s == TimeSpan.FromHours(7)), "wakeupSchedule.EveryDayAt.Any(s => s == TimeSpan.FromHours(7))");
        }

        [TestMethod]
        [Ignore("Conflicts with upper method - runs as single")]
        public async Task SubscribeToQueue()
        {
            var channel = this.ServiceProvider.GetRequiredService<IModel>();
            var service = RabbitMq.Service.Create(Service.Parse(typeof(GreetingsController)));
            
            var bgService = new BackgroundService(
                channel, 
                this.ServiceProvider, 
                service, 
                this.GetLogger<BackgroundService>());

            await bgService.StartAsync(CancellationToken.None);
            
            channel.BasicPublish("", "Greetings_RemindAboutYourself");
            var textStore = this.ServiceProvider.GetRequiredService<TextStore>();

            await Waiting.For(() => textStore.TextOne != null, TimeSpan.FromSeconds(5));
            
            Assert.AreEqual("Hey there I'm ready to say hello", textStore.TextOne);
        }

        public override IServiceCollection CreateBaseServiceCollection()
        {
            var serviceCollection = base.CreateBaseServiceCollection();
            serviceCollection.AddScoped<GreetingsController>();
            serviceCollection.AddPipe<EventContext>(pipe => 
                pipe.Use<Acknowledger>()
                    .Use<JsonBodyDeserializer>()
                    .Use<ActionExecutor>());

            return serviceCollection;
        }

        private static ActionSchedule single(ReceiverSchedule schedule, string actionId)
        {
            var actionSchedule = schedule.ActionSchedules.SingleOrDefault(a => a.ActionId == actionId);
            if (actionSchedule == null)
            {
                throw new InvalidOperationException($"schedule for {actionId} not found");
            }

            return actionSchedule;
        }

        private static void assertAnyTime(ActionSchedule schedule, TimeSpan time)
        {
            Assert.IsTrue(schedule.EveryDayAt.Any(t => t == time), $"no time {time}");
        }
    }
}
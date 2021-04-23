using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Astor.Background.Management.Service.Controllers;
using Astor.Background.Management.Service.Timers;
using FluentScheduler;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Astor.Background.Management.Service.Tests
{
    [TestClass]
    public class Timers_Should : Test
    {
        [TestMethod]
        public async Task FireIntervalActions()
        {
            var consumed = false;
            var queueName = "TimerTestHandler_FireIntervalActions";
            
            var host = GetValidatedHost();
            var model = host.Services.GetRequiredService<IModel>();
            model.QueueDeclare(queueName);
            var consumer = new EventingBasicConsumer(model);
            consumer.Received += (sender, args) =>
            {
                Console.WriteLine("message consumed");
                consumed = true;
            };
            
            model.BasicConsume(consumer, queueName);
            
            var schedulesStore = host.Services.GetRequiredService<SchedulesStore>();
            await schedulesStore.AddOrUpdateAsync(new ActionSchedule
            {
                ActionId = "TimerTestHandler_FireIntervalActions",
                Interval = TimeSpan.FromSeconds(2)
            });

            var timersController = host.Services.GetRequiredService<TimersController>();
            await timersController.RefreshAsync();
            
            var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token;
            while (!consumed)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }
        }
        
        [TestMethod]
        public async Task FireActionsInDifferentWays()
        {
            var intervalEventConsumed = false;
            var timeEventConsumed = false;

            var intervalActionId = "TimerTestHandler_FireActionsInDifferentWays_Interval";
            var timeActionId = "TimerTestHandler_FireActionsInDifferentWays_Times";

            var host = GetValidatedHost();
            var model = host.Services.GetRequiredService<IModel>();
            
            DeclareAndHandleQueue(model, intervalActionId, () => { intervalEventConsumed = true; }); 
            DeclareAndHandleQueue(model, timeActionId, () => { timeEventConsumed = true; });
            
            var schedulesStore = host.Services.GetRequiredService<SchedulesStore>();
            await schedulesStore.AddOrUpdateAsync(new ActionSchedule
            {
                ActionId = intervalActionId,
                Interval = TimeSpan.FromSeconds(20)
            });

            await schedulesStore.AddOrUpdateAsync(new ActionSchedule
            {
                ActionId = timeActionId,
                EveryDayAt = new[] {DateTime.Now.TimeOfDay.Add(TimeSpan.FromMinutes(1))}
            });

            var controller = host.Services.GetRequiredService<TimersController>();
            await controller.RefreshAsync();
            
            var cancellationToken = new CancellationTokenSource(TimeSpan.FromMinutes(1)).Token;
            while (!intervalEventConsumed || !timeEventConsumed)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine($"{nameof(intervalEventConsumed)} = {intervalEventConsumed}");
                    Console.WriteLine($"{nameof(timeEventConsumed)} = {timeEventConsumed}");
                }
                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        [TestMethod]
        public async Task FireOnTimeWithRegardsToTimezoneShift()
        {
            var actionId = "TimerTestHandler_FireOnTimeWithRegardsToTimezoneShift";
            var consumed = false;
            
            var host = GetValidatedHost("--TimeZoneShift=5");
            var channel = host.Services.GetRequiredService<IModel>();
            
            DeclareAndHandleQueue(channel, actionId, () => { consumed = true; });

            var schedulesStore = host.Services.GetRequiredService<SchedulesStore>();
            await schedulesStore.AddOrUpdateAsync(new ActionSchedule
            {
                ActionId = actionId,
                EveryDayAt = new []
                {
                    DateTime.Now.Subtract(TimeSpan.FromHours(5)).TimeOfDay.Add(TimeSpan.FromMinutes(1))
                }
            });
            
            var controller = host.Services.GetRequiredService<TimersController>();
            await controller.RefreshAsync();
            
            var cancellationToken = new CancellationTokenSource(TimeSpan.FromMinutes(1)).Token;
            while (!consumed)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        [TestMethod]
        public async Task UpdateScheduleOnTheFly()
        {
            var counter = 0;
            var actionId = "TimerTestHandler_UpdateScheduleOnTheFly";

            var host = GetValidatedHost();
            var channel = host.Services.GetRequiredService<IModel>();
            
            DeclareAndHandleQueue(channel, actionId, () =>
            {
                Console.WriteLine($"incrementing counter at {DateTime.Now}");
                counter++;
            });

            var scheduleStore = host.Services.GetRequiredService<SchedulesStore>();
            await scheduleStore.AddOrUpdateAsync(new ActionSchedule
            {
                ActionId = actionId,
                Interval = TimeSpan.FromSeconds(1)
            });

            using (var scope = host.Services.CreateScope())
            {
                var controller = scope.ServiceProvider.GetRequiredService<TimersController>();
                await controller.RefreshAsync();
            }

            var cancellationTokenFirst = new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token;

            while (counter != 3)
            {
                cancellationTokenFirst.ThrowIfCancellationRequested();
                await Task.Delay(100);
            }

            await scheduleStore.AddOrUpdateAsync(new ActionSchedule
            {
                ActionId = actionId,
                Interval = TimeSpan.FromSeconds(3)
            });

            using (var scope = host.Services.CreateScope())
            {
                var controller = scope.ServiceProvider.GetRequiredService<TimersController>();
                await controller.RefreshAsync();
            }

            counter = 0;

            var cancellationTokenTwo = new CancellationTokenSource(TimeSpan.FromSeconds(9)).Token;
            while (!cancellationTokenTwo.IsCancellationRequested)
            {
                if (counter > 3)
                {
                    throw new AssertFailedException($"counter greater than 3 already ({counter})");
                }
                
                await Task.Delay(100);
            }
            
            Assert.AreEqual(3, counter);
        }

        private static void DeclareAndHandleQueue(IModel channel, string queueName, Action handler)
        {
            channel.QueueDeclare(queueName);
            var intervalConsumer = new EventingBasicConsumer(channel);
            intervalConsumer.Received += (sender, args) =>
            {
                handler();
            };

            channel.BasicConsume(intervalConsumer, queueName);
        }
    }
}
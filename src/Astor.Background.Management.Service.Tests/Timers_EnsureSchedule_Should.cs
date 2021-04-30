﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Astor.Background.Management.Protocol;
using Astor.Background.Management.Service.Controllers;
using Astor.RabbitMq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Client;

namespace Astor.Background.Management.Service.Tests
{
    [TestClass]
    public class Timers_EnsureSchedule_Should : Test
    {
        [TestMethod]
        public async Task StartIntervalTimers()
        {
            var q1Name = "q1";
            var q2Name = "q2";
            var q3Name = "q3";

            var q1Count = 0;
            var q2Count = 0;
            var q3Count = 0;
            
            var host = GetValidatedHost();

            var channel = host.Services.GetRequiredService<IModel>();
            channel.DeclareAndConsumeQueue(q1Name, (sender, args) => q1Count++);
            channel.DeclareAndConsumeQueue(q2Name, (sender, args) => q2Count++);
            channel.DeclareAndConsumeQueue(q3Name, (sender, args) => q3Count++);
            
            var controller = host.Services.GetRequiredService<TimersController>();
            await controller.EnsureScheduleAsync(new ReceiverSchedule
            {
                Receiver = "Timers_EnsureSchedule_Should_StartTimers",
                ActionSchedules = new []
                {
                    new ActionSchedule
                    {
                        ActionId = q1Name,
                        Interval = TimeSpan.FromSeconds(2)
                    },
                    new ActionSchedule
                    {
                        ActionId = q2Name,
                        Interval = TimeSpan.FromSeconds(3)
                    },
                    new ActionSchedule
                    {
                        ActionId = q3Name,
                        Interval = TimeSpan.FromSeconds(4)
                    }
                }
            });

            var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(11)).Token;
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(100);
            }

            Assert.AreEqual(5, q1Count);
            Assert.AreEqual(3,q2Count);
            Assert.AreEqual(2, q3Count);
        }

        [TestMethod]
        public async Task StartAndUpdateCombinedTimers()
        {
            var q1Name = "q1";
            var q2Name = "q2";
            var q3Name = "q3";

            var q1Count = 0;
            var q2Count = 0;
            var q3Count = 0;
            
            var host = GetValidatedHost();

            var channel = host.Services.GetRequiredService<IModel>();
            channel.DeclareAndConsumeQueue(q1Name, (sender, args) => q1Count++);
            channel.DeclareAndConsumeQueue(q2Name, (sender, args) => q2Count++);
            channel.DeclareAndConsumeQueue(q3Name, (sender, args) => q3Count++);
            
            var controller = host.Services.GetRequiredService<TimersController>();
            await controller.EnsureScheduleAsync(new ReceiverSchedule
            {
                Receiver = "Timers_EnsureSchedule_Should_StartAndUpdateCombinedTimers",
                ActionSchedules = new []
                {
                    new ActionSchedule
                    {
                        ActionId = q1Name,
                        Interval = TimeSpan.FromSeconds(2)
                    },
                    new ActionSchedule
                    {
                        ActionId = q2Name,
                        EveryDayAt = new []
                        {
                            DateTime.Now.TimeOfDay.Add(TimeSpan.FromMinutes(1)),
                            DateTime.Now.TimeOfDay.Add(TimeSpan.FromMinutes(2))
                        }
                    },
                    new ActionSchedule
                    {
                        ActionId = q3Name,
                        EveryDayAt = new []
                        {
                            DateTime.Now.TimeOfDay.Add(TimeSpan.FromMinutes(1)),
                            DateTime.Now.TimeOfDay.Add(TimeSpan.FromMinutes(2))
                        }
                    }
                }
            });
            
            
            
        }
    }
}
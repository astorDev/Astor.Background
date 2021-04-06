using System;
using System.Threading;
using System.Threading.Tasks;
using FluentScheduler;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Client;

namespace Astor.Background.Management.Service.Tests
{
    [TestClass]
    public class Timers_Should
    {
        [TestMethod]
        public async Task RefreshEventsFromDb()
        {
            //TODO: create db record
            
            var host = Test.StartHost();
            
            var rabbitChannel = host.Services.GetRequiredService<IModel>();
            var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token;
            Schedule schedule = null;

            while (schedule == null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                schedule = JobManager.GetSchedule("Test_Record");
            }
        }
    }
}
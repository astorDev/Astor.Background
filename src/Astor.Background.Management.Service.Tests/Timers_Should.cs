using System;
using System.Threading;
using System.Threading.Tasks;
using Astor.Background.Management.Service.Timers;
using FluentScheduler;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Astor.Background.Management.Service.Tests
{
    [TestClass]
    public class Timers_Should : Test
    {
        [TestMethod]
        public async Task RefreshEventsFromDb()
        {
            var host = Test.StartHost();
            var schedulesStore = host.Services.GetRequiredService<SchedulesStore>();
            await schedulesStore.AddAsync(new ActionSchedule
            {
                ActionId = "Test_Record",
                Interval = TimeSpan.FromSeconds(2)
            });

            //var sa = await schedulesStore.GetAllAsync();
            
            var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token;
            Schedule schedule = null;

            while (schedule == null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var sch = JobManager.AllSchedules;
                schedule = JobManager.GetSchedule("Test_Record_0");
            }
            
            Console.WriteLine(JsonConvert.SerializeObject(schedule));
        }
    }
}
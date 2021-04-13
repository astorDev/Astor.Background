using System;
using System.Threading;
using System.Threading.Tasks;
using FluentScheduler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

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
            
            var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token;
            Schedule schedule = null;

            while (schedule == null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                schedule = JobManager.GetSchedule("Test_Record");
            }
            
            Console.WriteLine(JsonConvert.SerializeObject(schedule));
        }
    }
}
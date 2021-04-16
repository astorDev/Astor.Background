using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Astor.Background.Core;
using Astor.Background.Core.Abstractions;
using Astor.Background.Management.Protocol;
using Astor.Background.Management.Service.Timers;
using Astor.RabbitMq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Astor.Background.Management.Service.Controllers
{
    public class TimersController
    {
        public SchedulesStore Store { get; }
        public ILogger<TimersController> Logger { get; }

        private readonly Timers.Timers timers;

        private readonly int TimeZoneShift;
        
        public TimersController(SchedulesStore store, IConfiguration configuration, IModel rabbitChannel, ILogger<TimersController> logger)
        {
            this.Store = store;
            this.Logger = logger;
            this.timers = new Timers.Timers(action =>
            {
                this.Logger.LogDebug($"{action} timer occured at {DateTime.Now}");
                rabbitChannel.BasicPublish("", action);
            });

            Int32.TryParse(configuration["TimeZoneShift"], out this.TimeZoneShift);
        }
        
        [SubscribedOnInternal(InternalEventNames.Started)]
        public async Task RefreshAsync()
        {
            var schedule = await this.Store.GetAllAsync();

            foreach (var row in schedule)
            {
                if (row.Times != null)
                {
                    this.timers.EnsureValid(row.ActionId, row.EveryDayAt.Select(t =>
                    {
                        return this.TimeZoneShift switch
                        {
                            0 => t,
                            < 0 => t.Subtract(TimeSpan.FromHours(Math.Abs(this.TimeZoneShift))),
                            _ => t.Add(TimeSpan.FromHours(this.TimeZoneShift))
                        };
                    }));
                }
                else
                {
                    this.timers.EnsureValid(row.ActionId, row.Interval.Value);
                }
            }
            
            this.timers.EnsureOnly(schedule.Select(s => s.ActionId.ToString()));
        }
    }
}
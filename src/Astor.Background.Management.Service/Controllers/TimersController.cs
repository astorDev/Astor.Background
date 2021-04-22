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
using Telegram.Bot.Types.InlineQueryResults.Abstractions;

namespace Astor.Background.Management.Service.Controllers
{
    public class TimersController
    {
        public SchedulesStore Store { get; }
        public IModel RabbitChannel { get; }
        public Timers.Timers Timers { get; }
        public ILogger<TimersController> Logger { get; }
        
        private readonly int timeZoneShift;
        
        public TimersController(SchedulesStore store, 
            IConfiguration configuration, 
            IModel rabbitChannel,
            Timers.Timers timers,
            ILogger<TimersController> logger)
        {
            this.Store = store;
            this.RabbitChannel = rabbitChannel;
            this.Timers = timers;
            this.Logger = logger;

            Int32.TryParse(configuration["TimeZoneShift"], out this.timeZoneShift);
        }

        [SubscribedOnInternal(InternalEventNames.Started)]
        public async Task RefreshAsync()
        {
            var schedule = await this.Store.GetAllAsync();
            this.Logger.LogDebug($"{schedule.Count()} of schedules received - updating timers");
            
            foreach (var row in schedule)
            {
                var timesAction = row.ToTimesActionOrNull(this.timeZoneShift);
                
                if (timesAction != null)
                {
                    this.Timers.Ensure(timesAction, this.TriggerAction);
                }
                else
                {
                    this.Timers.Ensure(row.ToIntervalAction(), this.TriggerAction);
                }
            }
            
            this.Timers.EnsureOnly(schedule.Select(s => s.ActionId.ToString()));
        }

        private void TriggerAction(string actionId)
        {
            this.Logger.LogDebug($"{actionId} timer occured at {DateTime.Now}");
            this.RabbitChannel.BasicPublish("", actionId);
        }
        
    }
}
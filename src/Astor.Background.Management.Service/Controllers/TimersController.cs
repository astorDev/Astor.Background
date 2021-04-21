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
        public ILogger<TimersController> Logger { get; }

        private readonly Timers.Timers timers;

        private readonly int TimeZoneShift;
        
        public TimersController(SchedulesStore store, IConfiguration configuration, IModel rabbitChannel, 
            ILogger<TimersController> logger,
            ILogger<TimeActionsCollection> timeActionsCollectionLogger,
            ILogger<IntervalActionsCollection> intervalActionsCollectionLogger,
            ILogger<Timers.Timers> timersLogger)
        {
            this.Store = store;
            this.RabbitChannel = rabbitChannel;
            this.Logger = logger;

            var timeActionsCollection = new TimeActionsCollection(timeActionsCollectionLogger, this.TriggerAction);
            var intervalActionsCollection = new IntervalActionsCollection(intervalActionsCollectionLogger, this.TriggerAction);
            
            this.timers = new Timers.Timers(intervalActionsCollection, timeActionsCollection, timersLogger);

            Int32.TryParse(configuration["TimeZoneShift"], out this.TimeZoneShift);
        }

        [SubscribedOnInternal(InternalEventNames.Started)]
        public async Task RefreshAsync()
        {
            var schedule = await this.Store.GetAllAsync();
            this.Logger.LogDebug($"{schedule.Count()} of schedules received - updating timers");
            
            foreach (var row in schedule)
            {
                var intervalAction = row.ToIntervalActionOrNull();
                if (intervalAction != null)
                {
                    this.timers.Ensure(intervalAction);
                }
                else
                {
                    this.timers.Ensure(row.ToTimesAction(this.TimeZoneShift));
                }
            }
            
            this.timers.EnsureOnly(schedule.Select(s => s.ActionId.ToString()));
        }

        private void TriggerAction(string actionId)
        {
            this.Logger.LogDebug($"{actionId} timer occured at {DateTime.Now}");
            this.RabbitChannel.BasicPublish("", actionId);
        }
        
    }
}
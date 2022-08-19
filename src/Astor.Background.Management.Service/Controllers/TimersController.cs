using System;
using System.Linq;
using System.Threading.Tasks;
using Astor.Background.Core;
using Astor.Background.Core.Abstractions;
using Astor.Background.Management.Protocol;
using Astor.Background.Management.Service.Timers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using ActionSchedule = Astor.Background.Management.Service.Timers.ActionSchedule;
using IntervalAction = Astor.Timers.IntervalAction;

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
            var schedule = (await this.Store.GetAllAsync()).ToArray();
            this.Logger.LogDebug($"{schedule.Count()} of schedules received - updating timers");
            
            foreach (var row in schedule)
            {
                if (row.Times is null && row.Interval is null) throw new InvalidOperationException("either times or interval must be not null");
                
                if (row.Times != null)
                {
                    var timesAction = TimesAction.WithTimeZoneShift(row.ActionId, row.EveryDayAt, this.timeZoneShift);
                    this.Timers.Ensure(timesAction, this.TriggerAction);
                }
                else
                {
                    var intervalAction = new IntervalAction(row.ActionId, row.Interval!.Value);
                    this.Timers.Ensure(intervalAction, this.TriggerAction);
                }
            }
            
            this.Timers.EnsureOnly(schedule.Select(s => s.ActionId.ToString()));
        }

        [RabbitMq.Abstractions.SubscribedOn(ExchangeNames.Schedule, DeclareExchange = true)]
        public async Task EnsureScheduleAsync(ReceiverSchedule receiverSchedule)
        {
            await this.Store.RemoveByReceiverAsync(receiverSchedule.Receiver);
            foreach (var actionSchedule in receiverSchedule.ActionSchedules)
            {
                await this.Store.AddOrUpdateAsync(new ActionSchedule
                {
                    Receiver = receiverSchedule.Receiver,
                    ActionId = actionSchedule.ActionId,
                    Interval = actionSchedule.Interval,
                    EveryDayAt = actionSchedule.EveryDayAt
                });
            }

            await this.RefreshAsync();
        }

        private void TriggerAction(string actionId)
        {
            this.Logger.LogDebug($"{actionId} timer occured at {DateTime.Now}");
            this.RabbitChannel.BasicPublish("", actionId);
        }
        
    }
}
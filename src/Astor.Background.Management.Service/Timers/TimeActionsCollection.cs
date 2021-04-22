using System;
using System.Collections.Generic;
using System.Linq;
using FluentScheduler;
using Microsoft.Extensions.Logging;

namespace Astor.Background.Management.Service.Timers
{
    public class TimeActionsCollection
    {
        public ILogger<TimeActionsCollection> Logger { get; }

        private readonly List<TimeAction> innerCollection = new();

        public TimeActionsCollection(ILogger<TimeActionsCollection> logger)
        {
            this.Logger = logger;
        }

        public void Add(TimesAction timesAction, Action<string> action)
        {
            foreach (var (time, index) in timesAction.Times.Select((x, i) => (x, i)))
            {
                this.add(new TimeAction
                {
                    ActionId = timesAction.ActionId,
                    Number = index,
                    TimeOfDay = time
                }, action);
            }
        }



        public void Add(string actionId, TimeSpan timeOfDay, Action<string> action)
        {
            var number = this.Get(actionId).Max(a => a.Number) + 1;
            this.add(new TimeAction
            {
                ActionId = actionId,
                Number = number,
                TimeOfDay = timeOfDay
            }, action);
        }

        public IEnumerable<TimeAction> Get(string actionId)
        {
            return this.innerCollection.Where(e => e.ActionId == actionId);
        }

        public IEnumerable<string> GetAllActionIds()
        {
            return this.innerCollection.Select(c => c.ActionId).Distinct();
        }

        public void RemoveByActionId(string actionId)
        {
            var timeActions = this.Get(actionId);
            foreach (var timeAction in timeActions)
            {
                this.remove(timeAction);
            }
        }
        
        public void Remove(string id)
        {
            var existing = this.innerCollection.Single(e => e.Id == id);
            this.remove(existing);
        }

        private void remove(TimeAction existing)
        {
            this.Logger.LogDebug($"removing job with id {existing.Id} which was running every day at {existing.TimeOfDay}");
            
            this.innerCollection.Remove(existing);
            JobManager.RemoveJob(existing.Id);
        }
        
        private void add(TimeAction timeAction, Action<string> action)
        {
            this.innerCollection.Add(timeAction);
            this.Logger.LogDebug($"adding job {timeAction.Id} to run every day at {timeAction.TimeOfDay}");
            JobManager.AddJob(() => action(timeAction.ActionId),
                schedule =>
                {
                    schedule.WithName(timeAction.Id)
                        .ToRunEvery(1).Days()
                        .At(timeAction.TimeOfDay.Hours, timeAction.TimeOfDay.Minutes);
                });
        }
    }
}
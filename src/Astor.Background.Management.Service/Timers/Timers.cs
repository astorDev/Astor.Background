using System;
using System.Collections.Generic;
using System.Linq;
using FluentScheduler;
using Microsoft.Extensions.Logging;

namespace Astor.Background.Management.Service.Timers
{
    public class Timers
    {
        public IntervalActionsCollection IntervalActions { get; }
        public TimeActionsCollection TimeActionsCollection { get; }
        public ILogger<Timers> Logger { get; }

        public Timers(
            IntervalActionsCollection intervalActions, 
            TimeActionsCollection timeActionsCollection,
            ILogger<Timers> logger)
        {
            this.IntervalActions = intervalActions;
            this.TimeActionsCollection = timeActionsCollection;
            this.Logger = logger;
        }
        
        public void Ensure(IntervalAction intervalAction, Action<string> action)
        {
            this.TimeActionsCollection.RemoveByActionId(intervalAction.ActionId);

            var existing = this.IntervalActions.Search(intervalAction.ActionId);
            if (existing == null)
            {
                this.Logger.LogDebug($"no interval action was found by actionId {intervalAction.ActionId} - adding new one");
                
                this.IntervalActions.Add(intervalAction, action);
                return;
            }

            if (existing.Interval != intervalAction.Interval)
            {
                this.Logger.LogDebug($"updating interval for actionId {intervalAction.ActionId} from {existing.Interval} to {intervalAction.Interval}");
                
                this.IntervalActions.EnsureRemoved(intervalAction.ActionId);
                this.IntervalActions.Add(intervalAction, action);
            }
        }

        public void Ensure(TimesAction timesAction, Action<string> action)
        {
            this.IntervalActions.EnsureRemoved(timesAction.ActionId);
            
            var existing = this.TimeActionsCollection.Get(timesAction.ActionId);
            if (!existing.Any())
            {
                this.Logger.LogDebug($"no time for action {timesAction.ActionId} - adding few");
                
                this.TimeActionsCollection.Add(timesAction, action);
                return;
            }

            var superfluous = existing.Where(e => !timesAction.Times.Any(t => e.TimeOfDay == t));
            if (superfluous.Any())
            {
                this.Logger.LogDebug($"found superfluous times for action {timesAction.ActionId} - removing them");
                
                foreach (var existingTime in superfluous)
                {
                    this.TimeActionsCollection.Remove(existingTime.Id);
                }
            }

            var missedTimes = timesAction.Times.Where(t => !existing.Any(r => r.TimeOfDay == t));
            if (missedTimes.Any())
            {
                this.Logger.LogDebug($"found missing times for action {timesAction.ActionId} - adding them");
                
                foreach (var missed in missedTimes)
                {
                    this.TimeActionsCollection.Add(timesAction.ActionId, missed, action);
                }
            }
            
        }

        public void EnsureOnly(IEnumerable<string> actionIds)
        {
            var intervalActionIds = this.IntervalActions.GetAllActionIds();
            var superfluous = intervalActionIds.Where(id => !actionIds.Contains(id));
            foreach (var superfluousIntervalActionId in superfluous)
            {
                this.IntervalActions.EnsureRemoved(superfluousIntervalActionId);
            }

            var timeActionIds = this.TimeActionsCollection.GetAllActionIds();
            var superfluousTimes = timeActionIds.Where(id => !actionIds.Contains(id));
            foreach (var superfluousTImeActionId in superfluousTimes)
            {
                this.TimeActionsCollection.RemoveByActionId(superfluousTImeActionId);
            }
        }
    }
}
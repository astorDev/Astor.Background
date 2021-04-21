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
        
        public void Ensure(IntervalAction intervalAction)
        {
            var existing = this.IntervalActions.Search(intervalAction.ActionId);
            if (existing == null)
            {
                this.Logger.LogDebug($"no interval action was found by actionId {intervalAction.ActionId} - adding new one");
                
                this.IntervalActions.Add(intervalAction);
                return;
            }

            if (existing.Interval != intervalAction.Interval)
            {
                this.Logger.LogDebug($"updating interval for actionId {intervalAction.ActionId} from {existing.Interval} to {intervalAction.Interval}");
                
                this.IntervalActions.Remove(intervalAction.ActionId);
                this.IntervalActions.Add(intervalAction);
            }
        }

        public void Ensure(TimesAction timesAction)
        {
            var existing = this.TimeActionsCollection.Get(timesAction.ActionId);
            if (!existing.Any())
            {
                this.TimeActionsCollection.Add(timesAction);
                return;
            }

            var superfluous = existing.Where(e => !timesAction.Times.Any(t => e.TimeOfDay == t));
            foreach (var existingTime in superfluous)
            {
                this.TimeActionsCollection.Remove(existingTime.Id);
            }

            var missedTimes = timesAction.Times.Where(t => !existing.Any(r => r.TimeOfDay == t));
            foreach (var missed in missedTimes)
            {
                this.TimeActionsCollection.Add(timesAction.ActionId, missed);
            }
        }

        public void EnsureOnly(IEnumerable<string> actionIds)
        {
            var intervalActionIds = this.IntervalActions.GetAllActionIds();
            var superfluous = intervalActionIds.Where(id => !actionIds.Contains(id));
            foreach (var superfluousIntervalActionId in superfluous)
            {
                this.IntervalActions.Remove(superfluousIntervalActionId);
            }

            var timeActionIds = this.TimeActionsCollection.GetAllActionIds();
            var superfluousTimes = intervalActionIds.Where(id => !actionIds.Contains(id));
            foreach (var superfluousTImeActionId in superfluous)
            {
                this.TimeActionsCollection.Remove(superfluousTImeActionId);
            }
        }
    }
}
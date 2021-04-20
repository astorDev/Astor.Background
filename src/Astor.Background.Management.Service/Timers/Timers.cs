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

        public Timers(IntervalActionsCollection intervalActions, TimeActionsCollection timeActionsCollection)
        {
            this.IntervalActions = intervalActions;
            this.TimeActionsCollection = timeActionsCollection;
        }
        
        public void Ensure(IntervalAction intervalAction)
        {
            var existing = this.IntervalActions.Search(intervalAction.ActionId);
            if (existing == null)
            {
                this.IntervalActions.Add(intervalAction);
                return;
            }

            if (existing.Interval != intervalAction.Interval)
            {
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
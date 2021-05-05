using System;
using System.Collections.Generic;
using System.Linq;
using Astor.Background.Core;
using Astor.Background.Management.Protocol;

namespace Astor.Background.Management.Helpers
{
    public class ReceiverScheduleFactory
    {
        public static ReceiverSchedule Create(TimersBasedActions timersBasedActions)
        {
            if (!timersBasedActions.Any())
            {
                throw new InvalidOperationException(
                    $"Unable to create schedule when {nameof(timersBasedActions)} is empty");
            }
            
            var actions = timersBasedActions.IntervalActions.Select(Create)
                .Union(timersBasedActions.SpecificTimesActions.Select(Create));

            return new ReceiverSchedule
            {
                Receiver = timersBasedActions.Actions.First().Id.Receiver,
                ActionSchedules = actions.ToArray()
            };
        }

        public static ActionSchedule Create(IntervalAction intervalAction)
        {
            return new()
            {
                ActionId = intervalAction.Action.Id,
                Interval = intervalAction.Attribute.Interval
            };
        }

        public static ActionSchedule Create(SpecificTimesAction specificTimesAction)
        {
            return new()
            {
                ActionId = specificTimesAction.Action.Id,
                EveryDayAt = specificTimesAction.Attributes.Select(a => a.Time).ToArray()
            };
        }
    }
}
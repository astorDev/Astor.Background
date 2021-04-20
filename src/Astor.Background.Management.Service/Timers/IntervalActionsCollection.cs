using System;
using System.Collections.Generic;
using System.Linq;
using FluentScheduler;
using Microsoft.Extensions.Logging;

namespace Astor.Background.Management.Service.Timers
{
    public class IntervalActionsCollection
    {
        public ILogger<IntervalActionsCollection> Logger { get; }
        public Action<string> Action { get; }

        private readonly List<IntervalAction> innerCollection = new();

        public IntervalActionsCollection(ILogger<IntervalActionsCollection> logger, Action<string> action)
        {
            this.Logger = logger;
            this.Action = action;
        }

        public void Add(IntervalAction intervalAction)
        {
            if (this.Any(intervalAction.ActionId))
            {
                throw new InvalidOperationException($"{intervalAction.ActionId} already added");
            }
            
            this.innerCollection.Add(intervalAction);
            
            this.Logger.LogDebug($"adding timer job {intervalAction.ActionId} with interval {intervalAction.Interval}");
            JobManager.AddJob(() => this.Action(intervalAction.ActionId),
                schedule =>
                {
                    schedule.WithName(intervalAction.ActionId)
                        .ToRunEvery((int)intervalAction.Interval.TotalMilliseconds)
                        .Milliseconds();
                });
        }

        public void Remove(string actionId)
        {
            var existing = this.Search(actionId);
            this.innerCollection.Remove(existing);
            this.Logger.LogDebug($"removing job with name {actionId}");
            JobManager.RemoveJob(actionId);
        }
        
        public IntervalAction Search(string actionId)
        {
            return this.innerCollection.SingleOrDefault(a => a.ActionId == actionId);
        }

        public IEnumerable<string> GetAllActionIds()
        {
            return this.innerCollection.Select(e => e.ActionId);
        }
        
        public bool Any(string actionId)
        {
            return this.innerCollection.Any(a => a.ActionId == actionId);
        }
    }
}
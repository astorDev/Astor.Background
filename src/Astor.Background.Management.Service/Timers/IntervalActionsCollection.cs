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

        private readonly List<IntervalAction> innerCollection = new();

        public IntervalActionsCollection(ILogger<IntervalActionsCollection> logger)
        {
            this.Logger = logger;
        }

        public void Add(IntervalAction intervalAction, Action<string> action)
        {
            if (this.Any(intervalAction.ActionId))
            {
                throw new InvalidOperationException($"{intervalAction.ActionId} already added");
            }
            
            this.innerCollection.Add(intervalAction);
            
            this.Logger.LogDebug($"adding timer job {intervalAction.ActionId} with interval {intervalAction.Interval}");
            JobManager.AddJob(() => action(intervalAction.ActionId),
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

        public string[] GetAllActionIds()
        {
            return this.innerCollection.Select(e => e.ActionId).ToArray();
        }
        
        public bool Any(string actionId)
        {
            return this.innerCollection.Any(a => a.ActionId == actionId);
        }
    }
}
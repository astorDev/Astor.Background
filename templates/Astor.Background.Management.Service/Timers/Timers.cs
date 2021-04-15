using System;
using System.Collections.Generic;
using System.Linq;
using FluentScheduler;

namespace Astor.Background.Management.Service.Timers
{
    public class Timers
    {
        public Action<string> TimerEventAction { get; }
        
        private readonly Dictionary<string, TimeSpan> intervals = new();

        private readonly Dictionary<string, IEnumerable<TimeSpan>> times = new();

        public Timers(Action<string> timerEventAction)
        {
            this.TimerEventAction = timerEventAction;
        }
        
        public void EnsureValid(string name, TimeSpan interval)
        {
            this.ensureValid(
                name, 
                interval, 
                this.registerJob, 
                this.removePeriodic, 
                timeSpanValue => this.intervals[name] != timeSpanValue);
        }

        public void EnsureValid(string name, IEnumerable<TimeSpan> times)
        {
            this.ensureValid(
                name, 
                times, 
                this.registerJob, 
                this.removeTimes, 
                timeValues =>
                {
                    var current = this.times[name];
                    return !current.SequenceEqual(timeValues);
                });
        }

        private void ensureValid<T> (string name, T obj, Action<string, T> register, Action<string> remove, Func<T, bool> valueChanged)
        {
            var schedule = JobManager.AllSchedules.FirstOrDefault(s => s.Name.StartsWith($"{name}_"));

            if (schedule == null)
            {
                register(name, obj);
            }

            if (valueChanged(obj))
            {
                remove(name);
                register(name, obj);
            }
        }

        private void removeTimes(string name)
        {
            foreach (var schedule in JobManager.AllSchedules.Where(s => s.Name.StartsWith($"{name}_")))
            {
                JobManager.RemoveJob(schedule.Name);
            }
        }
        
        private void removePeriodic(string name)
        {
            JobManager.RemoveJob(name);
        }

        public void EnsureOnly(IEnumerable<string> names)
        {
            var superfluous = this.intervals.Keys.Where(k => !names.Contains(k));
            foreach (var name in superfluous)
            {
                this.removeJob(name);
            }
        }

        private void removeJob(string name)
        {
            this.intervals.Remove(name);
            JobManager.RemoveJob(name);
        }
        
        private void registerJob(string name, TimeSpan interval)
        {
            this.intervals[name] = interval;
            JobManager.AddJob(() => this.TimerEventAction(name),
                              schedule =>
                              {
                                  schedule.WithName($"{name}_0")
                                          .ToRunEvery((int)interval.TotalMilliseconds)
                                          .Milliseconds();
                              });
        }

        private void registerJob(string name, IEnumerable<TimeSpan> times)
        {
            this.times[name] = times;

            foreach (var row in times.Select((time, i) => new { time, i }))
            {
                JobManager.AddJob(() => this.TimerEventAction(name),
                                  schedule =>
                                  {
                                      schedule.WithName($"{name}_{row.i}")
                                              .ToRunEvery(1).Days()
                                              .At(row.time.Hours, row.time.Minutes);
                                  });
            }
        }
    }
}
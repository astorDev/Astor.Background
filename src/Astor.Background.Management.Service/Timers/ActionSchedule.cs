using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;

namespace Astor.Background.Management.Service.Timers
{
    public class ActionSchedule
    {
        [BsonId]
        public string ActionId { get; set; }
        
        public int?  IntervalInMilliseconds { get; set; }

        [BsonIgnore]
        public TimeSpan? Interval
        {
            get => this.IntervalInMilliseconds == null ? null : TimeSpan.FromMilliseconds(this.IntervalInMilliseconds.Value);
            set
            {
                if (value != null)
                {
                    this.IntervalInMilliseconds = (int)value.Value.TotalMilliseconds;
                }
            } 
        }

        public string[] Times { get; set; }

        [BsonIgnore]
        public IEnumerable<TimeSpan> EveryDayAt
        {
            get => this.Times.Select(date => TimeSpan.ParseExact(date, "hh\\:mm", CultureInfo.InvariantCulture));
            set
            {
                if (value != null)
                {
                    this.Times = value.Select(t => t.ToString("hh\\:mm", CultureInfo.InvariantCulture)).ToArray();
                }
            }
        }

        public IntervalAction ToIntervalActionOrNull()
        {
            if (this.Interval != null)
            {
                return new IntervalAction
                {
                    ActionId = this.ActionId,
                    Interval = this.Interval.Value
                };
            }

            return null;
        }

        public TimesAction ToTimesAction(int timezoneShift)
        {
            if (this.Times == null)
            {
                throw new InvalidOperationException($"cannot use {nameof(ToTimesAction)} when times is null");
            }

            var shiftedTimes = this.EveryDayAt.Select(t =>
            {
                return timezoneShift switch
                {
                    0 => t,
                    < 0 => t.Subtract(TimeSpan.FromHours(Math.Abs(timezoneShift))),
                    _ => t.Add(TimeSpan.FromHours(timezoneShift))
                };
            }).ToArray();

            return new TimesAction
            {
                ActionId = this.ActionId,
                Times = shiftedTimes
            };
        }
    }
}
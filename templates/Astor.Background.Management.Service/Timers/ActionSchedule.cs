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
            get => this.Times.Select(date => TimeSpan.ParseExact(date, "H:mm", CultureInfo.InvariantCulture));
            set
            {
                if (value != null)
                {
                    this.Times = value.Select(t => t.ToString("H:mm", CultureInfo.InvariantCulture)).ToArray();
                }
            }
        }
        
        public TimeSpan? GetInterval()
        {
            return this.IntervalInMilliseconds == null ? null : TimeSpan.FromMilliseconds(this.IntervalInMilliseconds.Value);
        }

        public IEnumerable<DateTime> GetTimes()
        {
            return this.Times.Select(date => DateTime.ParseExact(date, "H:mm", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces));
        }
    }
}
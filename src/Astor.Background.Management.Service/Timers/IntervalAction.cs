using System;

namespace Astor.Background.Management.Service.Timers
{
    public class IntervalAction
    {
        public string ActionId { get; set; }
        
        public TimeSpan Interval { get; set; }
    }
}
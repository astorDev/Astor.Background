using System;

namespace Astor.Background.Management.Protocol
{
    public class ReceiverSchedule
    {
        public string Receiver { get; set; }
        
        public ActionSchedule[] ActionSchedules { get; set; }
    }

    public class ActionSchedule
    {
        public string ActionId { get; set; }

        public TimeSpan? Interval { get; set; }
        
        public TimeSpan[] EveryDayAt { get; set; }
    }
}
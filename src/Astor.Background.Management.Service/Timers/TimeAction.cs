using System;

namespace Astor.Background.Management.Service.Timers
{
    public class TimeAction
    {
        public string Id => $"{this.ActionId}_{this.Number}";
        
        public int Number { get; set; }

        public string ActionId { get; set; }
        
        public TimeSpan TimeOfDay { get; set; }
    }
}
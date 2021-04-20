using System;

namespace Astor.Background.Management.Service.Timers
{
    public class TimesAction
    {
        public string ActionId { get; set; }
        
        public TimeSpan[] Times { get; set; }
    }
}
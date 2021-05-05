using System;

namespace Astor.Background.Core.Abstractions
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RunsEveryDayAtAttribute : Attribute
    {
        public TimeSpan Time { get; set; }
        
        public RunsEveryDayAtAttribute(string time)
        {
            this.Time = TimeSpan.Parse(time);
        }
    }
}
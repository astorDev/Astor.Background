using System;
using System.Globalization;

namespace Astor.Background.Core.Abstractions
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RunsEveryAttribute : Attribute
    {
        public TimeSpan Interval { get; }

        public RunsEveryAttribute(int intervalInSeconds)
        {
            this.Interval = TimeSpan.FromSeconds(intervalInSeconds);
        }

        public RunsEveryAttribute(string interval)
        {
            this.Interval = TimeSpan.Parse(interval, CultureInfo.InvariantCulture);
        }
    }
}
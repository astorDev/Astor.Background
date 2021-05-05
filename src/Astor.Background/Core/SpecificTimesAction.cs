using System;
using Astor.Background.Core.Abstractions;

namespace Astor.Background.Core
{
    public class SpecificTimesAction
    {
        public Action Action { get; }
        
        public RunsEveryDayAtAttribute[] Attributes { get; }

        public SpecificTimesAction(Action action, RunsEveryDayAtAttribute[] attributes)
        {
            this.Action = action;
            this.Attributes = attributes;
        }
    }
}
using Astor.Background.Core.Abstractions;

namespace Astor.Background.Core
{
    public class IntervalAction
    {
        public RunsEveryAttribute Attribute { get; }
        
        public Action Action { get; }

        public IntervalAction(RunsEveryAttribute attribute, Action action)
        {
            this.Attribute = attribute;
            this.Action = action;
        }
    }
}
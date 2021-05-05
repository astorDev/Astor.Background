using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Astor.Background.Core.Abstractions;

namespace Astor.Background.Core
{
    public class TimersBasedActions : IEnumerable<Action>
    {
        public IntervalAction[] IntervalActions { get; }
        public SpecificTimesAction[] SpecificTimesActions { get; }

        private Action[] action;
        public IReadOnlyCollection<Action> Actions => this.action ??= this.getActions();

        private TimersBasedActions(IntervalAction[] intervalActions, SpecificTimesAction[] specificTimesActions)
        {
            this.IntervalActions = intervalActions;
            this.SpecificTimesActions = specificTimesActions;
        }

        private Action[] getActions() => this.IntervalActions.Select(ia => ia.Action)
            .Union(this.SpecificTimesActions.Select(sta => sta.Action)).ToArray();
        
        public static TimersBasedActions Parse(params MethodInfo[] methods)
        {
            var intervalActions = from m in methods
                from a in m.GetCustomAttributes(typeof(RunsEveryAttribute))
                select new IntervalAction((RunsEveryAttribute) a, new Action(m));

            var timeActions = from m in methods
                from a in m.GetCustomAttributes(typeof(RunsEveryDayAtAttribute))
                group new {a, m} by m into ng 
                select new SpecificTimesAction(new Action(ng.Key), ng.Select(x => (RunsEveryDayAtAttribute)x.a).ToArray());

            return new TimersBasedActions(intervalActions.ToArray(), timeActions.ToArray());
        }

        public IEnumerator<Action> GetEnumerator()
        {
            return this.Actions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
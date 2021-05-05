using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Astor.Background.Core.Abstractions;
using Newtonsoft.Json.Linq;

namespace Astor.Background.Core
{
    public class Service
    {
        public Subscription[] Subscriptions { get; }
        public IntervalAction[] IntervalActions { get; }
        public SpecificTimesAction[] SpecificTimesActions { get; }

        private Dictionary<string, Action> actions;
        public Dictionary<string, Action> Actions => this.actions ??= this.readActions();
        public IEnumerable<Type> ControllerTypes => this.Actions.Values.Select(a => a.Type).Distinct();

        public IEnumerable<Type> InputTypes => this.Actions.Values.Select(a => a.InputType).Where(i => i != null).ToArray();
        
        public Service(Subscription[] subscriptions, 
            IntervalAction[] intervalActions, 
            SpecificTimesAction[] specificTimesActions)
        {
            this.Subscriptions = subscriptions;
            this.IntervalActions = intervalActions;
            this.SpecificTimesActions = specificTimesActions;
        }

        public static Service Parse(Assembly assembly)
        {
            return Parse(assembly.DefinedTypes.ToArray());
        }

        public static Service Parse(params Type[] types)
        {
            return Parse(types.SelectMany(t => t.GetMethods()).ToArray());
        }

        public static Service Parse(params MethodInfo[] methods)
        {
            var subscriptions = from m in methods
                from a in m.GetCustomAttributes(typeof(SubscribedOnAttribute))
                select new Subscription((SubscribedOnAttribute) a, new Action(m));

            var intervalActions = from m in methods
                from a in m.GetCustomAttributes(typeof(RunsEveryAttribute))
                select new IntervalAction((RunsEveryAttribute) a, new Action(m));

            var timeActions = from m in methods
                from a in m.GetCustomAttributes(typeof(RunsEveryDayAtAttribute))
                group new {a, m} by m into ng 
                select new SpecificTimesAction(new Action(ng.Key), ng.Select(x => (RunsEveryDayAtAttribute)x.a).ToArray());

            return new Service(subscriptions.ToArray(), intervalActions.ToArray(), timeActions.ToArray());
        }

        private Dictionary<string, Action> readActions()
        {
            var result = new Dictionary<string, Action>();
            
            var actionsRaw = this.Subscriptions.Select(s => s.Action)
                .Union(this.IntervalActions.Select(s => s.Action))
                .Union(this.SpecificTimesActions.Select(ta => ta.Action));
            
            foreach (var a in actionsRaw)
            {
                result.TryAdd(a.Id, a);
            }

            return result;
        }
    }
}
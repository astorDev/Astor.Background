using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Astor.Background.Core.Abstractions;

namespace Astor.Background.Core
{
    public class Service
    {
        public Subscription[] Subscriptions { get; }

        private Dictionary<string, Action> actions;
        public Dictionary<string, Action> Actions => this.actions ??= this.readActions();
        public IEnumerable<Type> ControllerTypes => this.Actions.Values.Select(a => a.Type).Distinct();
        
        public Service(Subscription[] subscriptions)
        {
            this.Subscriptions = subscriptions;
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

            return new Service(subscriptions.ToArray());
        }

        private Dictionary<string, Action> readActions()
        {
            var result = new Dictionary<string, Action>();
            
            foreach (var subscription in this.Subscriptions)
            {
                result.TryAdd(subscription.Action.Id, subscription.Action);
            }

            return result;
        }
    }
}
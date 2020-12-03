using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Astor.Background.Abstractions;

namespace Astor.Background
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
            var subscriptions = from t in assembly.DefinedTypes
                from m in t.DeclaredMethods
                from a in m.GetCustomAttributes(typeof(SubscribedOnAttribute))
                select new Subscription((SubscribedOnAttribute)a, new Action(m, t));

            return new Service(subscriptions.ToArray());
        }

        public Task<object> RunAsync(string actionId, string inputJson, IServiceProvider serviceProvider)
        {
            var action = this.Actions[actionId];
            return action.ExecuteAsync(inputJson, serviceProvider);
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
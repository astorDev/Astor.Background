using System;
using System.Linq;
using System.Reflection;
using Astor.Background.Abstractions;

namespace Astor.Background
{
    public class Service
    {
        public Subscription[] Subscriptions { get; }

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
    }
}
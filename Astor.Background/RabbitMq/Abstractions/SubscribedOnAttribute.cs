using System;

namespace Astor.Background.RabbitMq.Abstractions
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SubscribedOnAttribute : Astor.Background.Core.Abstractions.SubscribedOnAttribute
    {
        public bool DeclareExchange = false;

        public bool DeclareAndBindQueue = true;

        public SubscribedOnAttribute(object @event) : base(@event)
        {
        }
    }
}
using System;

namespace Astor.Background.Core.Abstractions
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SubscribedOnInternalAttribute : SubscribedOnAttribute
    {
        public SubscribedOnInternalAttribute(object @event) : base(@event)
        {
        }
    }
}
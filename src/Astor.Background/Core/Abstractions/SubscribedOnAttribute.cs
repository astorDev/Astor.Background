using System;

namespace Astor.Background.Core.Abstractions
{
    public class SubscribedOnAttribute : Attribute
    {
        public object Event { get; }
    
        public SubscribedOnAttribute(object @event)
        {
            this.Event = @event;
        }
    }
}
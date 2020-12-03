using System;

namespace Astor.Background.Abstractions
{
    public class SubscribedOnAttribute : Attribute
    {
        public string Event { get; }

        public SubscribedOnAttribute(string @event)
        {
            this.Event = @event;
        }
    }
}
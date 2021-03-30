using System;
using System.Diagnostics.CodeAnalysis;

namespace Astor.Background.Core.Abstractions
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SubscribedOnAttribute : Attribute
    {
        public object Event { get; }
    
        public SubscribedOnAttribute(object @event)
        {
            this.Event = @event;
        }
    }
}
using System;

namespace Astor.Background.Core.Abstractions
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AcceptsDirectMessagesAttribute : Attribute
    {
    }
}
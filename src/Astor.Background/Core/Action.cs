using System;
using System.Linq;
using System.Reflection;

namespace Astor.Background.Core
{
    public class Action
    {
        public ActionId Id { get; }
        public MethodInfo Method { get; }
        public Type Type { get; }
        public Type? InputType { get; }

        public Action(MethodInfo method)
        {
            this.Method = method;
            this.Type = method.DeclaringType;
            this.Id = new ActionId(method.DeclaringType!.Name.Replace("Controller", ""),
                this.Method.Name.Replace("Async", ""));
            this.InputType = method.GetParameters().SingleOrDefault()?.ParameterType;
        }
    }
}
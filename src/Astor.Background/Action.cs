using System;
using System.Linq;
using System.Reflection;

namespace Astor.Background
{
    public class Action
    {
        public string Id { get; }
        public MethodInfo Method { get; }
        public Type Type { get; }
        public Type InputType { get; }

        public Action(MethodInfo method)
        {
            this.Method = method;
            this.Type = method.DeclaringType;
            this.Id = $"{method.DeclaringType!.Name.Replace("Controller", "")}_{this.Method.Name.Replace("Async", "")}";
            this.InputType = method.GetParameters().Single().ParameterType;
        }
    }
}
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Astor.Background
{
    public class Action
    {
        public string Id { get; }
        public MethodInfo Method { get; }
        public Type Type { get; }
        
        public Type InputType { get; }

        public Action(MethodInfo method, Type type)
        {
            this.Method = method;
            this.Type = type;
            this.Id = $"{this.Type.Name.Replace("Controller", "")}_{this.Method.Name.Replace("Async", "")}";
            this.InputType = method.GetParameters().Single().ParameterType;
        }

        public Task<object> ExecuteAsync(string inputJson, IServiceProvider serviceProvider)
        {
            var controller = serviceProvider.GetRequiredService(this.Type);
            var input = JsonConvert.DeserializeObject(inputJson, this.InputType);

            return this.Method.InvokeAsync(controller, input);
        }
    }
}
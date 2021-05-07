using System;
using System.Threading.Tasks;
using Astor.Background.Reflection;
using Astor.GreenPipes;
using GreenPipes;
using Microsoft.Extensions.DependencyInjection;

namespace Astor.Background.Core.Filters
{
    public class ActionExecutor : IFilter<EventContext>
    {
        public IServiceProvider ServiceProvider { get; }

        public ActionExecutor(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }
        
        public async Task Send(EventContext context, IPipe<EventContext> next)
        {
            var controller = this.ServiceProvider.GetRequiredService(context.Action.Type);
            context.ActionResult.Output = await this.invokeAsync(context, controller);

            await next.Send(context);
        }

        private Task<object> invokeAsync(EventContext context, object controller)
        {
            return context.Input.BodyObject == null
                ? context.Action.Method.InvokeAsync(controller)
                : context.Action.Method.InvokeAsync(controller, context.Input.BodyObject);
        }
        
        public void Probe(ProbeContext context)
        {
        }
    }
    
    public static class ActionExecutorRegistrationExtension
    {
        public static PipeBuilder<EventContext> UseActionExecutor(
            this PipeBuilder<EventContext> pipeBuilder)=>
            pipeBuilder.Use<ActionExecutor>();
    }
}
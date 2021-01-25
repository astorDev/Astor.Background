using System;
using System.Threading.Tasks;
using Astor.GreenPipes;
using Astor.Reflection;
using GreenPipes;
using Microsoft.Extensions.DependencyInjection;

namespace Astor.Background.Filters
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
            context.ActionResult.Output = await context.Action.Method.InvokeAsync(controller, context.Input.BodyObject);

            await next.Send(context);
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
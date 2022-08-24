using System;
using System.Threading.Tasks;

using GreenPipes;

using Microsoft.Extensions.Logging;

namespace Astor.Background.Core.Filters;

public class IOLogger : IFilter<EventContext>
{
    readonly ILogger<IOLogger> logger;

    public IOLogger(ILogger<IOLogger> logger)
    {
        this.logger = logger;
    }
    
    public async Task Send(EventContext context, IPipe<EventContext> next)
    {
        await next.Send(context);
        this.logger.LogInformation(
            @"
{actionId}
{success}
{exception}
{result}
{startTime}
{elapsed}
{input}",
            context.Action.Id.Id,
            context.ActionResult.Exception == null,
            context.ActionResult.Exception == null ? null : ExceptionJson.From(context.ActionResult.Exception),
            context.ActionResult.Output,
            context.HandlingParams.StartTime,
            context.HandlingParams.EndTime - context.HandlingParams.StartTime,
            context.Input.BodyObject
        );
    }

    public record ExceptionJson(string Message, string? StackTrace, string? InnerExceptionMessage)
    {
        public static ExceptionJson From(Exception ex) => 
            new(ex.Message, ex.StackTrace, ex.InnerException?.Message);
    }

    public void Probe(ProbeContext context)
    {
    }
}
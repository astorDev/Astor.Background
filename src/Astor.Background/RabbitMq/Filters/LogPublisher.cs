using System.Threading.Tasks;
using Astor.Background.Core;
using Astor.Background.Management.Protocol;
using Astor.RabbitMq;
using GreenPipes;
using RabbitMQ.Client;

namespace Astor.Background.RabbitMq.Filters
{
    public class LogPublisher : IFilter<EventContext>
    {
        public IModel Channel { get; }

        public LogPublisher(IModel channel)
        {
            this.Channel = channel;
        }
        
        public async Task Send(EventContext context, IPipe<EventContext> next)
        {
            await next.Send(context);

            this.Channel.PublishJson(ExchangeNames.Logs, new ActionResultCandidate
            {
                ActionId = context.Action.Id,
                AttemptIndex = 0,
                EndTime = context.HandlingParams.EndTime,
                StartTime = context.HandlingParams.StartTime,
                Event = context.Input.BodyObject,
                Exception = context.ActionResult.Exception == null
                    ? null
                    : new Exception
                    {
                        Message = context.ActionResult.Exception.Message,
                        StackTrace = context.ActionResult.Exception.StackTrace
                    },
                IsSuccessful = context.ActionResult.Exception == null,
                SourceExchange = (string) context.Input.Headers[InputHelper.HeaderNames.Exchange],
                Result = context.ActionResult.Output
            });
        }

        public void Probe(ProbeContext context)
        {
        }
    }
}
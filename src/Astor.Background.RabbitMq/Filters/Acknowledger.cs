using System.Threading.Tasks;
using GreenPipes;
using RabbitMQ.Client;

namespace Astor.Background.RabbitMq.Filters
{
    public class Acknowledger : IFilter<EventContext>
    {
        public IModel Channel { get; }

        public Acknowledger(IModel channel)
        {
            this.Channel = channel;
        }
        
        public async Task Send(EventContext context, IPipe<EventContext> next)
        {
            await next.Send(context);
            var deliveryTag = (ulong) context.Input.Headers[InputHelper.HeaderNames.DeliveryTag];
            this.Channel.BasicAck(deliveryTag, false);
        }

        public void Probe(ProbeContext context)
        {
        }
    }
}
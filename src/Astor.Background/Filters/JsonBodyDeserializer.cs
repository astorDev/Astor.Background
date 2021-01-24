using System.Threading.Tasks;
using Astor.GreenPipes;
using GreenPipes;
using Newtonsoft.Json;

namespace Astor.Background.Filters
{
    public class JsonBodyDeserializer : IFilter<EventContext>
    {
        public Task Send(EventContext context, IPipe<EventContext> next)
        {
            context.Input.BodyObject = JsonConvert.DeserializeObject(context.Input.BodyString, context.Action.InputType);
            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
        }
    }

    public static class JsonBodyDeserializationFilterExtension
    {
        public static PipeBuilder<EventContext> UseJsonBodyDeserializer(this PipeBuilder<EventContext> builder)
        {
            return builder.Use<JsonBodyDeserializer>();
        }
    }
}
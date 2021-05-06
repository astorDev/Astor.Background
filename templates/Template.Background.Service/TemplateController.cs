using System.Threading.Tasks;
using Astor.Background.Core.Abstractions;

namespace Template.Background.Service
{
    public class TemplateController
    {
        [Astor.Background.RabbitMq.Abstractions.SubscribedOn("eventName", DeclareExchange = true)]
        public async Task<string> ReceiveAsync(Event @event)
        {
            return $"received {@event.Text}";
        }

        [RunsEvery("00:01:00")]
        public string Health()
        {
            return "I'm alive";
        }
    }

    public class Event
    {
        public string Text { get; set; }
    }
}
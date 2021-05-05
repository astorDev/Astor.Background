using System.Linq;
using Astor.Background.Core;

namespace Astor.Background.RabbitMq
{
    public class Service
    {
        public Subscription[] Subscriptions { get; init; }
        
        public TimersBasedActions TimersBasedActions { get; init; }
        
        public string[] InternalEventsForPublishing { get; init; }

        public string InternalExchangesPrefix { get; init; }

        private Service()
        {
        }

        public static string InternalExchangeName(string internalExchangesPrefix, object @event) => $"{internalExchangesPrefix}.background-service.{@event}";

        public string InternalExchangeName(object @event) => InternalExchangeName(this.InternalExchangesPrefix, @event);
        
        public static Service Create(Core.Service coreService, string internalExchangesPrefix = null)
        {
            var subscriptions = coreService.Subscriptions
                .Select(s => Subscription.Create(s, internalExchangesPrefix))
                .ToArray();
            
            return new Service
            {
                Subscriptions = subscriptions,
                InternalEventsForPublishing = subscriptions.Select(s => s.InternalExchangeName).Distinct().ToArray(),
                InternalExchangesPrefix = internalExchangesPrefix,
                TimersBasedActions = coreService.TimersBasedActions
            };
        }
    }
}
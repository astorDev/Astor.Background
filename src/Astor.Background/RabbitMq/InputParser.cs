using System.Collections.Generic;
using System.Text;
using Astor.Background.Core;
using RabbitMQ.Client.Events;

namespace Astor.Background.RabbitMq
{
    public static class InputHelper
    {
        public static Input Parse(BasicDeliverEventArgs eventArgs)
        {
            return new()
            {
                Headers = new Dictionary<string, object>
                {
                    { HeaderNames.DeliveryTag, eventArgs.DeliveryTag},
                    { HeaderNames.Exchange, eventArgs.Exchange }
                },
                BodyString = Encoding.UTF8.GetString(eventArgs.Body.ToArray())
            };
        }

        public static class HeaderNames
        {
            public const string DeliveryTag = "deliveryTag";
            public const string Exchange = "exchange";
        }
    }
}
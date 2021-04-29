using System.Collections.Generic;

namespace Astor.RabbitMq
{
    public class QueueSettings
    {
        public bool Durable { get; set; } = true;

        public bool Exclusive { get; set; } = false;

        public bool AutoDelete { get; set; } = false;

        public Dictionary<string, object> Arguments { get; set; } = new();
    }
}
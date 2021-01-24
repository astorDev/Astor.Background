using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Astor.RabbitMq
{
    public static  class JsonPublishHelper
    {
        public static void PublishJson(this IModel channel, string exchange, object payload)
        {
            var json = JsonConvert.SerializeObject(payload);
            var jsonBytes = Encoding.UTF8.GetBytes(json);
            channel.BasicPublish(exchange, "", null, jsonBytes);
        }
    }
}
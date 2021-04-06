using Astor.Background.Management.Protocol;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;

namespace Astor.Background.Management.Service
{
    public static class Setup
    {
        public static void Init(this IHost host)
        {
            var channel = host.Services.GetRequiredService<IModel>();
            channel.ExchangeDeclare(ExchangeNames.Logs, "fanout", true, false);
            
            
        }
    }
}
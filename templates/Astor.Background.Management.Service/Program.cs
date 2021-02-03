using System.Threading.Tasks;
using Astor.Background.Core;
using Astor.Background.Management.Protocol;
using Astor.GreenPipes;
using Example.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;

namespace Astor.Background.Management.Service
{
    public class Program
    {
        public static Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureHostConfiguration(config =>
                {
                    config.AddUserSecrets(typeof(Program).Assembly);
                    config.AddCommandLine(args);
                })
                .ConfigureServices((host, services) =>
                {
                    services.AddSingleton<IHostedService, Astor.Background.RabbitMq.BackgroundService>();
                    var startup = new Startup(host.Configuration);
                    startup.ConfigureServices(services);
                    var pipeBuilder = new PipeBuilder<EventContext>(services);
                    startup.ConfigurePipe(pipeBuilder);
                    pipeBuilder.RegisterPipe();
                });

            var host = builder.UseConsoleLifetime().Build();

            var channel = host.Services.GetRequiredService<IModel>();
            channel.ExchangeDeclare(ExchangeNames.Logs, "fanout", true, false);
            
            return host.RunAsync();
        }
    }
}
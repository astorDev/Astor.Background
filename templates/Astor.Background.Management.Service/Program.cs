using System.Threading.Tasks;
using Astor.Background.Core;
using Astor.GreenPipes;
using Example.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Astor.Background.Management.Service
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureHostConfiguration(config =>
                {
                    config.AddUserSecrets(typeof(Program).Assembly);
                    config.AddCommandLine(args);
                })
                .ConfigureAppConfiguration((context, config) => 
                {
                    var environment = context.HostingEnvironment.EnvironmentName;

                    config.AddJsonFile("appsettings.json");
                    config.AddJsonFile($"appsettings.{environment}.json");
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

            await host.RunAsync();
        }
    }
}
using System;
using System.Threading.Tasks;
using Astor.Background.Core;
using Astor.GreenPipes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Astor.Background.ElasticLogs.Service
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var hostBuilder = CreateHostBuilder(args);
            
            var host = hostBuilder.UseConsoleLifetime().Build();

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return new HostBuilder()
                .ConfigureHostConfiguration(config =>
                {
                    config.AddCommandLine(args);
                })
                .ConfigureAppConfiguration((context, config) => 
                {
                    var environment = context.HostingEnvironment.EnvironmentName;

                    config.AddJsonFile("appsettings.json");
                    config.AddJsonFile($"appsettings.{environment}.json", true);
                })
                .ConfigureServices((host, services) =>
                {
                    services.AddSingleton<IHostedService, Astor.Background.RabbitMq.BackgroundService>();
                    var startup = new Startup(host.Configuration);
                    startup.ConfigureServices(services);
                    var pipeBuilder = new PipeBuilder<EventContext>(services);
                    startup.ConfigurePipe(pipeBuilder);
                    pipeBuilder.RegisterPipe();
                })
                .ConfigureLogging(loggingBuilder => 
                    loggingBuilder.AddConsole());
        }
    }
}
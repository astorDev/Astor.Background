using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Astor.Background.Core;
using Astor.GreenPipes;
using Example.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Astor.Background.Management.Service
{
    public class Program
    {
        public static Task Main(string[] args)
        {
            try
            {
                var host = CreateHost(args);
                return host.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return new HostBuilder()
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
                })
                .ConfigureLogging(logging => { logging.AddConsole();});
        }
        
        public static IHost CreateHost(string[] args)
        {
            var builder = CreateHostBuilder(args);

            return builder.UseConsoleLifetime().Build();
        }
    }
}
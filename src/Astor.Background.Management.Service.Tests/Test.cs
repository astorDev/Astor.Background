using System;
using System.Collections.Generic;
using Astor.Background.TelegramNotifications;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

namespace Astor.Background.Management.Service.Tests
{
    public class Test
    {
        public static IHost StartHost()
        {
            var hostBuilder = Program.CreateHostBuilder(new[]
            {
                "--ConnectionStrings:Rabbit=amqp://localhost:5672",
                "--ConnectionStrings:Mongo=mongodb://localhost:27017"
            });

            hostBuilder.ConfigureServices(s =>
            {
                s.AddSingleton(A.Fake<ITelegramBotClient>());
                s.AddSingleton(A.Fake<TelegramNotifier>());
            });

            var host = hostBuilder.UseConsoleLifetime().Build();
            
            var hostedServices = host.Services.GetService<IEnumerable<IHostedService>>();
            
            // try
            // {
            //     var hostedServices = host.Services.GetService<IEnumerable<IHostedService>>();
            // }
            // catch
            // {
            //     Console.WriteLine("here we go");
            //     
            // }
            
            host.RunAsync();

            return host;
        }
    }
}
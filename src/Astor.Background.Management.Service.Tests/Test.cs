using System;
using System.Collections.Generic;
using Astor.Background.TelegramNotifications;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Telegram.Bot;

namespace Astor.Background.Management.Service.Tests
{
    public class Test
    {
        [TestInitialize]
        public void Cleanup()
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            mongoClient.DropDatabase("background");
        }
        
        public static IHost StartHost()
        {
            var hostBuilder = Program.CreateHostBuilder(new[]
            {
                "--ConnectionStrings:Rabbit=amqp://localhost:5672",
                "--ConnectionStrings:Mongo=mongodb://localhost:27017",
                "--InternalExchangePrefix=my-autotests"
            });

            hostBuilder.ConfigureServices(s =>
            {
                s.AddSingleton(A.Fake<ITelegramBotClient>());
                s.AddSingleton(A.Fake<TelegramNotifier>());
            });

            var host = hostBuilder.UseConsoleLifetime().Build();
            
            //In order to avoid silent exceptions while resolving services
            //They are silent because RunAsync is not awaited
            host.Services.GetService<IEnumerable<IHostedService>>();

            host.RunAsync();
            
            return host;
        }
    }
}
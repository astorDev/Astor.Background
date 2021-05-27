using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Astor.Background.RabbitMq;
using Astor.Tests;
using Example.Service.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Astor.Background.Tests.Integrations
{
    [TestClass]
    public class BackgroundService_Should_RegardingLoopActions : RabbitMqBackgroundServiceTest
    {
        [TestMethod]
        public async Task RunThemOnALoop()
        {
            var backgroundService = this.ServiceProvider.GetRequiredService<BackgroundService>();

            var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(3)).Token;

            await backgroundService.StartAsync(cancellationToken);

            await Waiting.For(cancellationToken);

            var textStore = this.ServiceProvider.GetRequiredService<TextStore>();
            
            Assert.AreEqual(GreetingsController.BoredMessage, textStore.TextOne);
            Assert.AreEqual(GreetingsController.WavingHand, textStore.Messages.FirstOrDefault());
            textStore.WriteMessagesToConsole();
        }
    }
}
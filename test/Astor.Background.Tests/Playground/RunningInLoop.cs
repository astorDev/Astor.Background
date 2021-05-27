using System;
using System.Threading;
using System.Threading.Tasks;
using Astor.Background.Core;
using Astor.Background.Core.Filters;
using Astor.Background.RabbitMq;
using Astor.GreenPipes;
using Astor.Tests;
using Example.Service.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Action = Astor.Background.Core.Action;

namespace Astor.Background.Tests.Playground
{
    [TestClass]
    public class RunningInLoop : GreetingsBasedTest
    {
        [TestMethod]
        public async Task WithBeingAwaited()
        {
            var context = createEventContext();

            await this.runForFiveSecsAsync(context);

            this.writeTextStoreMessages();
        }

        [TestMethod]
        public async Task WithoutBeingAwaited()
        {
            var context = createEventContext();

            this.runForFiveSecsAsync(context);

            await Waiting.For(TimeSpan.FromSeconds(6));
            
            this.writeTextStoreMessages();
        }

        public override IServiceCollection CreateBaseServiceCollection()
        {
            var serviceCollection = base.CreateBaseServiceCollection();
            
            ;
            
            new PipeBuilder<EventContext>(serviceCollection)
                .Use<ExceptionsLogger>()
                .Use<ActionExceptionCatcher>()
                .Use<ActionExecutor>()
                .RegisterPipe();

            return serviceCollection;
        }
        
        private static EventContext createEventContext()
        {
            var action = new Action(typeof(GreetingsController).GetMethod("WaveAsync"));
            var context = new EventContext(action, new Input());
            return context;
        }

        private async Task runForFiveSecsAsync(EventContext context)
        {
            var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token;
            while (!cancellationToken.IsCancellationRequested)
            {
                await HandlingLogic.ExecuteAsync(context, this.ServiceProvider);
            }
        }
        
        private void writeTextStoreMessages()
        {
            Console.WriteLine("messages from text store: ");
            
            foreach (var message in this.ServiceProvider.GetRequiredService<TextStore>().Messages)
            {
                Console.WriteLine(message);
            }
        }
    }
}
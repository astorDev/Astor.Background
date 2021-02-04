using System;
using System.Linq;
using System.Threading.Tasks;
using Astor.Background.Core;
using Astor.Background.Core.Filters;
using Astor.Background.TelegramNotifications;
using Example.Service.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telegram.Bot;

namespace Astor.Background.Tests.Filters
{
    [TestClass]
    [Ignore("no mocks yet")]
    public class ExceptionsCatcherWithTelegramNotifications_Should : FilterTest
    {
        private const string token = "foo";
        private const int chatId = -1; //provide your char id
        
        [TestMethod]
        public async Task SendMessageToTelegram()
        {
            this.ServiceCollection.AddBackground(typeof(GreetingsController));
            this.ServiceCollection.AddSingleton<ITelegramBotClient>(new TelegramBotClient(token));
            this.ServiceCollection.AddSingleton(sp =>
            {
                var bot = sp.GetRequiredService<ITelegramBotClient>();
                return new TelegramNotifier(bot, chatId);
            });

            var context = new EventContext(GreetingAction, new Input
            {
                BodyString = "{ 'name' : 'Mathew'}"
            });

            await this.RunPipeAsync(pipe =>
                    pipe.Use<ExceptionCatcherWithTelegramNotifications>()
                        .Use<JsonBodyDeserializer>()
                        .Use<ActionExecutor>()
                ,
                context
            );
        }

        [TestMethod]
        public async Task SendBigMessagesToTelegram()
        {
            this.ServiceCollection.AddSingleton<ITelegramBotClient>(new TelegramBotClient(token));
            this.ServiceCollection.AddSingleton(sp =>
            {
                var bot = sp.GetRequiredService<ITelegramBotClient>();
                return new TelegramNotifier(bot, chatId);
            });

            var notifier = this.ServiceProvider.GetRequiredService<TelegramNotifier>();

            var chars = Enumerable.Range(0, 5000).Select(i => 'a').ToArray();
            
            await notifier.SendAsync(new Exception("test"), new EventContext(GreetingAction, new Input
            {
                BodyString = new string(chars)
            }));
        }
    }
}
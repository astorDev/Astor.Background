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
    public class ExceptionsCatcherWithTelegramNotifications_Should : FilterTest
    {
        [TestMethod]
        [Ignore("No mocks yet")]
        public async Task SendMessageToTelegram()
        {
            const string token = "provide your bot token";
            const int chatId = -1; //provide your chat id
            
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
    }
}
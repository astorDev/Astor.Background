using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Astor.Background.Core;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace Astor.Background.TelegramNotifications
{
    public class TelegramNotifier
    {
        public ITelegramBotClient Bot { get; }
        public ChatId ChatId { get; }

        public TelegramNotifier(ITelegramBotClient bot, ChatId chatId)
        {
            this.Bot = bot;
            this.ChatId = chatId;
        }

        public async Task SendAsync(Exception exception, EventContext context)
        {
            var exceptionHtml = HttpUtility.HtmlEncode(exception);
            var inputBody = HttpUtility.HtmlEncode(context.Input.BodyString);

            var html = @$"
                <b>Exception occured while handling event:</b> 

            {exceptionHtml}

            <b>Input body:</b>

            {inputBody}
            ";

            if (exceptionHtml.Length + inputBody.Length > 4000)
            {
                var fileName = $"{Guid.NewGuid().ToString()}.html";

                await using var ms = new MemoryStream(Encoding.UTF8.GetBytes(html));
                var file = new InputOnlineFile(ms, fileName);
                await this.Bot.SendDocumentAsync(this.ChatId, file, "Exception occured while handling event:");
                return;
            }

            await this.Bot.SendTextMessageAsync(this.ChatId, new string(html.Take(5000).ToArray()), ParseMode.Html);
        }
    }
}
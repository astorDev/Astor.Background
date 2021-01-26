using System;
using System.Threading.Tasks;
using System.Web;
using Astor.Background.Core;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

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

        public Task SendAsync(Exception exception, EventContext context)
        {
            var html = @$"
                <b>Exception occured while handling event:</b> 

            {HttpUtility.HtmlEncode(exception)}

            <b>Input body:</b>

            {HttpUtility.HtmlEncode(context.Input.BodyString)}
            ";

            return this.Bot.SendTextMessageAsync(this.ChatId, html, ParseMode.Html);
        }
    }
}
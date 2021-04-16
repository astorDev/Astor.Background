using System;
using Astor.Background.Core;
using Astor.Background.Core.Filters;
using Astor.Background.RabbitMq.Filters;
using Astor.Background.TelegramNotifications;
using Astor.GreenPipes;
using Astor.RabbitMq;
using Example.Service.Domain;
using Example.Service.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace Example.Service
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddBackground(this.GetType().Assembly);
            services.AddRabbit(this.Configuration.GetSection("Rabbit"));
            services.Configure<GreetingPhrases>(this.Configuration.GetSection("Phrases"));
        }

        public void ConfigurePipeServices(IServiceCollection services)
        {
            services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(this.Configuration["Telegram:BotToken"]));
            services.AddSingleton(sp =>
            {
                var botClient = sp.GetRequiredService<ITelegramBotClient>();
                var chatId = Int64.Parse(this.Configuration["Telegram:ChatId"]);
                return new TelegramNotifier(botClient, chatId);
            });
        }
        
        public void ConfigurePipe(PipeBuilder<EventContext> builder)
        {
            builder
                    .Use<Acknowledger>()
                    .Use<LogPublisher>()
                    .Use<HandlingTimer>()
                    .Use<ExceptionCatcherWithTelegramNotifications>()
                    .Use<JsonBodyDeserializer>()
                    .Use<ActionExceptionCatcher>()
                    .Use<ActionExecutor>()
                ;
        }
    }
}
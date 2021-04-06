using System;
using Astor.Background.Core;
using Astor.Background.Core.Filters;
using Astor.Background.RabbitMq.Filters;
using Astor.Background.TelegramNotifications;
using Astor.GreenPipes;
using Astor.RabbitMq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using RabbitMQ.Client;
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

            var rabbitConnectionString = this.Configuration.GetConnectionString("Rabbit");
            services.AddRabbit(rabbitConnectionString);

            services.AddSingleton<ITelegramBotClient>(sp => new TelegramBotClient(this.Configuration["Telegram:Token"]));
            services.AddSingleton(sp =>
            {
                var botClient = sp.GetRequiredService<ITelegramBotClient>();
                var chatId = Int64.Parse(this.Configuration["Telegram:ChatId"]);
                return new TelegramNotifier(botClient, chatId);
            });

            services.AddSingleton<IMongoClient>(new MongoClient(this.Configuration.GetConnectionString("Mongo")));
            services.AddSingleton(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                var dbName = this.Configuration["Mongo:DbName"] ?? "background";
                
                return client.GetDatabase(dbName);
            });
        }

        public void ConfigurePipe(PipeBuilder<EventContext> builder)
        {
            builder
                .Use<Acknowledger>()
                .Use<ExceptionCatcherWithTelegramNotifications>()
                .Use<JsonBodyDeserializer>()
                .Use<ActionExecutor>()
                ;
        }
    }
}
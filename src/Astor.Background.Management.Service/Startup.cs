using System;
using Astor.Background.Core;
using Astor.Background.Core.Filters;
using Astor.Background.Management.Service.Timers;
using Astor.Background.RabbitMq.Filters;
using Astor.Background.TelegramNotifications;
using Astor.GreenPipes;
using Astor.RabbitMq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Conventions;
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
            services.AddSingleton<Astor.Background.RabbitMq.Service>(sp =>
            {
                var coreService = sp.GetRequiredService<Astor.Background.Core.Service>();
                return Astor.Background.RabbitMq.Service.Create(coreService, this.Configuration["InternalExchangePrefix"]);
            });

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
            
            services.AddSingleton(sp =>
            {
                var mongodb = sp.GetRequiredService<IMongoDatabase>();
                var mongoCollection = mongodb.GetCollection<ActionSchedule>("schedule");

                return new SchedulesStore(mongoCollection);
            });

            services.AddSingleton<IntervalActionsCollection>();
            services.AddSingleton<TimeActionsCollection>();
            services.AddSingleton<Timers>();
            
            var pack = new ConventionPack();
            pack.AddRange(new IConvention[]
            {
                new IgnoreExtraElementsConvention(true)
            });
            ConventionRegistry.Register("myConventions", pack, t => true);
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
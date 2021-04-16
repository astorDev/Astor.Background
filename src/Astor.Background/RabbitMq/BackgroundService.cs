using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Astor.Background.Core;
using Astor.Background.RabbitMq.Abstractions;
using Astor.RabbitMq;
using GreenPipes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Astor.Background.RabbitMq
{
    public class BackgroundService : IHostedService
    {
        public IModel Channel { get; }
        public IServiceProvider ServiceProvider { get; }
        public Service Service { get; }
        public ILogger<BackgroundService> Logger { get; }
        public IConfiguration Configuration { get; }

        public BackgroundService(IModel channel, 
            IServiceProvider serviceProvider, 
            Service service,
            ILogger<BackgroundService> logger,
            IConfiguration configuration)
        {
            this.Channel = channel;
            this.ServiceProvider = serviceProvider;
            this.Service = service;
            this.Logger = logger;
            this.Configuration = configuration;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                foreach (var subscription in this.Service.Subscriptions)
                {
                    subscription.Register(this.Channel, this.ServiceProvider);
                }

                if (this.Service.InternalEventsForPublishing.Contains(InternalEventNames.Started))
                {
                    this.Channel.PublishJson(this.Service.InternalExchangeName(InternalEventNames.Started), null);
                }
                
            }
            catch (Exception ex)
            {
                this.Logger.LogCritical(ex, "exception occured while starting background service");
                throw;
            }
            
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
        }
    }
}
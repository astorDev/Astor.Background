using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Astor.Background.Core;
using Astor.Background.Management.Helpers;
using Astor.Background.Management.Protocol;
using Astor.Background.RabbitMq.Abstractions;
using Astor.RabbitMq;
using GreenPipes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Exception = System.Exception;

namespace Astor.Background.RabbitMq
{
    public class BackgroundService : IHostedService
    {
        public IModel Channel { get; }
        public IServiceProvider ServiceProvider { get; }
        public Service Service { get; }
        public ILogger<BackgroundService> Logger { get; }

        public BackgroundService(IModel channel, 
            IServiceProvider serviceProvider, 
            Service service,
            ILogger<BackgroundService> logger)
        {
            this.Channel = channel;
            this.ServiceProvider = serviceProvider;
            this.Service = service;
            this.Logger = logger;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                this.registerSubscriptions();

                this.registerTimers();

                this.publishStartedEventIfNeeded();
            }
            catch (Exception ex)
            {
                this.Logger.LogCritical(ex, "exception occured while starting background service");
                throw;
            }
            
            return Task.CompletedTask;
        }

        private void publishStartedEventIfNeeded()
        {
            if (this.Service.InternalEventsForPublishing.Contains(InternalEventNames.Started))
            {
                this.Channel.PublishJson(this.Service.InternalExchangeName(InternalEventNames.Started), null);
            }
        }

        private void registerTimers()
        {
            if (this.Service.TimersBasedActions.Any())
            {
                var schedule = ReceiverScheduleFactory.Create(this.Service.TimersBasedActions);
                this.Channel.PublishJson(ExchangeNames.Schedule, schedule);
            }

            foreach (var serviceTimersBasedAction in this.Service.TimersBasedActions)
            {
                serviceTimersBasedAction.DeclareAndConsumeQueue(this.Channel, this.ServiceProvider);
            }
        }

        private void registerSubscriptions()
        {
            foreach (var subscription in this.Service.Subscriptions)
            {
                subscription.Register(this.Channel, this.ServiceProvider);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
        }
    }
}
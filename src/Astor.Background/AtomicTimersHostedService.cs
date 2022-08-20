using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Astor.Background.Core;
using Astor.Timers;

using FluentScheduler;

using GreenPipes;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Namotion.Reflection;

using Service = Astor.Background.Core.Service;

namespace Astor.Background;

public class AtomicTimersHostedService : IHostedService
{
    public Service Service { get; }
    public IServiceProvider ServiceProvider { get; }
    public TimeActionsCollection TimeActions { get; }
    public IntervalActionsCollection IntervalActions { get; }
    private int TimezoneShift { get; }
    
    public AtomicTimersHostedService(
        Service service, 
        IServiceProvider serviceProvider,
        TimeActionsCollection timeActions,
        IntervalActionsCollection intervalActions,
        IConfiguration configuration)
    {
        this.Service = service;
        this.ServiceProvider = serviceProvider;
        this.TimeActions = timeActions;
        this.IntervalActions = intervalActions;
        this.TimezoneShift = configuration.TryGetPropertyValue("TimezoneShift", 0);
    }

    private void Handle(string actionId)
    {
        var action = this.Service.Actions[actionId];

        using var scope = this.ServiceProvider.CreateScope();
        var pipe = scope.ServiceProvider.GetRequiredService<IPipe<EventContext>>();
        pipe.Send(new(action, new()));
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var intervalAction in this.Service.TimersBasedActions.IntervalActions)
        {
            var action = new Timers.IntervalAction(intervalAction.Action.Id, intervalAction.Attribute.Interval);
            this.IntervalActions.Add(action, this.Handle);
        }

        foreach (var specificTimesAction in this.Service.TimersBasedActions.SpecificTimesActions)
        {
            var action = TimesAction.WithTimeZoneShift(
                specificTimesAction.Action.Id,
                specificTimesAction.Attributes.Select(a => a.Time),
                this.TimezoneShift
            );
            
            this.TimeActions.Add(action, this.Handle);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        JobManager.Stop();
    }
}
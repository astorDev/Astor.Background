using Microsoft.Extensions.DependencyInjection;

namespace Astor.Timers;

public static class TimersCollectionsExtensions
{
    public static IServiceCollection AddTimerCollections(this IServiceCollection services)
    {
        services.AddSingleton<IntervalActionsCollection>();
        services.AddSingleton<TimeActionsCollection>();
        return services;
    }
}
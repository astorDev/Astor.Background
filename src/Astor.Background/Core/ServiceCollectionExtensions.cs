using System;
using System.Reflection;

using Astor.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace Astor.Background.Core
{
    public static class ServiceCollectionExtensions
    {
        public static void AddBackground(this IServiceCollection services, Service service)
        {
            services.AddSingleton(service);
            foreach (var controllerTypes in service.ControllerTypes)
            {
                services.AddScoped(controllerTypes);
            }
        }

        public static void AddBackgroundServiceDeclaration(this IServiceCollection services)
        {
            var callerType = StackTraceAnalyzer.GetCallerType();
            var serviceDeclaration = Service.Parse(callerType.Assembly);

            services.AddSingleton(serviceDeclaration);
        }
        
        public static void AddBackgroundServiceControllers(this IServiceCollection services)
        {
            var backgroundService = services.BuildServiceProvider().GetRequiredService<Service>();
            foreach (var controllerTypes in backgroundService.ControllerTypes) services.AddScoped(controllerTypes);
        }

        public static void AddBackground(this IServiceCollection services, Assembly assembly)
        {
            var service = Service.Parse(assembly);
            
            AddBackground(services, service);
        }

        public static void AddBackground(this IServiceCollection services, params Type[] types)
        {
            var service = Service.Parse(types);
            
            AddBackground(services, service);
        }
    }
}
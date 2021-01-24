using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Astor.Background
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
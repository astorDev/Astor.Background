using System.Reflection;

using Astor.Background.Core;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.Swagger;

namespace Astor.Background.Descriptions.OpenApiDocuments;

public static class ServiceCollectionExtensions
{
    public static void AddBackgroundServiceSwaggerGenerator(this IServiceCollection services)
    {
        var info = new OpenApiInfo { Title = Assembly.GetEntryAssembly()!.GetName().Name!, Version = "1.0" };
        services.AddSingleton(sp => ServiceDescriptionBuilder.Generate(sp.GetRequiredService<Service>(), info));
        services.AddSingleton<ISwaggerProvider, SingleServiceDescriptionSwaggerProvider>();
    }
}
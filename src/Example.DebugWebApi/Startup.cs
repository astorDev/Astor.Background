using Astor.Background;
using Astor.Background.Core;
using Astor.Background.Core.Filters;
using Astor.Background.Management;
using Astor.Background.Management.Scraper;
using Astor.GreenPipes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Example.DebugWebApi
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
            services.AddControllers()
                .AddNewtonsoftJson(o =>
                {
                    o.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    o.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });
            
            var backgroundServiceStartup = new Example.Service.Startup(this.Configuration);
            backgroundServiceStartup.ConfigureServices(services);
            
            services.AddPipe<EventContext>(pipe => 
                pipe.UseJsonBodyDeserializer()
                    .UseActionExecutor()
                );
            
            services.AddSingleton(sp =>
            {
                var service = sp.GetRequiredService<Astor.Background.Core.Service>();
                var description = service.GetDescription(new OpenApiInfo
                {
                    Title = "Example.DebugWebApi",
                    Version = "1.0"
                });

                return description.ToOpenApiDocument();
            });
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<BackgroundServiceBasedSwaggerMiddleware>();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Example.DebugWebApi v1"));

            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

                    var error = Error.Interpret(exceptionHandlerPathFeature.Error, true);

                    context.Response.StatusCode = (int) error.Code;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsync(JsonConvert.SerializeObject(error, new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver(),
                        NullValueHandling = NullValueHandling.Ignore
                    }));
                });
            });
            
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
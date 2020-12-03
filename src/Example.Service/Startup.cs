using System.Linq;
using Example.Service.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            var service = Astor.Background.Service.Parse(this.GetType().Assembly);

            services.AddSingleton(service);
            foreach (var controllerTypes in service.ControllerTypes)
            {
                services.AddScoped(controllerTypes);
            }
            
            services.Configure<GreetingPhrases>(this.Configuration.GetSection("Phrases"));
        }
    }
}
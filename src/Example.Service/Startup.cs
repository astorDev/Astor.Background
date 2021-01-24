using Astor.Background;
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
            services.AddBackground(this.GetType().Assembly);
            
            services.Configure<GreetingPhrases>(this.Configuration.GetSection("Phrases"));
        }
    }
}
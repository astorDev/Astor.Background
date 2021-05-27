using Example.Service.Controllers;
using Example.Service.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Astor.Background.Tests
{
    public class GreetingsBasedTest : Astor.Tests.Test
    {
        public override IServiceCollection CreateBaseServiceCollection()
        {
            var serviceCollection = base.CreateBaseServiceCollection();

            serviceCollection.AddLogging(l => l.AddSystemdConsole());
            
            serviceCollection.AddScoped<GreetingsController>();
            serviceCollection.AddSingleton(Options.Create(
                new GreetingPhrases
                {
                    Beginning = "AutoTestsHi"
                }));

            return serviceCollection;
        }
    }
}
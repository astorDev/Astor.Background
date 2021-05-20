using Astor.Background.ElasticLogs.DebugWebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Astor.Background.ElasticLogs.Tests
{
    public class WebApplicationFactory : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                Test.OverrideConfiguration(configBuilder);
            });
        }
    }
}
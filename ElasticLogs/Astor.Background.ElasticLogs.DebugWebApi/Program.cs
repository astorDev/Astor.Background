using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Astor.Background.ElasticLogs.DebugWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .ConfigureAppConfiguration((context, config) =>
                {
                    var dir = "bin/Debug/net5.0";
                    
                    config.AddJsonFile(Path.Combine(dir, "appsettings.json"));
                    config.AddJsonFile(Path.Combine(dir,
                        $"appsettings.{context.HostingEnvironment.EnvironmentName}.json"), true);
                });
    }
}
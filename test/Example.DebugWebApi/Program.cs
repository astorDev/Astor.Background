using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Example.DebugWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(System.IO.Path.GetDirectoryName( 
                System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase));

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
                        $"appsettings.{context.HostingEnvironment.EnvironmentName}.json"));
                });
    }
}
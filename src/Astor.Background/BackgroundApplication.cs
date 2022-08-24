using System.Threading.Tasks;

using Astor.Background.Core;
using Astor.GreenPipes;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Astor.Background;

public class BackgroundApplication
{
    readonly Builder builder;
    public PipeBuilder<EventContext> Pipe { get; }

    BackgroundApplication(Builder builder) {
        this.builder = builder;
        this.Pipe = new(builder.Services);
    }
    
    public async Task Run()
    {
        var hostBuilder = this.CreateHostBuilder();
        var host = hostBuilder.UseConsoleLifetime().Build();

        await host.RunAsync();
    }

    IHostBuilder CreateHostBuilder() =>
        new HostBuilder()
            .ConfigureAppConfiguration(config => config.AddConfiguration(this.builder.Configuration))
            .ConfigureServices(s =>
            {
                this.Pipe.RegisterPipe();
                foreach (var descriptor in this.builder.Services) s.Add(descriptor);
            });

    public class Builder
    {
        class LoggingBuilder : ILoggingBuilder {
            public IServiceCollection Services { get; }
            public LoggingBuilder(IServiceCollection services) { this.Services = services; }
        }
        
        public string Environment { get; }
        public ConfigurationManager Configuration { get; }
        public ServiceCollection Services { get; }
        public ILoggingBuilder Logging { get; }
        
        public Builder(string[]? args)
        {
            var configuration = new ConfigurationBuilder().AddCommandLine(args).Build();
            this.Environment = configuration["environment"];
            this.Configuration = new();
            this.Configuration.AddJsonFile("appsettings.json", optional: true);
            this.Configuration.AddJsonFile($"appsettings.{this.Environment}.json", optional: true);
            this.Configuration.AddConfiguration(configuration);

            this.Services = new();
            this.Logging = new LoggingBuilder(this.Services);
            this.Logging.AddConfiguration(this.Configuration.GetSection("Logging"));
        }

        public BackgroundApplication Build() => new(this);
    }
}
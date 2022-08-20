using System.Threading.Tasks;

using Astor.Background.Core;
using Astor.GreenPipes;
using Astor.Reflection;
using Astor.Timers;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Namotion.Reflection;

namespace Astor.Background;

public class BackgroundApplication
{
    readonly Builder builder;

    public PipeBuilder<EventContext> Pipe { get; }
    
    private BackgroundApplication(Builder builder)
    {
        this.builder = builder;
        this.Pipe = new(builder.Services);
    }
    
    public async Task Run()
    {
        var hostBuilder = this.CreateHostBuilder();
        var host = hostBuilder.UseConsoleLifetime().Build();
        

        await host.RunAsync();
    }

    IHostBuilder CreateHostBuilder()
    {
        return new HostBuilder()
            .ConfigureAppConfiguration(config => config.AddConfiguration(this.builder.Configuration))
            .ConfigureServices(s =>
            {
                this.Pipe.RegisterPipe();
                foreach (var descriptor in this.builder.Services) s.Add(descriptor);
            });
    }

    public static Builder CreateTimersOnlyBuilder(string[] args)
    {
        var builder = new Builder(args);
        
        builder.Services.AddSingleton<IHostedService, AtomicTimersHostedService>();
        
        var callerType = StackTraceAnalyzer.GetCallerType();
        builder.Services.AddBackground(callerType.Assembly);

        return builder;
    }
    
    public class Builder
    {
        class LoggingBuilder : ILoggingBuilder
        {
            public IServiceCollection Services { get; }
            public LoggingBuilder(IServiceCollection services) { this.Services = services; }
        }
        
        public string Environment { get; }
        
        public IConfiguration Configuration { get; }
        
        public ServiceCollection Services { get; }
        
        public ILoggingBuilder Logging { get; }
        
        public Builder(string[]? args)
        {
            var configuration = new ConfigurationBuilder().AddCommandLine(args).Build();
            this.Environment = configuration.TryGetPropertyValue("Environment", "")!;
            this.Configuration = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{Environment}.json", optional: true)
                .Build();
            
            this.Services = new();
            this.Logging = new LoggingBuilder(this.Services);
            this.Logging.AddConfiguration(this.Configuration.GetSection("Logging"));
        }

        public BackgroundApplication Build() => new(this);
    }
}
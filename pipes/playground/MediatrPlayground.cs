using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Astor.Background.Pipes.Playground;

[TestClass]
public class MediatrPlayground
{
    [TestMethod]
    public async Task Simple()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddLogging(l => l.AddSimpleConsole(c => c.SingleLine = true));
        serviceCollection.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(MediatrPlayground).Assembly);
            cfg.AddOpenBehavior(typeof(PeriodicCommandLoggingBehavior<,>));
        });

        var sp = serviceCollection.BuildServiceProvider();
        using var scope = sp.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Send(new ExampleNonPeriodicCommand());
        await mediator.Send(new ExamplePeriodicCommand());
    }
}

class PeriodicCommandLoggingBehavior<TRequest, TResponse>(ILogger<PeriodicCommandLoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse> 
{
    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is IPeriodicAction)
        {
            logger.LogInformation("Handling periodic command {requestType}", request.GetType().Name);
        }

        return next();
    }
}

public interface IPeriodicAction : IRequest;

public class ExampleNonPeriodicCommand : IRequest
{
    public class Handler(ILogger<Handler> logger) : IRequestHandler<ExampleNonPeriodicCommand>
    {
        public async Task Handle(ExampleNonPeriodicCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Inside NonPeriodicCommand Handle");
        }
    }
}

public class ExamplePeriodicCommand : IPeriodicAction
{
    public class Handler(ILogger<Handler> logger) : IRequestHandler<ExamplePeriodicCommand>
    {
        public async Task Handle(ExamplePeriodicCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Inside PeriodicCommand Handle");
        }
    }
}
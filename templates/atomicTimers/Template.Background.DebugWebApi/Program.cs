var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("bin/Debug/net6.0/appsettings.json");
builder.Configuration.AddJsonFile($"bin/Debug/net6.0/appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

builder.Services.AddBackgroundServiceDeclaration();
builder.Services.AddBackgroundServiceControllers();

builder.Services.AddApplicationServices();
builder.Services.AddBackgroundServiceSwaggerGenerator();

builder.Services.AddPipe<EventContext>(p => 
    p.Use<IOLogger>()
     .Use<HandlingTimer>()
     .Use<JsonBodyDeserializer>()
     .Use<ActionExecutor>()
);

builder.Logging.AddSimpleConsole(c => c.SingleLine = true);
builder.Logging.AddStateJsonConsole(j => j.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/{actionId}", async (Service service, IPipe<EventContext> pipe, string actionId, [FromBody] object? request) =>
{
    var action = service.Actions[actionId];
    var context = new EventContext(action, new() { BodyString = JsonSerializer.Serialize(request) });
            
    await pipe.Send(context);
    return context.ActionResult.Output;
});

app.Run();
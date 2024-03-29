# Astor.Background

Micro-framework for creating .Net background services, with actions triggered either by events or time-based.  
It's just like ASP .NET Core MVC, but for background work.

## Installation

### 1. Deploy Background.Management.Service  
The service is the basic management tool for the whole background infrastructure. What it does is:

- Logs action result in an actionable way. Namely, stores them in MongoDB
- Manages time-based actions. This includes storing schedules and triggering events based on them.

The recommended way to install it is by using [docker image](https://hub.docker.com/repository/docker/vosarat/background-management-service) like this:

```yml
version: '3.9'

services: 
  service:
    image: vosarat/background-management-service
    environment:
      - RABBIT_CONNECTIONSTRING=<Connection String To RabbitMQ>
      - MONGO_CONNECTIONSTRING=<Connection String To MongoDb>
      - TELEGRAM_TOKEN=<Your Telegram Token>
      - TELEGRAM_CHATID=<Your Telegram Chat Id>
      - INTERNALEXCHANGEPREFIX=<Prefix for exchanges created specifically for that service>
      - TIMEZONESHIFT=<Integer, specifying shift from UTC timezone>
```

| Parameter | Why And What? | Example |
| ------------ | :-----------: | :----------: |
| RABBIT_CONNECTIONSTRING | The whole framework is built on top of the RabbitMq. The background management service needs it to receive logs and schedules and trigger time-based events | amqp://localhost:5672 |
| MONGO_CONNECTIONSTRING | Here it will store the results of an action | mongodb://localhost:27017 |
| TELEGRAM_TOKEN | Bot will send you messages if something goes wrong. Since it's considered crucial notification and we have not yet any other notification mechanism, the bot token is mandatory | 111111111:AAAAAAAAAAAAAAAAAAAAAAAAAAA |
| TELEGRAM_CHATID | Bot also needs a chat for notifications | -11111111111 |
| INTERNALEXCHANGEPREFIX | The prefix is used for creating exchanges for internal service events. Since there are actions triggered on the service start, this parameter is mandatory | my |
| TIMEZONESHIFT | If you are not in the UTC zone, you can shift schedules to your time zone by this optional parameter | -3 |

### 2. Install the template

The easiest way to create a new background service is by using a dotnet new template.  
You can [install it from a file system directory](https://docs.microsoft.com/en-us/dotnet/core/tools/custom-templates#to-install-a-template-from-a-file-system-directory) as following:
```sh
git clone https://github.com/astorDev/Astor.Background.git
cd Astor.Background
dotnet new -i templates
```

### 3. Enjoy

Now, if you run `dotnet new bg --name MyApp` it will create the folder with two projects in it:
- MyApp.Background.Service - the actual background service to run
- MyApp.Background.DebugWebApi - An Web Api to use only for debugging properties

## Usage

Let's start with the template controller.

```csharp
public class TemplateController
{
    [Astor.Background.RabbitMq.Abstractions.SubscribedOn("numbers.supply", DeclareExchange = true)]
    public Task<int> TryParse(string num)
    {
        Int32.TryParse(num, out var n);
        return Task.FromResult(n);
    }
    
    [RunsEvery("00:00:10")]
    public Task<string> GetTime()
    {
        return Task.FromResult($"Current Time is {DateTime.Now}");
    }
}
```

The first action will:
- Declare exchange `numbers.supply`. If you don't need this, just remove DeclareExchange
- Declare queue `Template_TryParse`
- Bind `Template_TryParse` with `numbers.supply`
- Consume `Template_TryParse` by running the action method with arguments deserialized from json received from the queue

The second action will:
- Declare queue `Template_GetTime`
- Publishes the service schedule so that the action will be fired once in 10 seconds. If you need it to run at specific times, use `RunsEveryDayAt`.
- Consume `Template_GetTime` by running the action method

In addition to that, every action execution result will be published to `background-logs` exchange for further consumption by Background.Management.Service. The remaining is up to you to implement. The framework tries to look as close to the ASP .NET MVC as possible, so the remaining should look familiar, including dependency injection.
